using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Unity.Mathematics;

public class CharacterPointHandler : MonoBehaviour {
  // different ways to get points
  // 1. airtime
  // 2. tricks
  // 3. grind time
  // 4. manny time
  // 5. air turns
  // 6. ???
  PointManager pointSystem;
  [SerializeField] TextMeshProUGUI groundSpeedDisplay, slowSpeedDisplay, maxSpeedDisplay;
  [SerializeField] Image speedometerDisplay;
  private Material speedometerDisplayMat;
  public int pointsPerHalfTurn = 10;
  bool onGround = true;
  float groundSpeed = 0;
  float orientation = 0;
  float cwturnAmount = 0;
  float ccwturnAmount = 0;
  public float groundSpeedSlowSpeed = 0.1f;
  public float groundSpeedSlowDuration = 1f;
  float slowDurationTimer = 0;

  void Start() {
    pointSystem = PointManager.instance;
    speedometerDisplayMat = new(speedometerDisplay.material);
    speedometerDisplay.material = speedometerDisplayMat;
    if (slowSpeedDisplay != null)
      slowSpeedDisplay.text = groundSpeedSlowSpeed.ToString("F");
    if (speedometerDisplayMat)
      speedometerDisplayMat.SetFloat("_slowSpeedThreshold", groundSpeedSlowSpeed);
  }

  void Update() {

  }

  public void CompleteTrick(Combo trick) {
    // depending on the trick, add points corresponding to that trick
    pointSystem.AddPoints(trick._ComboTrickValue);
  }

  public void ValidateTricks() {
    pointSystem.Validate();
  }

  public void CompleteAndValidateTrick(Combo trick) {
    CompleteTrick(trick);
    ValidateTricks();
  }

  public void Die() {
    pointSystem.EndLine();
  }

  private void ResolveRotation() {
    if (cwturnAmount < 0.5f && ccwturnAmount < 0.5f) return;
    if (cwturnAmount > ccwturnAmount) {

    }
    else {

    }
    var halfturns = Mathf.FloorToInt(Mathf.Max(cwturnAmount, ccwturnAmount)*2f);
    pointSystem.AddPoints(halfturns * pointsPerHalfTurn);
  }

  private void Landed() {
    ResolveRotation();
    ValidateTricks();
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

  public void SetGrounded(bool isOnGround) {
    if (!onGround && isOnGround) Landed();
    onGround = isOnGround;
  }

  public void SetOrientation(Vector3 up, Vector3 forward) {
    
  }
}
