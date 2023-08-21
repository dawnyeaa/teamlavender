using UnityEngine;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(WheelController))]
public class Skateboard3StateMachine : StateMachine {
  // User Constants - Runtime only
  // [Header("Constants - Only read at runtime")]
  // User Constants - Live update
  [Header("Constants - Live update")]
  public float PushForce = 7000f;
  public float WheelFriction = 0.01f;
  public float BrakingFriction = 0.4f;
  public float MaxTruckTurnDeg = 8.34f;
  public float TruckSpacing = 0.205f;
  public float TruckMass = 7.5f;
  public float TruckTurnDamping = 0.3f;
  public AnimationCurve TurningEase;
  [Range(0, 1)] public float TruckGripFactor = 0.8f;
  // Internal State Processing
  [Header("Internal State")]
  [ReadOnly] public bool FacingForward = true;
  [ReadOnly] public bool Grounded = true;
  [ReadOnly] public float TruckTurnPercent;
  // Objects to link
  [Header("Link Slot Objects")]
  public PhysicMaterial PhysMat;
  public Rigidbody BoardRb;
  public Transform frontAxis, backAxis;
  public Transform MainCamera { get; private set; }
  public InputController Input { get; private set; }
  public WheelController Wheels { get; private set; }

  [HideInInspector] public Transform ball1, ball2, ball3;

  private void Start() {
    MainCamera = Camera.main.transform;

    Input = GetComponent<InputController>();
    Wheels = GetComponent<WheelController>();

    SwitchState(new Skateboard3MoveState(this));
  }
}