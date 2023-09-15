using UnityEngine;

using UnityEditor;

public class SkateboardMoveState : SkateboardBaseState {
  public SkateboardMoveState(SkateboardStateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    sm.Input.OnPushPerformed += StartPush;
    sm.Input.OnSwitchPerformed += OnSwitch;
    sm.Input.OnPausePerformed += sm.Pause;
    sm.Input.OnStartBraking += StartBrake;
    sm.Input.OnEndBraking += EndBrake;
    sm.ComboActions["ollie"] += OnOllieTrickInput;
    sm.ComboActions["kickflip"] += OnOllieTrickInput;
    sm.ComboActions["heelflip"] += OnOllieTrickInput;
    sm.ComboActions["popShuvit"] += OnOllieTrickInput;
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
    CapSpeed();
    CheckRails();
    CheckWalls();
    BodyUprightCorrect();
    ApplyFrictionForce();
    SetHipHelperPos();

    SaveDebugFrame();
  }

  public override void Exit() {
    sm.Input.OnPushPerformed -= StartPush;
    sm.Input.OnSwitchPerformed -= OnSwitch;
    sm.Input.OnPausePerformed -= sm.Pause;
    sm.Input.OnStartBraking -= StartBrake;
    sm.Input.OnEndBraking -= EndBrake;
    sm.ComboActions["ollie"] -= OnOllieTrickInput;
    sm.ComboActions["kickflip"] -= OnOllieTrickInput;
    sm.ComboActions["heelflip"] -= OnOllieTrickInput;
    sm.ComboActions["popShuvit"] -= OnOllieTrickInput;
  }
}