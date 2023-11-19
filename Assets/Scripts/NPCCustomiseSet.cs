using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class CustoSlotInfo {
  public int selection;
  public float hue;
}

public class NPCCustomiseSet : MonoBehaviour {
  public NPCCustomiseSetObject custoObject;
  void Start() {
    ApplySavedCustomisation();
  }

  public void SaveCustomisation() {
    var path = $"Assets/Config/NPCCustoSaves/{gameObject.name}.asset";
    if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets))) {
      custoObject = AssetDatabase.LoadAssetAtPath<NPCCustomiseSetObject>(path);
    }
    else {
      custoObject = ScriptableObject.CreateInstance<NPCCustomiseSetObject>();

      AssetDatabase.CreateAsset(custoObject, path);
    }
    var character = GetComponent<CustomiseCharacter>();
    var slots = GetComponents<CustomiseSlot>();
    if (character) custoObject.characterHue = character.GetT();
    for (int i = 0; i < slots.Length; ++i) {
      var slot = slots[i];
      var info = new CustoSlotInfo() {
        selection = slot.GetSelected(),
        hue = slot.GetT()
      };
      if (i < custoObject.selections.Count) {
        custoObject.selections[i] = info;
      }
      else {
        custoObject.selections.Add(info);
      }
    }
    EditorUtility.SetDirty(custoObject);
  }

  public void ApplySavedCustomisation() {
    var path = $"Assets/Config/NPCCustoSaves/{gameObject.name}.asset";
    if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets))) {
      custoObject = AssetDatabase.LoadAssetAtPath<NPCCustomiseSetObject>(path);
    }
    else {
      return;
    }
    var character = GetComponent<CustomiseCharacter>();
    var slots = GetComponents<CustomiseSlot>();
    if (character) character.CustomiseColor(custoObject.characterHue);
    for (int i = 0; i < slots.Length; ++i) {
      var slot = slots[i];
      if (i < custoObject.selections.Count) {
        var info = custoObject.selections[i];
        slot.UpdateSelected(info.selection);
        slot.CustomiseColor(info.hue);
      }
    }
  }
}
