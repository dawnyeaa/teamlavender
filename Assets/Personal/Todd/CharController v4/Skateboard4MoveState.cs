using UnityEngine;

using UnityEditor;

public class Skateboard4MoveState : Skateboard4BaseState {
  private readonly int TurnHash = Animator.StringToHash("dir");

  public Skateboard4MoveState(Skateboard4StateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    sm.Input.OnPushPerformed += StartPush;
  }

  public override void Tick() {
    BodyUprightCorrect();
    AdjustSpringMultiplier();
    SetFriction();
    VertBodySpring();
    CalculateTurn();
    CalculatePush();
    ApplyFrictionForce();
    // if (Vector3.Dot(Vector3.up, -sm.Down) < (1-Mathf.Epsilon)) {
    //   EditorApplication.isPaused = true;
    // }
    // sm.Board.SetFloat(TurnHash, sm.Turning);
  }

  public override void Exit() {
    sm.Input.OnPushPerformed -= StartPush;
  }
}