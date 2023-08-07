using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComboController : MonoBehaviour {
  public Queue<Input> comboBuffer;
  public const int capacity = 5;
  public Dictionary<string, Input[]> combos;
  public Dictionary<string, Action> comboCallbacks;
  public ComboController() {
    comboBuffer = new Queue<Input>(capacity);
    combos = new Dictionary<string, Input[]>();
    comboCallbacks = new Dictionary<string, Action>();
    combos.Add("ollie", new Input[] {Input.rs5, Input.rs2, Input.rs5, Input.rs8});
  }

  public bool AddToBuffer(Input input) {
    bool isFull = comboBuffer.Count >= capacity;
    comboBuffer.Dequeue();
    comboBuffer.Enqueue(input);
    return isFull;
  }

  public void ClearBuffer() {
    comboBuffer.Clear();
  }

  public void CheckCombos() {
    foreach (string combo in combos.Keys) {
      int comboInputI = 0;
      List<Input> buffer = comboBuffer.ToList<Input>();
      for (int i = 0; i < capacity; ++i) {
        if (buffer[i+comboInputI] == combos[combo][comboInputI]) {
          comboInputI++;
          if (comboInputI >= combos[combo].Count()) {
            // we've got a combo!
            // gotta call the callback functions for this specific combo
            comboCallbacks[combo]?.Invoke();
          }
        }
        else 
          comboInputI = 0;
      }
    }
  }
}