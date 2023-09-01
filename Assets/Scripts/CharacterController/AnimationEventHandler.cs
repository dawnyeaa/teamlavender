using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {
  public SkateboardStateMachine sm;
  public float framesPerSecond = 30;
  public void OnOllie() {
    sm.OnOllie();
  }

  public void OnKickflip() {
    sm.OnKickflipForce();
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
