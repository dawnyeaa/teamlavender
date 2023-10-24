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
    sm.ComboActions["ollie"] += OnHopTrickInput;
    sm.ComboActions["kickflip"] += OnHopTrickInput;
    sm.ComboActions["heelflip"] += OnHopTrickInput;
    sm.ComboActions["popShuvit"] += OnHopTrickInput;

    sm.HeadSensZone.AddCallback(sm.Die);

    StartRollingSFX();
  }

  public override void Tick() {
    CreateDebugFrame();

    AdjustSpringMultiplier();
    SetMovingFriction();
    SetCrouching();
    VertBodySpring();
    CalculateTurn();
    CalculatePush();
    CalculateAirTurn();
    CapSpeed();
    CheckRails();
    CheckWalls();
    BodyUprightCorrect();
    ApplyFrictionForce();
    SetHipHelperPos();
    SetWheelSpinParticleChance();
    SetSpeedyLines();
    SetRollingVolume();
    PassGroundSpeedToPointSystem();
    PassSpeedToMotionBlur();
    RollIdleAnimation();

    SaveDebugFrame();
  }

  public override void Exit() {
    sm.Input.OnPushPerformed -= StartPush;
    sm.Input.OnSwitchPerformed -= OnSwitch;
    sm.Input.OnPausePerformed -= sm.Pause;
    sm.Input.OnStartBraking -= StartBrake;
    sm.Input.OnEndBraking -= EndBrake;
    sm.ComboActions["ollie"] -= OnHopTrickInput;
    sm.ComboActions["kickflip"] -= OnHopTrickInput;
    sm.ComboActions["heelflip"] -= OnHopTrickInput;
    sm.ComboActions["popShuvit"] -= OnHopTrickInput;
    
    sm.HeadSensZone.RemoveCallback(sm.Die);

    StopRollingSFX();
  }
}