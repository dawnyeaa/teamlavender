using UnityEngine;

public class SkateboardMoveState : SkateboardBaseState {
  private readonly int TurnHash = Animator.StringToHash("dir");
  
  public SkateboardMoveState(SkateboardStateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    stateMachine.Input.OnPushPerformed += BeginPush;
  }

  public override void Tick() {
    Accelerate();
    CalculateTurn();
    Brake();
    Move();
    SpinWheels();
    ApplyGravity();
    MatchGround();
    BodyRotationDamp();

    stateMachine.SpringBodController.squatT = Mathf.Pow(Mathf.InverseLerp(0, stateMachine.MaxSpeed, stateMachine.CurrentSpeed), 8f);

    stateMachine.Animator.SetFloat(TurnHash, stateMachine.Turning);
  }

  public override void Exit() {
    stateMachine.Input.OnPushPerformed -= BeginPush;
  }
}