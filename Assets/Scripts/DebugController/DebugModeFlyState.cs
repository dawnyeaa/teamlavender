using UnityEngine;
public class DebugModeFlyState : DebugModeBaseState {
  public DebugModeFlyState(DebugModeStateMachine stateMachine) : base(stateMachine) {}
  public override void Enter() {
    sm.VirtCam.Priority = 3;
    sm.DebugFrameHandler.ShowTraceLines(true);
    sm.Input.SetDelegate(0, SelectPrevFrame);
    sm.Input.SetDelegate(1, SelectNextFrame);
    sm.Input.SetDelegate(2, IncreaseFrameWindow);
    sm.Input.SetDelegate(3, DecreaseFrameWindow);
  }

  public override void Tick() {
    MoveCharacter();
    DisplayDebugInfo();
  }

  public override void Exit() {
    sm.VirtCam.Priority = 1;
    sm.DebugFrameHandler.ShowTraceLines(false);
    sm.Input.RemoveDelegate(0, SelectPrevFrame);
    sm.Input.RemoveDelegate(1, SelectNextFrame);
    sm.Input.RemoveDelegate(2, IncreaseFrameWindow);
    sm.Input.RemoveDelegate(3, DecreaseFrameWindow);
  }
}