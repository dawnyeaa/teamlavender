using UnityEngine;

public class SkateboardMoveState : SkateboardBaseState {
  private readonly int TurnHash = Animator.StringToHash("dir");
  
  public SkateboardMoveState(SkateboardStateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    stateMachine.Input.OnPushPerformed += BeginPush;
  }

  public override void Tick() {
    ResetBodyAccel();
    Accelerate();
    CalculateTurn();
    Move();
    SpinWheels();
    ApplyGravity();
    MatchGround();
    BodyPhysicsUpdate();

    if (stateMachine.shovey) {
      Shove();
      stateMachine.shovey = false;
    }
    stateMachine.Animator.SetFloat(TurnHash, stateMachine.Turning);
  }

  public override void Exit() {
    stateMachine.Input.OnPushPerformed -= BeginPush;
  }
}