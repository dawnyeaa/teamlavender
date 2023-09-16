public class SkateboardRailState : SkateboardBaseState {
  public SkateboardRailState(SkateboardStateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    DisableSpinBody();
    FaceAlongRail();
    InitRailPos();
    ApplyRotationToModels();
    StartRailAnim();
    StartRailBoost();
    
    sm.HeadSensZone.AddCallback(sm.Die);
  }

  public override void Tick() {
    CheckRailValidity();
    KeepOnRail();
    AdjustToTargetRailOffset();
    SetRailPos();
    SetGrindingFriction();
    ApplyFrictionForce();
  }

  public override void Exit() {
    PushOffRail();
    EndRailAnim();
    EnableSpinBody();
    
    sm.HeadSensZone.RemoveCallback(sm.Die);
  }
}