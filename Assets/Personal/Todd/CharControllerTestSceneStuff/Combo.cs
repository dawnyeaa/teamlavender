using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewCombo", menuName = "ScriptableObjects/Combo", order = 1)]
public class Combo : ScriptableObject {
  public string _ComboName;

  public UnityEvent _ComboEvents;
}
