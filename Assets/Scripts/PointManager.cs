using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointManager : MonoBehaviour {
  public static PointManager instance;
  public TextMeshProUGUI pendingPointsDisplay, newPointsDisplay, totalPointsDisplay;
  public TextMeshProUGUI newPointsMessageDisplay;
  public TextMeshProUGUI pendingPointsDebugDisplay, newPointsDebugDisplay, currentLinePointsDebugDisplay, totalPointsDebugDisplay;
  public TextMeshProUGUI pendingPointsDebugTimerDisplay;

  public GameObject pointsDebugContainer;

  public float pendingPointsTimeout = 20f;
  public float newPointsDisplayTimeout = 10f;

  public string[] newPointsTexts = {"sick air (:<", "woah nice one >:O", "nailed it!!!!!!!"};
  public Color[] newPointsColors;

  [ReadOnly] public int pendingPoints;
  [ReadOnly] public int newPoints;
  [ReadOnly] public int currentLinePoints;
  [ReadOnly] public int totalPoints;

  [ReadOnly] public float pointHeat;
  
  [ReadOnly] public bool pendingNewToggle = false;
  [ReadOnly] public bool showPoints = false;
  
  [ReadOnly] public float pendingPointsTimer;
  [ReadOnly] public float newPointsDisplayTimer;

  void Awake() {
    instance ??= this;
  }
  public void Start() {
    pendingPoints = 0;
    newPoints = 0;
    currentLinePoints = 0;
    totalPoints = 0;

    InputController.instance.OnShowDebugPointsPerformed += ToggleDebugPointsDisplay;
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
    currentLinePoints += newPoints;
    totalPoints += newPoints;
    TrashPending();
    showPoints = true;
    pendingNewToggle = true;
    pendingPointsTimer = 0;
    newPointsDisplayTimer = newPointsDisplayTimeout;
    Display();
  }

  public void EndLine() {
    currentLinePoints = 0;
    TrashPending();
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
    DisplayDebugPoints(pendingPointsDebugDisplay, pendingPoints);
    DisplayDebugPoints(newPointsDebugDisplay, newPoints);
    DisplayDebugPoints(currentLinePointsDebugDisplay, currentLinePoints);
    DisplayDebugPoints(totalPointsDebugDisplay, totalPoints);
  }

  private void DisplayDebugPoints(TextMeshProUGUI textMesh, int points) {
    string displayText = textMesh.text;
    if (displayText[^1] != ':')
      displayText = displayText.Remove(displayText.IndexOf(':') + 1);
    displayText += ' ' + points.ToString();
    textMesh.text = displayText;
  }

  private void ToggleDebugPointsDisplay() {
    pointsDebugContainer.SetActive(!pointsDebugContainer.activeSelf);
  }
}
