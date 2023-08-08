using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointManager : MonoBehaviour {
  public TextMeshProUGUI pendingPointsDisplay, newPointsDisplay, totalPointsDisplay;
  public TextMeshProUGUI newPointsMessageDisplay;

  public float pendingPointsTimeout = 20f;
  public float newPointsDisplayTimeout = 10f;

  public string[] newPointsTexts = {"sick air (:<", "woah nice one >:O", "nailed it!!!!!!!"};
  public Color[] newPointsColors;

  [ReadOnly] public int pendingPoints;
  [ReadOnly] public int newPoints;
  [ReadOnly] public int totalPoints;
  
  [ReadOnly] public bool pendingNewToggle = false;
  [ReadOnly] public bool showPoints = false;
  
  [ReadOnly] public float pendingPointsTimer;
  [ReadOnly] public float newPointsDisplayTimer;

  // public int TestPointsToAdd = 0;

  // public bool TestAddPoints = false;
  // public bool TestTrashPending = false;
  // public bool TestValidate = false;

  public void Start() {
    pendingPoints = 0;
    newPoints = 0;
    totalPoints = 0;
  }

  public void Update() {
    if (pendingPointsTimer > 0) {
      pendingPointsTimer -= Time.deltaTime;
      if (pendingPointsTimer <= 0)
        TrashPending();
    }
    if (newPointsDisplayTimer > 0) {
      newPointsDisplayTimer -= Time.deltaTime;
      if (newPointsDisplayTimer <= 0) {
        showPoints = false;
        Display();
      }
    }

    // if (TestAddPoints) {
    //   TestAddPoints = false;
    //   AddPoints(TestPointsToAdd);
    // }
    // if (TestTrashPending) {
    //   TestTrashPending = false;
    //   TrashPending();
    // }
    // if (TestValidate) {
    //   TestValidate = false;
    //   Validate();
    // }
  }

  public void AddPoints(int pointsToAdd) {
    pendingPoints += pointsToAdd;
    pendingNewToggle = false;
    showPoints = true;
    pendingPointsTimer = pendingPointsTimeout;
    newPointsDisplayTimer = 0;
    Display();
  }

  public void TrashPending() {
    pendingPoints = 0;
    if (!pendingNewToggle)
      showPoints = false;
    Display();
  }

  public void Validate() {
    newPoints = pendingPoints;
    totalPoints += newPoints;
    TrashPending();
    showPoints = true;
    pendingNewToggle = true;
    pendingPointsTimer = 0;
    newPointsDisplayTimer = newPointsDisplayTimeout;
    Display();
  }

  private void Display() {
    totalPointsDisplay.text = totalPoints.ToString();
    if (showPoints) {
      if (pendingNewToggle) {
        pendingPointsDisplay.text = "";
        newPointsDisplay.text = newPoints.ToString();
        Color color = newPointsColors[Random.Range(0, newPointsColors.Length)];
        color = new Color(color.r, color.g, color.b);
        newPointsMessageDisplay.text = newPointsTexts[Random.Range(0, newPointsTexts.Length)];
        newPointsMessageDisplay.color = color;
      }
      else {
        newPointsDisplay.text = "";
        newPointsMessageDisplay.text = "";
        pendingPointsDisplay.text = pendingPoints.ToString();
      }
    }
    else {
      pendingPointsDisplay.text = "";
      newPointsDisplay.text = "";
      newPointsMessageDisplay.text = "";
    }
  }
}
