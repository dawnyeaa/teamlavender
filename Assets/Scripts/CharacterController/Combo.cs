using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewCombo", menuName = "ScriptableObjects/Combo", order = 1)]
public class Combo : ScriptableObject {
  public string _ComboName;
  public List<Input> _ComboInputs;
  public int _OllieTrickIndex;
  public UnityEvent _ExtraComboEvents;
  // public int _TrickValue;

  private SkateboardStateMachine sm;

  public void ExecuteCombo() {
    sm = FindObjectOfType<SkateboardStateMachine>();
    sm.CurrentOllieTrickIndex = _OllieTrickIndex;
    sm.OnCombo(_ComboName);
    _ExtraComboEvents?.Invoke();
  }

}
