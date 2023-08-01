using UnityEngine;

public class Skateboard4MoveState : Skateboard4BaseState {
  private readonly int TurnHash = Animator.StringToHash("dir");

  public Skateboard4MoveState(Skateboard4StateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    sm.Input.OnPushPerformed += OnPush;
  }

  public override void Tick() {
    VertBodySpring();
    SetFriction();
    ApplyFrictionForce();
    // sm.Board.SetFloat(TurnHash, sm.Turning);
  }

  public override void Exit() {
    sm.Input.OnPushPerformed -= OnPush;
  }
}