using UnityEngine;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(WheelController))]
[RequireComponent(typeof(Rigidbody))]
public class PhysxSkateboardStateMachine : StateMachine {
  public float PushStrength = 1f;
  public float WheelFriction = 0.5f;
  public float BrakingFriction = 0.6f;
  public float Turning;
  public float Facing;
  public float MaxTruckTurnDeg = 8.34f;
  public float TruckSpacing = 0.205f;
  public float TurnSpeedDamping = 0.3f;
  public float TurnAlignForce = 1f;
  public bool Grounded = true;
  public PhysicMaterial PhysxMat;
  public Animator Board;
  public Transform FrontGroundedChecker, BackGroundedChecker;
  public Transform[] WheelPoints;
  public float GroundedCheckRaycastDistance = 0.05f;
  public float GroundAngleRaycastOffset = 0.5f;
  public float GroundAngleRaycastDistance = 0.65f;
  public Transform SkateboardMeshTransform;
  public Transform MainCamera { get; private set; }
  public InputController Input { get; private set; }
  public Rigidbody PhysxBody { get; private set; }
  public WheelController Wheels { get; private set; }

  private void Start() {
    MainCamera = Camera.main.transform;

    Input = GetComponent<InputController>();
    Wheels = GetComponent<WheelController>();
    PhysxBody = GetComponent<Rigidbody>();

    SwitchState(new PhysxSkateboardMoveState(this));
  }
}