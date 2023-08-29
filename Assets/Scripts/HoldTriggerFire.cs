using System;
using UnityEngine;
using System.Collections.Generic;

public class HoldTriggerFire : MonoBehaviour {
  private Action actionsToFire;
  private static readonly List<(float, float)> thresholds = new() {
    (0.3f, 5),
    (0.6f, 10),
    (0.9f, 20),
    (1.2f, 40),
  };
  public bool active = false;

  public float currentDuration = 0;
  public float firesPerSecond = 0;

  public float timeSinceFire = 0;

  private void OnAwake() {
    actionsToFire = null;
  }

  private void Update() {
    if (active) {
      currentDuration += Time.unscaledDeltaTime;
      timeSinceFire += Time.unscaledDeltaTime;

      foreach ((float, float) threshold in thresholds) {
        if (currentDuration >= threshold.Item1) {
          firesPerSecond = threshold.Item2;
        }
      }

      if (timeSinceFire > (1/firesPerSecond)) {
        fireAction();
        timeSinceFire -= (1/firesPerSecond);
      }
    }
  }

  private void OnDisable() {
    active = false;
  }

  public void setup(Action toFire) {
    actionsToFire += toFire;
    active = false;
  }

  public void clearDel(Action toFire) {
    actionsToFire -= toFire;
  }

  private void setAccumulator(bool active) {
    this.active = active;
  }

  private void resetAccumulator(bool active = true) {
    this.active = active;
    currentDuration = 0;
    timeSinceFire = 0;
  }

  public void startHold() {
    currentDuration = 0;
    timeSinceFire = 0;
    firesPerSecond = 0;
    active = true;
  }

  public void endHold() {
    active = false;
  }

  public void fireAction() {
    actionsToFire?.Invoke();
  }
}