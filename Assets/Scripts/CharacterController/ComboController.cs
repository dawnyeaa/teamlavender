using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComboController : MonoBehaviour {
  public Queue<Input> comboBuffer;
  [SerializeField] List<Input> queueVis;
  public List<Combo> comboList;
  public const int capacity = 5;
  public ComboController() {
    comboBuffer = new Queue<Input>(capacity);
  }

  public bool AddToBuffer(Input input) {
    bool isFull = comboBuffer.Count >= capacity;
    if (isFull) comboBuffer.Dequeue();
    comboBuffer.Enqueue(input);
    queueVis = comboBuffer.ToList();
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
}