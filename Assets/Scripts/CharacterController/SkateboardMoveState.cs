using CharacterController;
using UnityEngine;

public class SkateboardMoveState : SkateboardBaseState
{
    private Camera mainCam;
    private bool useMouse;
    public float airborneTimer;
    private int wheelsOnGround;
    private Vector2 leanInput;
    private Vector3 upVector;
    private Vector3 leanVector;

    private Transform cameraTargetTransform;
    private Vector3 cameraTargetPosition;
    private Vector3 cameraTargetVelocity;
    private Quaternion cameraTargetRotation;

    public bool jump;
    public bool flipped;
    public float jumpTimer;
    public float pushTimer;
    private SkateboardMoveAnimator animator;

    public Truck[] trucks = new Truck[4];

    public Transform transform => sm.transform;
    public SkateboardMoveSettings settings => sm.moveSettings;
    public Rigidbody body => sm.MainRB;
    public float steer { get; private set; }
    public Vector3 Gravity => Physics.gravity * (body.velocity.y > 0.0f || isOnGround ? settings.upGravity : settings.downGravity);
    public bool isOnGround
    {
        get => sm.Grounded;
        set => sm.Grounded = value;
    }

    public SkateboardMoveState(SkateboardStateMachine stateMachine) : base(stateMachine)
    {
        animator = new SkateboardMoveAnimator(this);
        cameraTargetTransform = stateMachine.transform.Find("cameraTarget");
    }

    public override void Enter()
    {
        sm.Input.OnPushPerformed += OnPush;
        sm.Input.OnSwitchPerformed += OnSwitch2;
        sm.Input.OnPausePerformed += sm.Pause;
        sm.Input.OnStartBraking += StartBrake;
        sm.Input.OnEndBraking += EndBrake;
        sm.ComboActions["ollie"] += OnHopTrickInput;
        sm.ComboActions["kickflip"] += OnHopTrickInput;
        sm.ComboActions["heelflip"] += OnHopTrickInput;
        sm.ComboActions["popShuvit"] += OnHopTrickInput;

        InitTrucks();
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        sm.HeadSensZone.AddCallback(sm.Die);

        StartRollingSFX();
    }


    public override void Exit()
    {
        sm.Input.OnPushPerformed -= OnPush;
        sm.Input.OnSwitchPerformed -= OnSwitch2;
        sm.Input.OnPausePerformed -= sm.Pause;
        sm.Input.OnStartBraking -= StartBrake;
        sm.Input.OnEndBraking -= EndBrake;
        sm.ComboActions["ollie"] -= OnHopTrickInput;
        sm.ComboActions["kickflip"] -= OnHopTrickInput;
        sm.ComboActions["heelflip"] -= OnHopTrickInput;
        sm.ComboActions["popShuvit"] -= OnHopTrickInput;

        sm.HeadSensZone.RemoveCallback(sm.Die);

        SetWheelSpinParticleChance();
        SetSpeedyLines();
        SetRollingVolume();
        StopRollingSFX();
        PassGroundSpeedToPointSystem();
        PassSpeedToMotionBlur();
    }

    private void OnSwitch2()
    {
        transform.rotation *= Quaternion.Euler(Vector3.up * 180.0f);
    }

    private void InitTrucks()
    {
        trucks[0] = new Truck(this, -1, 1);
        trucks[1] = new Truck(this, 1, 1);
        trucks[2] = new Truck(this, -1, -1);
        trucks[3] = new Truck(this, 1, -1);
    }

    private void OnPush()
    {
        if (pushTimer > 0.0) return;
        sm.CharacterAnimator.SetTrigger("push");
        pushTimer = 1.0f;
    }

    public override void Tick()
    {
        CreateDebugFrame();

        var rawSteerInput = sm.Input.turn;
        steer += (rawSteerInput * settings.maxSteer - steer) * (1.0f - settings.steerInputSmoothing);

        body.centerOfMass = settings.localCenterOfMass;
        body.inertiaTensor = settings.inertiaTensor;
        
        sm.CharacterAnimator.SetBool("crouching", sm.Input.crouching);

        UpdateTrucks();
        DoPrediction();
        ApplyResistance();
        ApplyPushForce();
        ApplyLeanForces();
        ApplyBrakeForce();
        CheckForWalls();
        UpdateCamera();
        animator.Tick();
        
        body.AddForce(Gravity - Physics.gravity, ForceMode.Acceleration);

        //sm.collisionProcessor.FixedUpdate(sm);
        PassGroundSpeedToPointSystem();
        PassSpeedToMotionBlur();

        SaveDebugFrame();
    }

    private void UpdateCamera()
    {
        if (!cameraTargetTransform) return;

        var force = (body.position + new Vector3(0.0f, 0.9590001f, 0.0f) - cameraTargetPosition) * settings.cameraTargetSpring + (body.velocity - cameraTargetVelocity) * settings.cameraTargetDamp;
        
        cameraTargetPosition += cameraTargetVelocity * Time.deltaTime;
        cameraTargetVelocity += force * Time.deltaTime;
        cameraTargetTransform.position = cameraTargetPosition;
        
        if (isOnGround)
        {
            cameraTargetRotation = sm.FacingParent.rotation;
        }

        cameraTargetTransform.rotation = cameraTargetRotation;
    }

    private float GetForwardSpeed() => Vector3.Dot(transform.forward, body.velocity);

    private void CheckForWalls()
    {
        var fwdSpeed = GetForwardSpeed();
        var ray = new Ray(transform.position, transform.forward);
        if (!Physics.Raycast(ray, out var hit, settings.wallSlideDistance)) return;

        var cross = Vector3.Cross(ray.direction, hit.normal * (1.0f - hit.distance / settings.wallSlideDistance));
        var torque = cross  * settings.wallSlideTorque * fwdSpeed;
        body.AddTorque(torque);
    }

    private void ApplyBrakeForce()
    {
        if (!sm.Input.braking) return;

        var fwdSpeed = GetForwardSpeed();
        var friction = Mathf.Lerp(settings.staticBrake, settings.dynamicBrake, settings.lastEvaluatedBrakeThreshold = Mathf.Abs(fwdSpeed) / settings.brakeThreshold);
        settings.lastEvaluatedBrakeThreshold = Mathf.Clamp01(settings.lastEvaluatedBrakeThreshold);

        var force = transform.forward * -fwdSpeed * Mathf.Min(friction, 1.0f);
        body.AddForce(force * body.mass / Time.deltaTime);
    }

    private void DoPrediction()
    {
        if (isOnGround) return;

        var position = body.position;
        var velocity = body.velocity;
        var force = Vector3.zero;
        var deltaTime = settings.predictionTimestep;

        for (var t = 0.0f; t < settings.predictionMaxTime; t += deltaTime)
        {
            var nextPosition = position + velocity * deltaTime;

            if (Physics.Linecast(position, nextPosition, out var hit))
            {
                if (!ValidateHit(hit)) return;

                Debug.DrawLine(position, hit.point, Color.magenta);
                Debug.DrawRay(hit.point, hit.normal * 2.0f, Color.magenta);
                upVector = hit.normal * settings.predictionWeight;
                break;
            }

            Debug.DrawLine(position, nextPosition, Color.magenta);

            position = nextPosition;
            velocity += force * deltaTime;
            force = Physics.gravity;
        }
    }

    private bool ValidateHit(RaycastHit hit)
    {
        if (hit.collider.transform.IsChildOf(transform)) return false;

        return true;
    }

    private void ApplyLeanForces()
    {
        var up = upVector;
        var weight = upVector.magnitude;
        leanVector = transform.right * steer * settings.rotationalLean;
        var target = (up + leanVector).normalized * weight;
        var cross = Vector3.Cross(transform.up, target);

        var angularVelocity = body.angularVelocity;
        angularVelocity -= Vector3.Project(angularVelocity, target);

        var torque = cross * settings.rotationalForce - angularVelocity * settings.rotationalDamping;

        if (!isOnGround && (airborneTimer > settings.spinDelay || sm.Input.crouching))
        {
            var rps = sm.Input.crouching ? settings.spinMaxRpsCrouching : settings.spinMaxRps;
            var targetSpin = rps * Mathf.PI * 2.0f * steer / settings.maxSteer;
            var current = Vector3.Dot(transform.up, body.angularVelocity);
            torque += transform.up * (targetSpin - current) * settings.spinAcceleration;
        }

        body.AddTorque(torque, ForceMode.Acceleration);
    }

    private void ApplyPushForce()
    {
        if (pushTimer < 0.0f) return;

        var forwardSpeed = Vector3.Dot(transform.forward, body.velocity);
        var force = transform.forward * (settings.maxSpeed - forwardSpeed) * settings.acceleration * settings.pushCurve.Evaluate(pushTimer);
        force *= wheelsOnGround / 4.0f;
        body.AddForce(force * body.mass);

        pushTimer -= Time.deltaTime / settings.pushDuration;
    }

    private void UpdateTrucks()
    {
        wheelsOnGround = 0;
        var up = Vector3.zero;
        foreach (var e in trucks)
        {
            e.Process();
            if (!e.isOnGround) continue;

            wheelsOnGround++;
            up += e.groundHit.normal;
        }

        var wasOnGround = isOnGround;
        isOnGround = wheelsOnGround > 0;
        if (isOnGround)
        {
            if (!wasOnGround) sm.CharacterAnimator.SetTrigger("startAirborne");
            
            upVector = up.normalized;
            airborneTimer = 0.0f;
        }
        else airborneTimer += Time.deltaTime;
        sm.CharacterAnimator.SetBool("falling", !isOnGround && Vector3.Dot(body.velocity, Vector3.up) < 0.0f);
    }

    private void ApplyResistance()
    {
        var velocity = body.velocity;

        var force = -velocity.normalized * velocity.sqrMagnitude * settings.airResistance;
        force -= Vector3.Project(velocity, transform.forward) * settings.rollingResistance;
        body.AddForce(force * body.mass, ForceMode.Acceleration);
    }

    public override void DrawGizmos(bool selected)
    {
        if (!settings) return;

        if (!Application.isPlaying) InitTrucks();
        foreach (var e in trucks) e.DrawGizmos();
    }

    public class Truck
    {
        public SkateboardMoveState state;

        public RaycastHit groundHit;
        public bool isOnGround;
        public Ray groundRay;
        public float groundRayLength;
        public int xSign, zSign;
        public float rotation;
        public float evaluatedTangentialFriction;

        public Vector3 Position => state.sm.transform.TransformPoint(Settings.truckOffset.x * xSign, Settings.truckOffset.y, Settings.truckOffset.z * zSign);
        public Quaternion Rotation => state.sm.transform.rotation * Quaternion.Euler(0.0f, state.steer * zSign, 0.0f);
        private SkateboardMoveSettings Settings => state.sm.moveSettings;
        public Rigidbody Body => state.sm.MainRB;

        public Truck(SkateboardMoveState state, int xSign, int zSign)
        {
            this.state = state;

            this.xSign = xSign > 0 ? 1 : -1;
            this.zSign = zSign > 0 ? 1 : -1;
        }

        public void Process()
        {
            Orient();
            LookForGround();
            Depenetrate();
            ApplySidewaysFriction();
        }

        private void Orient()
        {
            if (isOnGround)
            {
                var velocity = Body.GetPointVelocity(groundHit.point);
                var speed = Vector3.Dot(velocity, state.transform.forward);
                rotation += speed / Settings.wheelRadius * Time.deltaTime * Mathf.Rad2Deg;
            }
        }

        private void LookForGround()
        {
            GetGroundRay();

            isOnGround = false;
            var results = Physics.RaycastAll(groundRay, groundRayLength);
            var best = float.MaxValue;

            foreach (var e in results)
            {
                if (e.transform.IsChildOf(state.sm.transform)) continue;
                if (e.distance > best) continue;

                best = e.distance;
                groundHit = e;
                isOnGround = true;
            }
        }

        private void GetGroundRay()
        {
            groundRayLength = Settings.distanceToGround;
            groundRay = new Ray(Position, -state.transform.up);
        }

        private void Depenetrate()
        {
            var force = Vector3.zero;

            if (isOnGround)
            {
                var point = groundHit.point;
                var normal = groundHit.normal;
                force += Vector3.Project(groundHit.normal * (groundRayLength - groundHit.distance), normal) * Settings.truckDepenetrationSpring;

                var velocity = Body.GetPointVelocity(point);
                var dot = Vector3.Dot(velocity, normal);
                force += normal * Mathf.Max(0.0f, -dot) * Settings.truckDepenetrationDamper;
                Body.AddForceAtPosition(force / 8 * Body.mass, point);

                Debug.DrawLine(groundHit.point, point, Color.red);
            }
        }

        private void ApplySidewaysFriction()
        {
            if (!isOnGround) return;

            var right = Rotation * Vector3.right;
            var velocity = Body.GetPointVelocity(groundHit.point);
            var dot = Vector3.Dot(right, -velocity);
            evaluatedTangentialFriction = dot * Settings.tangentialFriction;
            var force = right * evaluatedTangentialFriction;

            Body.AddForceAtPosition(force * Body.mass, groundHit.point);
        }

        public void DrawGizmos()
        {
            GetGroundRay();
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(Body.worldCenterOfMass, 0.1f);
            Gizmos.DrawLine(groundRay.origin, groundRay.GetPoint(groundRayLength));
        }
    }
}