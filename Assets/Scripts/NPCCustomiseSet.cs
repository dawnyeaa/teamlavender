using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NPCCustomiseSet : MonoBehaviour {
  public NPCCustomiseSetObject custoObject;
  void Start() {
    ApplySavedCustomisation();
  }

  public void SaveCustomisation() {
    #if UNITY_EDITOR
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
      if (character) {
        custoObject.characterHue = character.GetT();
        custoObject.characterGradientIndex = character.gradientIndex;
        custoObject.deckIndex = character.deckIndex;
      }
      for (int i = 0; i < slots.Length; ++i) {
        var slot = slots[i];
        var info = new CustoSlotInfo() {
          selection = slot.GetSelected(),
          hue = slot.GetT(),
          gradientIndex = slot.GetGradientIndex()
        };
        if (i < custoObject.selections.Count) {
          custoObject.selections[i] = info;
        }
        else {
          custoObject.selections.Add(info);
        }
      }
      EditorUtility.SetDirty(custoObject);
    #endif
  }

  public void ApplySavedCustomisation() {
    #if UNITY_EDITOR
    var path = $"Assets/Config/NPCCustoSaves/{gameObject.name}.asset";
    if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets))) {
      custoObject = AssetDatabase.LoadAssetAtPath<NPCCustomiseSetObject>(path);
    }
    else {
      return;
    }
    #else
    if (custoObject == null) return;
    #endif
    var character = GetComponent<CustomiseCharacter>();
    var slots = GetComponents<CustomiseSlot>();
    if (character) {
      character.CustomiseColor(custoObject.characterHue);
      character.SetDeck(custoObject.deckIndex);
      character.SetGradient(custoObject.characterGradientIndex);
    }
    for (int i = 0; i < slots.Length; ++i) {
      var slot = slots[i];
      if (i < custoObject.selections.Count) {
        var info = custoObject.selections[i];
        slot.UpdateSelected(info.selection);
        slot.CustomiseColor(info.hue);
        slot.SetGradient(info.gradientIndex);
      }
    }
  }
}
