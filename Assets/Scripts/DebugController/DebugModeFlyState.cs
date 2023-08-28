using UnityEngine;
public class DebugModeFlyState : DebugModeBaseState {
  public DebugModeFlyState(DebugModeStateMachine stateMachine) : base(stateMachine) {}
  public override void Enter() {
    sm.VirtCam.Priority = 3;
    sm.DebugFrameHandler.ShowTraceLines(true);
    sm.Input.OnStepNextPerformed += SelectNextFrame;
    sm.Input.OnStepPrevPerformed += SelectPrevFrame;
    sm.Input.OnIncFrameWindowPerformed += IncreaseFrameWindow;
    sm.Input.OnDecFrameWindowPerformed += DecreaseFrameWindow;
  }

  public override void Tick() {
    MoveCharacter();
    DisplayDebugInfo();
  }

  public override void Exit() {
    sm.VirtCam.Priority = 1;
    sm.DebugFrameHandler.ShowTraceLines(false);
    sm.Input.OnStepNextPerformed -= SelectNextFrame;
    sm.Input.OnStepPrevPerformed -= SelectPrevFrame;
    sm.Input.OnIncFrameWindowPerformed -= IncreaseFrameWindow;
    sm.Input.OnDecFrameWindowPerformed -= DecreaseFrameWindow;
  }
}