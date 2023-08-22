using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {
  public SkateboardStateMachine sm;
  public void OnOllie() {
    sm.OnOllie();
  }
}
