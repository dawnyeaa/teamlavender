using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ComboController : MonoBehaviour {
  public float maxTimeBetweenInputs = 0.1f;
  public Queue<Input> comboBuffer;
  public float timeSinceInput = 0;
  [ReadOnly] [SerializeField] List<Input> queueVis;
  public List<Combo> comboList;
  public const int capacity = 5;
  public TMPro.TextMeshProUGUI comboNameDisplay;
  public Combo currentlyPlayingCombo { get; private set; }
  public float comboNameDisplayTime = 5f;
  [ReadOnly, SerializeField] private float comboDisplayTimer = 0;
  public ComboController() {
    comboBuffer = new Queue<Input>(capacity);
  }

  void FixedUpdate() {
    timeSinceInput += Time.fixedDeltaTime;
    // if (timeSinceInput > maxTimeBetweenInputs) {
    //   ClearBuffer();
    // }
  }

  void Update() {
    if (comboDisplayTimer >= comboNameDisplayTime)
      ResetComboDisplay();
    else
      comboDisplayTimer += Time.deltaTime;
  }

  public bool AddToBuffer(Input input) {
    bool isFull = comboBuffer.Count >= capacity;
    if (isFull) comboBuffer.Dequeue();
    comboBuffer.Enqueue(input);
    queueVis = comboBuffer.ToList();
    timeSinceInput = 0;
    CheckCombos();
    return isFull;
  }

  public void ClearBuffer() {
    comboBuffer.Clear();
    queueVis = comboBuffer.ToList();
  }

  public void CheckCombos() {
    // reverse the buffer so we can check inputs with the most recent first
    List<Input> reversedbuffer = comboBuffer.ToList();
    reversedbuffer.Reverse();

    foreach (Combo combo in comboList) {
      List<Input> comboInput = new(combo._ComboInputs);
      comboInput.Reverse();
      int bufferI = 0, comboI = 0;
      while (bufferI+(comboInput.Count-comboI) <= reversedbuffer.Count) {
        if ((int)reversedbuffer[bufferI] == (int)comboInput[comboI]) {
          comboI++;
          if (comboI >= comboInput.Count) {
            combo.ExecuteCombo();
            currentlyPlayingCombo = combo;
            ClearBuffer();
            return;
          }
        }
        else if (comboI > 0) {
          break;
        }
        bufferI++;
      }
    }
  }

  public void SetComboDisplay() {
    if (comboNameDisplay != null && currentlyPlayingCombo != null) {
      var displayName = currentlyPlayingCombo._ComboDisplayName;
      comboNameDisplay.text = displayName;
      if (displayName.Length > 0)
        comboDisplayTimer = 0;
    }
  }

  public void ResetComboDisplay() {
    if (comboNameDisplay != null) {
      comboNameDisplay.text = "";
    }
  }
}