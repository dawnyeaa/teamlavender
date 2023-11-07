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

    // private Transform cameraTargetTransform;
    private CameraTargetController cameraTarget;
    private Vector3 cameraTargetPosition;
    private Vector3 cameraTargetVelocity;
    private Quaternion cameraTargetRotation;

    public bool jump;
    public bool flipped;
    public float jumpTimer;
    public float pushTimer;

    public Truck[] trucks = new Truck[4];

    public Transform transform => sm.transform;
    public SkateboardMoveSettings settings => sm.moveSettings;
    public Rigidbody body => sm.MainRB;
    public float steer { get; private set; }
    public float modifiedSteer { get; private set; }
    public Vector3 Gravity => Physics.gravity * (body.velocity.y > 0.0f || isOnGround ? settings.upGravity : settings.downGravity);
    public bool isOnGround
    {
        get => sm.Grounded;
        set => sm.Grounded = value;
    }

    public ContinuousDataStepper slopeCrouch;
    private float slopeCrouchDampingSpeed = 0;

    public SkateboardMoveState(SkateboardStateMachine stateMachine) : base(stateMachine)
    {
        //animator = new SkateboardMoveAnimator(this);
        // cameraTargetTransform = stateMachine.transform.Find("cameraTarget");
        cameraTarget = stateMachine.transform.Find("cameraTarget").GetComponent<CameraTargetController>();
        slopeCrouch = new ContinuousDataStepper(0, settings.slopeCrouchFPS);
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
        sm.ComboActions["nollie"] += OnHopTrickInput;
        sm.ComboActions["nollieKickflip"] += OnHopTrickInput;
        sm.ComboActions["nollieHeelflip"] += OnHopTrickInput;

        InitTrucks();
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        sm.PointHandler.SetMaxSpeed(settings.maxSpeed);

        // StartRollingSFX();
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
        sm.ComboActions["nollie"] -= OnHopTrickInput;
        sm.ComboActions["nollieKickflip"] -= OnHopTrickInput;
        sm.ComboActions["nollieHeelflip"] -= OnHopTrickInput;

        StopRollingSFX();
        PassGroundSpeedToPointSystem();
        PassSpeedToMotionBlur();
    }

    private void OnSwitch2()
    {
        sm.IsGoofy = !sm.IsGoofy;
        sm.CharacterAnimator.SetFloat("mirrored", sm.IsGoofy ? 1 : 0);
        sm.Facing.rotation *= Quaternion.Euler(Vector3.up * 180.0f);
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

        var up = upVector;
        var slopeAngle = Vector3.Angle(up, Vector3.up);
        if (slopeAngle > settings.pushingMaxSlope) 
        {
            pushTimer = 0;
            return;
        }
        sm.CharacterAnimator.SetTrigger("push");
        pushTimer = 1.0f;
    }

    public override void Tick()
    {
        CreateDebugFrame();

        var rawSteerInput = sm.Input.turn;
        steer += (rawSteerInput * settings.maxSteer - steer) * (1.0f - settings.steerInputSmoothing);

        modifiedSteer = steer * GoofyMultiplier();
        
        float normalizedSteer = 0.5f * modifiedSteer / sm.MaxAnimatedTruckTurnDeg + 0.5f;
        sm.CharacterAnimator.SetFloat("leanValue", normalizedSteer);
        sm.BoardIKTiltAnimator.SetFloat("leanValue", normalizedSteer);

        body.centerOfMass = settings.localCenterOfMass;
        body.inertiaTensor = settings.inertiaTensor;
        
        SetCrouching();
        UpdateTrucks();
        DoPrediction();
        ApplyResistance();
        ApplyPushForce();
        ApplyLeanForces();
        ApplyBrakeForce();
        CheckForWalls();
        UpdateCamera();
        SetSlopeCrouching();
        SetWheelSpinParticleChance();
        SetSpeedyLines();
        CheckFacing();
        SpinWheels();
        // SetRollingVolume();
        //animator.Tick();
        
        body.AddForce(Gravity - Physics.gravity, ForceMode.Acceleration);

        sm.collisionProcessor.FixedUpdate(sm);
        PassGroundSpeedToPointSystem();
        PassSpeedToMotionBlur();

        SaveDebugFrame();
    }

    private void UpdateCamera()
    {
        if (!cameraTarget) return;
        
        cameraTarget.update = isOnGround;
    }

    private void SetSlopeCrouching()
    {
        var horizontalness = Mathf.Clamp01(1-Vector3.Dot(transform.up, Vector3.up));
        var speedFactor = GetForwardSpeed()/settings.maxSpeed;
        var crouchTarget = horizontalness + settings.speedCrouchCurve.Evaluate(speedFactor);
        slopeCrouch.Tick(Mathf.SmoothDamp(slopeCrouch.GetContinuous(), crouchTarget, ref slopeCrouchDampingSpeed, settings.slopeCrouchDamping), Time.deltaTime);
        sm.CharacterAnimator.SetLayerWeight(5, slopeCrouch.GetStepped());
    }

    private void CheckFacing() 
    {
        if (GetForwardSpeed() < -settings.autoSwitchThreshold) OnSwitch2();
    }

    private int GoofyMultiplier() => sm.IsGoofy ? -1 : 1;

    private Vector3 GetForward() => transform.forward * GoofyMultiplier();

    private float GetForwardSpeed() => Vector3.Dot(GetForward(), body.velocity);

    private void CheckForWalls()
    {
        var fwdSpeed = GetForwardSpeed();
        var ray = new Ray(transform.position, GetForward());
        if (!Physics.Raycast(ray, out var hit, settings.wallSlideDistance, LayerMask.NameToLayer("Ground"))) return;

        var cross = Vector3.Cross(ray.direction, hit.normal * (1.0f - hit.distance / settings.wallSlideDistance));
        var torque = cross  * settings.wallSlideTorque * fwdSpeed;
        body.AddTorque(torque);
    }

    private void SetCrouching() 
    {
        if ((sm.Input.crouching && !sm.CharacterAnimator.GetBool("crouching")) ||
            (sm.Input.nolliecrouching && !sm.CharacterAnimator.GetBool("nollieCrouching"))) 
        {
            if (pushTimer > 0) 
            {
                sm.CharacterAnimator.Play("idle");
                pushTimer = 0;
            }
        }
        sm.CharacterAnimator.SetBool("crouching", sm.Input.crouching);
        sm.CharacterAnimator.SetBool("nollieCrouching", sm.Input.nolliecrouching);
    }

    private void ApplyBrakeForce()
    {
        if (!sm.Input.braking) return;

        var fwdSpeed = GetForwardSpeed();
        var friction = Mathf.Lerp(settings.staticBrake, settings.dynamicBrake, settings.lastEvaluatedBrakeThreshold = Mathf.Abs(fwdSpeed) / settings.brakeThreshold);
        settings.lastEvaluatedBrakeThreshold = Mathf.Clamp01(settings.lastEvaluatedBrakeThreshold);

        var force = GetForward() * -fwdSpeed * Mathf.Min(friction, 1.0f);
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
                sm.TimeToLand = t;
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
        leanVector = transform.right * modifiedSteer * settings.rotationalLean;
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
        if (!isOnGround) return;
        if (pushTimer <= 0.0f) return;

        var up = upVector;
        var slopeAngle = Vector3.Angle(up, Vector3.up);
        if (slopeAngle > settings.pushingMaxSlope)
        {
            sm.CharacterAnimator.Play("idle");
            pushTimer = 0;
            return;
        }

        var forwardSpeed = Vector3.Dot(GetForward(), body.velocity);
        var force = GetForward() * (settings.maxSpeed - forwardSpeed) * settings.acceleration * settings.pushCurve.Evaluate(pushTimer);
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
        sm.CharacterAnimator.SetBool("grounded", isOnGround);
        if (isOnGround)
        {
            if (!wasOnGround) 
            {
                sm.CharacterAnimator.SetTrigger("startAirborne");
                // uncommenting this line can look real jank
                // sm.CharacterAnimator.SetFloat("landStrength", airborneTimer/1f);
            }
            
            upVector = up.normalized;
            airborneTimer = 0.0f;
        }
        else airborneTimer += Time.deltaTime;
        if (!isOnGround && Vector3.Dot(body.velocity, Vector3.up) < 0.0f)
        {
            sm.CharacterAnimator.SetBool("falling", true);
            sm.CharacterAnimator.SetInteger("Ollie", 0);
            sm.CharacterAnimator.SetInteger("Nollie", 0);
        }
        else
        {
            sm.CharacterAnimator.SetBool("falling", false);
        }
    }

    private void ApplyResistance()
    {
        var velocity = body.velocity;

        var force = -velocity.normalized * velocity.sqrMagnitude * settings.airResistance;
        force -= Vector3.Project(velocity, GetForward()) * settings.rollingResistance;
        body.AddForce(force * body.mass, ForceMode.Acceleration);
    }

    private void SpinWheels() 
    {
        float circum = ThisIsJustTau.TAU * settings.wheelRadius;
        float rotation = Time.fixedDeltaTime*GetForwardSpeed()*GoofyMultiplier()/circum;
        sm.WheelSpinAnimationController.AddRotation(rotation*360f);
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
        public Quaternion Rotation => state.sm.transform.rotation * Quaternion.Euler(0.0f, state.modifiedSteer * zSign, 0.0f);
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
                var speed = Vector3.Dot(velocity, state.GetForward());
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