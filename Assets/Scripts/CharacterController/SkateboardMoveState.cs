using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class SkateboardMoveState : SkateboardBaseState
{
    private Camera mainCam;
    private bool useMouse;
    public bool isOnGround;
    private int wheelsOnGround;
    private Vector2 leanInput;
    private Vector3 upVector;

    public bool jump;
    public bool flipped;
    public float jumpTimer;
    public float pushTimer;

    public Truck[] trucks = new Truck[4];

    public Transform transform => sm.transform;
    public SkateboardMoveSettings settings => sm.moveSettings;
    public Rigidbody body => sm.MainRB;
    public float steer { get; private set; }

    public SkateboardMoveState(SkateboardStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        sm.Input.OnPushPerformed += OnPush;
        sm.Input.OnSwitchPerformed += OnSwitch;
        sm.Input.OnPausePerformed += sm.Pause;
        sm.Input.OnStartBraking += StartBrake;
        sm.Input.OnEndBraking += EndBrake;
        sm.ComboActions["ollie"] += OnOllieTrickInput;
        sm.ComboActions["kickflip"] += OnOllieTrickInput;
        sm.ComboActions["heelflip"] += OnOllieTrickInput;
        sm.ComboActions["popShuvit"] += OnOllieTrickInput;
        
        InitTrucks();
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        sm.HeadSensZone.AddCallback(sm.Die);

        StartRollingSFX();
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
        pushTimer = 1.0f;
    }

    public override void Tick()
    {
        CreateDebugFrame();

        var rawSteerInput = sm.Input.turn;
        steer += (rawSteerInput * settings.maxSteer - steer) * (1.0f - settings.steerInputSmoothing);

        body.centerOfMass = settings.localCenterOfMass;
        body.inertiaTensor = settings.inertiaTensor;

        UpdateTrucks();
        DoPrediction();
        ApplyResistance();
        ApplyPushForce();
        ApplyLeanForces();

        //sm.collisionProcessor.FixedUpdate(sm);
        PassGroundSpeedToPointSystem();
        PassSpeedToMotionBlur();

        SaveDebugFrame();
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

    public override void Exit()
    {
        sm.Input.OnPushPerformed -= OnPush;
        sm.Input.OnSwitchPerformed -= OnSwitch;
        sm.Input.OnPausePerformed -= sm.Pause;
        sm.Input.OnStartBraking -= StartBrake;
        sm.Input.OnEndBraking -= EndBrake;
        sm.ComboActions["ollie"] -= OnOllieTrickInput;
        sm.ComboActions["kickflip"] -= OnOllieTrickInput;
        sm.ComboActions["heelflip"] -= OnOllieTrickInput;
        sm.ComboActions["popShuvit"] -= OnOllieTrickInput;

        sm.HeadSensZone.RemoveCallback(sm.Die);

        StopRollingSFX();
    }

    private void ApplyLeanForces()
    {
        var up = upVector;
        var weight = upVector.magnitude;
        var lean = transform.right * steer * settings.rotationalLean;
        var target = (up + lean).normalized * weight;
        var cross = Vector3.Cross(transform.up, target);

        var angularVelocity = body.angularVelocity;
        angularVelocity -= Vector3.Project(angularVelocity, target);
        
        var torque = cross * settings.rotationalForce - angularVelocity * settings.rotationalDamping;
        body.AddTorque(torque * body.mass);
        
        Debug.DrawRay(transform.position, target, Color.cyan);
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
        

        isOnGround = wheelsOnGround > 0;
        if (isOnGround) upVector = up.normalized;
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