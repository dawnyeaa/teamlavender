using UnityEngine;
using UnityEngine.Animations;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputController))]
// [RequireComponent(typeof(WheelController))]
public class SkateboardStateMachine : StateMachine {
  // User Constants - Runtime only
  // [Header("Constants - Only read at runtime")]
  // User Constants - Live update
  [Header("Constants - Live update")]
  public float MaxSpeed = 20f;
  public float TurnLockSpeed = 30f;
  public float PushForce = 10f;
  public AnimationCurve PushForceCurve;
  public float MaxPushDuration = 1f;
  public float WheelFriction = 0.01f;
  public float BrakingFriction = 0.4f;
  public float MaxTruckTurnDeg = 8.34f;
  public float TruckSpacing = 0.205f;
  public float TruckTurnDamping = 0.3f;
  public float SpringConstant = 40f;
  public float SpringMultiplierMin = 0.5f;
  public float SpringMultiplierMax = 1f;
  public float SpringDamping = 1f;
  public float ProjectRadius = 0.25f;
  public float ProjectLength = 1.5f;
  public float RightingStrength = 1f;
  // public float EdgeSafeSpeedEpsilon = 0.1f;
  // public float EdgeSafeAngle = 60f;
  public float GoingDownThreshold = -0.1f;
  public float LandingAngleGive = 0.8f;
  public float AirTurningDrag = 1f;
  public AnimationCurve TurningEase;
  [Range(0, 1)] public float TruckGripFactor = 0.8f;
  public float BoardPositionDamping = 1f;
  public float PushingMaxSlope = 5f;
  public float OllieForce = 1f;
  public float DeadTime = 3f;
  public float MinHeadZoneSize = 2.4f;
  public float MaxHeadZoneSize = 6f;
  [Range(0, 1)] public float HeadZoneSpeedToHorizontalRatio = 0.5f;
  public float MinimumAirTime = 0.5f;
  public float PointsPerAirTimeSecond = 100f;
  // Internal State Processing
  [Header("Internal State")]
  [ReadOnly] public bool FacingForward = true;
  [ReadOnly] public bool Grounded = true;
  [ReadOnly] public float TruckTurnPercent;
  [ReadOnly] public float SpringMultiplier;
  [ReadOnly] public Vector3 Down = Vector3.down;
  [ReadOnly] public Vector3 DampedDown = Vector3.down;
  [ReadOnly] public float CurrentProjectLength;
  [ReadOnly] public float CurrentPushT = 0;
  [ReadOnly] public float AirTimeCounter = 0;
  // Objects to link
  [Header("Link Slot Objects")]
  public PhysicMaterial PhysMat;
  public Rigidbody BoardRb;
  public Transform frontAxis, backAxis;
  public Rigidbody FacingParentRB, FacingRB;
  public Transform MainCamera { get; private set; }
  public InputController Input { get; private set; }
  // public WheelController Wheels { get; private set; }

  public Transform footRepresentation;
  public Transform BodyMesh;
  public Animator CharacterAnimator;
  public Transform RegularModel, RagdollModel;
  public Rigidbody[] RagdollTransformsToPush;
  public ParentConstraint LookatConstraint;
  public TransformHeirarchyMatch RagdollMatcher;
  public SpawnPointManager SpawnPointManager;
  public HeadSensWrapper HeadSensZone;
  public PointManager PointManager;

  [HideInInspector] public Transform ball1, ball2, ball3;

  private void Start() {
    MainCamera = Camera.main.transform;

    Input = GetComponent<InputController>();
    // Wheels = GetComponent<WheelController>();

    SwitchState(new SkateboardMoveState(this));

    HeadSensZone.AddCallback(EnterDead);
    HeadSensZone.AddCallback(SlamRumble);

    Input.OnSlamPerformed += EnterDead;
    Input.OnSlamPerformed += SlamRumble;
  }

  public async void EnterDead() {
    SwitchState(new SkateboardDeadState(this));
    await Task.Delay((int)(DeadTime*1000));
    SwitchState(new SkateboardMoveState(this));
  }

  public async void SlamRumble() {
    if (Gamepad.current != null) {
      Gamepad.current.SetMotorSpeeds(0.25f, 0.75f);
      await Task.Delay(100);
      Gamepad.current.SetMotorSpeeds(0, 0);
    }
  }
}