using UnityEngine;

using UnityEditor;

public class SkateboardMoveState : SkateboardBaseState {
  public SkateboardMoveState(SkateboardStateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    sm.Input.OnPushPerformed += StartPush;
    sm.Input.OnSwitchPerformed += OnSwitch;
    sm.Input.OnOlliePerformed += OnOllieInput;
    sm.Input.OnStartBraking += StartBrake;
    sm.Input.OnEndBraking += EndBrake;
    sm.ComboActions["ollie"] += OnOllieInput;
    sm.ComboActions["kickflip"] += OnKickflipInput;
  }

  public override void Tick() {
    CreateDebugFrame();

    AdjustSpringMultiplier();
    SetMovingFriction();
    SetCrouching();
    VertBodySpring();
    CalculateTurn();
    ApplyRotationToModels();
    CalculatePush();
    CheckRails();
    CheckWalls();
    BodyUprightCorrect();
    ApplyFrictionForce();

    SaveDebugFrame();
  }

  public override void Exit() {
    sm.Input.OnPushPerformed -= StartPush;
    sm.Input.OnSwitchPerformed -= OnSwitch;
    sm.Input.OnOlliePerformed -= OnOllieInput;
    sm.Input.OnStartBraking -= StartBrake;
    sm.Input.OnEndBraking -= EndBrake;
    sm.ComboActions["ollie"] -= OnOllieInput;
    sm.ComboActions["kickflip"] -= OnKickflipInput;
  }
}