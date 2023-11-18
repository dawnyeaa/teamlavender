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
  private NPCCustomiseSetObject custoObject;
  void Start() {
    ApplySavedCustomisation();
  }

  public void SaveCustomisation() {
    if (!custoObject) {
      var path = $"Assets/Config/NPCCustoSaves/{gameObject.GetInstanceID()}.asset";
      if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets))) {
        custoObject = AssetDatabase.LoadAssetAtPath<NPCCustomiseSetObject>(path);
      }
      else {
        custoObject = ScriptableObject.CreateInstance<NPCCustomiseSetObject>();

        AssetDatabase.CreateAsset(custoObject, path);
        AssetDatabase.SaveAssets();
      }
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
        Debug.Log($"slot: {slot.slotName}, selection = {info.selection}");
        custoObject.selections.Add(info);
      }
    }
    AssetDatabase.SaveAssets();
  }

  public void ApplySavedCustomisation() {
    if (!custoObject) {
      var path = $"Assets/Config/NPCCustoSaves/{gameObject.GetInstanceID()}.asset";
      if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets))) {
        custoObject = AssetDatabase.LoadAssetAtPath<NPCCustomiseSetObject>(path);
      }
      else {
        return;
      }
    }
    var character = GetComponent<CustomiseCharacter>();
    var slots = GetComponents<CustomiseSlot>();
    if (character) character.CustomiseColor(custoObject.characterHue);
    for (int i = 0; i < slots.Length; ++i) {
      var slot = slots[i];
      if (i < custoObject.selections.Count) {
        var info = custoObject.selections[i];
        slot.SetSelected(info.selection);
        slot.CustomiseColor(info.hue);
      }
    }
  }
}
