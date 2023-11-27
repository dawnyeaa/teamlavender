using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {
  public SkateboardStateMachine sm;
  public CoolTimeController coolTime;
  public ComboController comboController;
  public ComboDisplayManager comboDisplayManager;
  public float framesPerSecond = 30;
  public List<string> nonCoolTimeTricks = new() {
    "ollie",
    "nollie"
  };
  public void OnOllieForce() {
    sm.OnOllieForce();
  }

  public void CoolTime() {
    if (comboController.currentlyPlayingCombo != null && !nonCoolTimeTricks.Contains(comboController.currentlyPlayingCombo._ComboName))
      coolTime.StartCoolTime(sm.TimeToLand);
  }

  public void PushForce(int pushFrames) {
    sm.StartPushForce(pushFrames/framesPerSecond);
  }

  public void PushingEnd() {
    sm.PushingEnd();
  }

  public void BrakeForce() {
    sm.BrakeForce();
  }

  public void DisplayCombo() {
    comboDisplayManager.SetComboDisplay();
  }

  public void ComboStarted() {
    CoolTime();
    DisplayCombo();
    sm.OnTrickPeak();
    sm.HideTuteTrickHint();
  }
}
