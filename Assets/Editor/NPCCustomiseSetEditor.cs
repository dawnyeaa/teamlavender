using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(NPCCustomiseSet))]
public class NPCCustomiseSetEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    var customiseSet = target as NPCCustomiseSet;
    if (GUILayout.Button("Save Customisation")) {
      customiseSet.SaveCustomisation();
    }
  }
}
