using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScuzzNamesList", menuName = "ScriptableObjects/ScuzzNamesList", order = 3)]
public class ScuzzNames : ScriptableObject {
  public List<string> names;
}
