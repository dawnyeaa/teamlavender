using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CustomiseSlot))]
public class CustomiseSlotEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    var slot = target as CustomiseSlot;
    if (GUILayout.Button("Previous Item")) {
      slot.SelectPreviousOption();
    }
    if (GUILayout.Button("Next Item")) {
      slot.SelectNextOption();
    }
  }
}