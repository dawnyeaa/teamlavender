using UnityEngine;
public class SkateboardPauseState : SkateboardBaseState {
  public SkateboardPauseState(SkateboardStateMachine stateMachine) : base(stateMachine) {}
  public override void Enter() {
    sm.Input.OnDisable();
    sm.SFX.isPaused = true;
  }

  public override void Tick() {
  }

  public override void Exit() {
    sm.Input.OnEnable();
    sm.SFX.isPaused = false;
  }
}