using UnityEngine;

using UnityEditor;

public class SkateboardMoveState : SkateboardBaseState {
  private readonly int TurnHash = Animator.StringToHash("dir");

  public SkateboardMoveState(SkateboardStateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    sm.Input.OnPushPerformed += StartPush;
    sm.Input.OnSwitchPerformed += OnSwitch;
    sm.Input.OnOlliePerformed += OnOllieInput;
    sm.Input.OnStartBraking += StartBrake;
    sm.Input.OnEndBraking += EndBrake;
  }

  public override void Tick() {
    CreateDebugFrame();

    AdjustSpringMultiplier();
    SetFriction();
    SetCrouching();
    VertBodySpring();
    CalculateTurn();
    CalculatePush();
    BodyUprightCorrect();
    ApplyFrictionForce();

    SaveDebugFrame();
    // sm.Board.SetFloat(TurnHash, sm.Turning);
  }

  public override void Exit() {
    sm.Input.OnPushPerformed -= StartPush;
    sm.Input.OnSwitchPerformed -= OnSwitch;
    sm.Input.OnOlliePerformed -= OnOllieInput;
    sm.Input.OnStartBraking -= StartBrake;
    sm.Input.OnEndBraking -= EndBrake;
  }
}