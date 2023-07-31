using UnityEngine;

public class PhysxSkateboardMoveState : PhysxSkateboardBaseState {
  private readonly int TurnHash = Animator.StringToHash("dir");
  
  public PhysxSkateboardMoveState(PhysxSkateboardStateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    stateMachine.Input.OnPushPerformed += BeginPush;
  }

  public override void Tick() {
    CheckGrounded();
    MatchGround();
    SetFriction();
    CalculateTurn();
    AddTurningFriction();
    stateMachine.Board.SetFloat(TurnHash, stateMachine.Turning);
  }

  public override void Exit() {
    stateMachine.Input.OnPushPerformed -= BeginPush;
  }
}