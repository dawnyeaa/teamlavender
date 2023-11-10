using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CharacterPointHandler : MonoBehaviour {
  // different ways to get points
  // 1. airtime
  // 2. tricks
  // 3. grind time
  // 4. ???
  PointManager pointSystem;
  [SerializeField] TextMeshProUGUI groundSpeedDisplay, slowSpeedDisplay, maxSpeedDisplay;
  [SerializeField] Image speedometerDisplay;
  Material speedometerDisplayMat;
  float groundSpeed = 0;
  public float groundSpeedSlowSpeed = 0.1f;
  public float groundSpeedSlowDuration = 1f;
  float slowDurationTimer = 0;
  public Dictionary<string, int> trickPoints;

  public void Start() {
    pointSystem = PointManager.instance;
    speedometerDisplayMat = new(speedometerDisplay.material);
    speedometerDisplay.material = speedometerDisplayMat;
    if (slowSpeedDisplay != null)
      slowSpeedDisplay.text = groundSpeedSlowSpeed.ToString("F");
    if (speedometerDisplayMat)
      speedometerDisplayMat.SetFloat("_slowSpeedThreshold", groundSpeedSlowSpeed);
  }

  public void CompleteTrick(string trickName) {
    // depending on the trick, add points corresponding to that trick
    if (trickPoints == null || !trickPoints.ContainsKey(trickName)) return;
    pointSystem.AddPoints(trickPoints[trickName]);
  }

  public void ValidateTricks() {
    pointSystem.Validate();
  }

  public void CompleteAndValidateTrick(string trickName) {
    CompleteTrick(trickName);
    ValidateTricks();
  }

  public void Die() {
    pointSystem.EndLine();
  }

  public void SetMaxSpeed(float maxSpeed) {
    if (maxSpeedDisplay != null)
      maxSpeedDisplay.text = maxSpeed.ToString("F");
    if (speedometerDisplayMat)
      speedometerDisplayMat.SetFloat("_maxSpeed", maxSpeed);
  }

  public void SetSpeed(float speed) {
    groundSpeed = speed;
    if (groundSpeedDisplay != null)
      groundSpeedDisplay.text = groundSpeed.ToString("F");
    if (speedometerDisplayMat)
      speedometerDisplayMat.SetFloat("_currentSpeed", speed);
    if (groundSpeed < groundSpeedSlowSpeed) {
      if (slowDurationTimer < groundSpeedSlowDuration) {
        slowDurationTimer += Time.deltaTime;
      }
      else {
        slowDurationTimer = 0;
        pointSystem.EndLine();
      }
    }
    else {
      slowDurationTimer = 0;
    }
  }
}
