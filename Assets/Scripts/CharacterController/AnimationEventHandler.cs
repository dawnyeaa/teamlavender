using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {
  public SkateboardStateMachine sm;
  public CoolTimeController coolTime;
  public float coolTimeAirDurationThreshold = 1.5f;
  public float framesPerSecond = 30;
  public void OnOllieForce() {
    sm.OnOllieForce();
  }

  public void CoolTime() {
    if (sm.TimeToLand > coolTimeAirDurationThreshold)
      coolTime.StartCoolTime();
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
}
