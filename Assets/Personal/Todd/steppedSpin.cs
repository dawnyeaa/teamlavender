using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class steppedSpin : MonoBehaviour {
  public float spinSpeed = 1f;
  public float angle = 0f;
  public ContinuousDataStepper angleStepper;
  public float fps = 24;
  void Start() {
    angleStepper = new ContinuousDataStepper(angle, fps);
  }

  void OnValidate() {
    angleStepper?.SetStepRate(fps);
  }

  void Update() {
    if (Time.timeScale > 0) {
      angle = angleStepper.TickDelta(spinSpeed, Time.deltaTime);
      transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
  }
}
