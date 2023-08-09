using UnityEngine;

using UnityEditor;

public class Skateboard4MoveState : Skateboard4BaseState {
  private readonly int TurnHash = Animator.StringToHash("dir");

  public Skateboard4MoveState(Skateboard4StateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    sm.Input.OnPushPerformed += StartPush;
    sm.Input.OnSwitchPerformed += OnSwitch;
    sm.Input.OnOlliePerformed += OnOllie;
  }

  public override void Tick() {
    AdjustSpringMultiplier();
    SetFriction();
    VertBodySpring();
    CalculateTurn();
    CalculatePush();
    BodyUprightCorrect();
    ApplyFrictionForce();
    // sm.Board.SetFloat(TurnHash, sm.Turning);
  }

  public override void Exit() {
    sm.Input.OnPushPerformed -= StartPush;
    sm.Input.OnSwitchPerformed -= OnSwitch;
    sm.Input.OnOlliePerformed -= OnOllie;
  }
}