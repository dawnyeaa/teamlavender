using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TrickAnimationGroup {
  Ollie,
  Nollie
}

[CreateAssetMenu(fileName = "NewCombo", menuName = "ScriptableObjects/Combo", order = 1)]
public class Combo : ScriptableObject {
  public string _ComboName;
  public string _ComboDisplayName;
  public List<Input> _ComboInputs;
  public float _HopVerticalForceMultiplier = 1;
  public float _HopHorizontalForceMultiplier = 1;
  public TrickAnimationGroup _TrickAnimGroup;
  public int _TrickAnimIndex;
  public UnityEvent _ExtraComboEvents;
  // public int _TrickValue;

  private SkateboardStateMachine sm;

  public void ExecuteCombo() {
    sm = FindObjectOfType<SkateboardStateMachine>();
    sm.CurrentAnimTrickIndexes[(int)_TrickAnimGroup] = _TrickAnimIndex;
    sm.OnCombo(_ComboName, (int)_TrickAnimGroup, _HopVerticalForceMultiplier, _HopHorizontalForceMultiplier);
    _ExtraComboEvents?.Invoke();
  }

}
