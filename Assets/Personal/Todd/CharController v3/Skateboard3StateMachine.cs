using UnityEngine;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(WheelController))]
public class Skateboard3StateMachine : StateMachine {
  public float PushForce = 1000f;
  public float WheelFriction = 0.5f;
  public float BrakingFriction = 0.6f;
  public bool FacingForward = true;
  public bool Grounded = true;
  public float MaxTruckTurnDeg = 8.34f;
  public float TruckSpacing = 0.205f;
  public float TruckMass = 7.5f;
  public float TruckTurnDamping = 0.3f;
  public float TruckTurnPercent;
  [Range(0, 1)] public float TruckGripFactor = 0.8f;
  public PhysicMaterial PhysMat;
  public Rigidbody BoardRb;
  public Transform frontAxis, backAxis;
  public Transform MainCamera { get; private set; }
  public InputController Input { get; private set; }
  public WheelController Wheels { get; private set; }

  private void Start() {
    MainCamera = Camera.main.transform;

    Input = GetComponent<InputController>();
    Wheels = GetComponent<WheelController>();

    SwitchState(new Skateboard3MoveState(this));
  }
}