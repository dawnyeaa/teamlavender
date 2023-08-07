using UnityEngine;

[RequireComponent(typeof(InputController))]
// [RequireComponent(typeof(WheelController))]
public class Skateboard4StateMachine : StateMachine {
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
  public AnimationCurve TurningEase;
  [Range(0, 1)] public float TruckGripFactor = 0.8f;
  public float BoardPositionDamping = 1f;
  public float PushingMaxSlope = 5f;
  public float OllieForce = 1f;
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

  [HideInInspector] public Transform ball1, ball2, ball3;

  private void Start() {
    MainCamera = Camera.main.transform;

    Input = GetComponent<InputController>();
    // Wheels = GetComponent<WheelController>();

    SwitchState(new Skateboard4MoveState(this));
  }
}