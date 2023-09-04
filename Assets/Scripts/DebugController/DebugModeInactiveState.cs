using UnityEngine;
public class DebugModeInactiveState : DebugModeBaseState {
  public DebugModeInactiveState(DebugModeStateMachine stateMachine) : base(stateMachine) {}
  public override void Enter() {
    sm.Input.OnDisable();
  }

  public override void Tick() {
  }

  public override void Exit() {
    sm.Input.OnEnable();
  }
}