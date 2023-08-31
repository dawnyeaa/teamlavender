using System;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewCombo", menuName = "ScriptableObjects/Combo", order = 1)]
public class Combo : ScriptableObject {
  public string _ComboName;
  public List<Input> _ComboInputs;
  public UnityEvent _ExtraComboEvents;

  public void ExecuteCombo() {
    FindObjectOfType<SkateboardStateMachine>().OnCombo(_ComboName);
    _ExtraComboEvents?.Invoke();
  }

}
