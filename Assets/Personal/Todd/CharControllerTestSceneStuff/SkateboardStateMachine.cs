using UnityEngine;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(WheelController))]
public class SkateboardStateMachine : StateMachine {
  public float CurrentSpeed;
  public float Facing;
  public float Turning;
  public float FallingSpeed;
  public float MaxSpeed = 3.2f;
  public float PushAccel = 1f;
  public float PushDuration = 0.5f;
  public float PushCooldown = 0.7f;
  public float Drag = 0.2f;
  public float Mass = 2f;
  public float BodyMass = 75f;
  public float TruckSpacing = 0.205f;
  public float MaxTruckTurnDeg = 8.34f;
  public float WheelRadius = 0.03f;
  public AnimationCurve PushAccelCurve;
  public float LookRotationDampFactor = 10f;
  public float TurnSpeedDamping = 0.3f;
  public float TurnSlowdown = 1f;
  public float DecelTime = 0f;
  public bool Decelerating = false;
  public float PushTime = 0f;
  public bool Pushing = false;
  public AnimationCurve PushSpeedFalloff;
  //public float ExcessPushStrength = 0.1f;
  public float DragMultiplier = 1.0f;
  public float CoastSpeed;
  public float ProjectOffset = 0.75f;
  public float ProjectRadius = 0.72f;
  public float GradientProjectOffset = 0.01f;
  public Vector3 BodyAcceleration;
  public Vector3 BodyVelocity;
  public Transform BodyPosition;
  // public Transform bALL;
  public bool shovey = false;
  public Transform gROUNDY;
  public Transform FrontGradientTrack, BackGradientTrack;
  public Transform FrontGradientOrigin, BackGradientOrigin;
  public Animator Animator;
  public Transform SkateboardMeshTransform;
  public Transform MainCamera { get; private set; }
  public InputController Input { get; private set; }
  public WheelController Wheels { get; private set; }

  private void Start() {
    MainCamera = Camera.main.transform;

    Input = GetComponent<InputController>();
    Wheels = GetComponent<WheelController>();
    BodyPosition.position = transform.position + ProjectOffset * Vector3.up;

    SwitchState(new SkateboardMoveState(this));
  }
}