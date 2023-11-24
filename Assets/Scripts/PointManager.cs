using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LineMessages {
  public string message;
  public int threshold;
}

public class PointManager : MonoBehaviour {
  public static PointManager instance;
  public TextMeshProUGUI pendingPointsDisplay, newPointsDisplay, totalPointsDisplay;
  public TextMeshProUGUI newPointsMessageDisplay;
  public TextMeshProUGUI pendingPointsDebugDisplay, newPointsDebugDisplay, currentLinePointsDebugDisplay, totalPointsDebugDisplay;
  public TextMeshProUGUI pendingPointsDebugTimerDisplay;
  public TextMeshProUGUI lineMessageDisplay, lineTimeDisplay, linePickupDisplay;
  [SerializeField] Renderer lineLineBarImage;

  public GameObject pointsDebugContainer;

  public float pendingPointsTimeout = 20f;
  public float newPointsDisplayTimeout = 10f;

  public string[] newPointsTexts = {"sick air (:<", "woah nice one >:O", "nailed it!!!!!!!"};
  public Color[] newPointsColors;
  public float lineStartValue = 300;
  public float lineStartDecreaseSpeed = 5;
  public float lineAcceleration = 1;
  public float lineCooldown = 5;

  public List<LineMessages> messages;

  [ReadOnly] public float pendingPoints;
  [ReadOnly] public float newPoints;
  [ReadOnly] public float currentLinePoints;
  [ReadOnly] public float totalPoints;
  [ReadOnly] public Material lineLineBarMat;

  [ReadOnly] public float pointHeat;
  
  [ReadOnly] public bool pendingNewToggle = false;
  [ReadOnly] public bool showPoints = false;
  
  [ReadOnly] public float pendingPointsTimer;
  [ReadOnly] public float newPointsDisplayTimer;
  [ReadOnly] public bool inLine = false;
  [ReadOnly] public float lineValue = 0;
  [ReadOnly] public float lineTimer = 0;
  [ReadOnly] public float lineDecreaseSpeed = 0;
  [ReadOnly] public float lineCooldownTimer = 0;
  [ReadOnly] public int linePickups = 0;

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
    UpdateLineValue();
    DrawLineValue();
    lineCooldownTimer -= Time.deltaTime;
    lineTimer += Time.deltaTime;
  }

  public bool IsInLine() {
    return inLine;
  }

  public void AddPoints(float pointsToAdd) {
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
    if (pendingPoints == 0) return;
    newPoints = pendingPoints;
    currentLinePoints += newPoints;
    totalPoints += newPoints;
    if (inLine) lineValue += newPoints;
    TrashPending();
    showPoints = true;
    pendingNewToggle = true;
    pendingPointsTimer = 0;
    newPointsDisplayTimer = newPointsDisplayTimeout;
    Display();
  }

  public void StartLine() {
    if (!inLine && lineCooldownTimer <= 0) {
      inLine = true;
      lineTimer = 0;
      linePickups = 0;
      lineDecreaseSpeed = lineStartDecreaseSpeed;
      lineValue = lineStartValue;
    }
  }

  private void UpdateLineValue() {
    if (inLine) {
      UpdateLineTimeDisplay();
      lineDecreaseSpeed += lineAcceleration * Time.deltaTime;
      lineValue -= lineDecreaseSpeed * Time.deltaTime;
      foreach (var message in messages) {
        if (lineTimer < message.threshold) {
          lineMessageDisplay.text = message.message;
          break;
        }
      }
      if (lineValue <= 0) {
        EndLine();
      }
    }
  }

  public void EndLine() {
    inLine = false;
    lineCooldownTimer = lineCooldown;
    currentLinePoints = 0;
    TrashPending();
    Display();
  }

  private void DrawLineValue() {
    lineLineBarMat = new(lineLineBarImage.material);
    lineLineBarImage.material = lineLineBarMat;
    if (lineLineBarMat)
      lineLineBarMat.SetFloat("_current", lineValue);
  }

  private void UpdateLineTimeDisplay() {
    lineTimeDisplay.text = $"{(int)lineTimer} seconds";
  }

  public void UpdatePickupDisplay() {
    if (inLine) {
      linePickupDisplay.text = $"{++linePickups} pickups";
    }
  }

  private void ResetPickupDisplay() {
    linePickupDisplay.text = $"0 pickups";
  }

  private void Display() {
    DisplayDebugPoints(pendingPointsDebugDisplay, pendingPoints);
    DisplayDebugPoints(newPointsDebugDisplay, newPoints);
    DisplayDebugPoints(currentLinePointsDebugDisplay, currentLinePoints);
    DisplayDebugPoints(totalPointsDebugDisplay, totalPoints);
  }

  private void DisplayDebugPoints(TextMeshProUGUI textMesh, float points) {
    int p = Mathf.RoundToInt(points);
    string displayText = textMesh.text;
    if (displayText[^1] != ':')
      displayText = displayText.Remove(displayText.IndexOf(':') + 1);
    displayText += ' ' + p.ToString();
    textMesh.text = displayText;
  }

  private void ToggleDebugPointsDisplay() {
    pointsDebugContainer.SetActive(!pointsDebugContainer.activeSelf);
  }
}
