using UnityEngine;
public class DebugModeFlyState : DebugModeBaseState {
  public DebugModeFlyState(DebugModeStateMachine stateMachine) : base(stateMachine) {}
  public override void Enter() {
    sm.VirtCam.Priority = 3;
  }

  public override void Tick() {
    MoveCharacter();
  }

  public override void Exit() {
    sm.VirtCam.Priority = 1;
  }
}