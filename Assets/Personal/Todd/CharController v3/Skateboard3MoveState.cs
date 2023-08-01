using UnityEngine;

public class Skateboard3MoveState : Skateboard3BaseState {
  private readonly int TurnHash = Animator.StringToHash("dir");
  
  public Skateboard3MoveState(Skateboard3StateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    stateMachine.Input.OnPushPerformed += OnPush;
  }

  public override void Tick() {
    SetFriction();
    CalculateTurn();
    // stateMachine.Board.SetFloat(TurnHash, stateMachine.Turning);
  }

  public override void Exit() {
    stateMachine.Input.OnPushPerformed -= OnPush;
  }
}