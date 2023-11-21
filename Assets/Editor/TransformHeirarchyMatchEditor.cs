using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TransformHeirarchyMatch))]
public class TransformHeirarchyMatchEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    var matcher = target as TransformHeirarchyMatch;
    if (GUILayout.Button("Match Hierarchy")) {
      int group = Undo.GetCurrentGroup();
      matcher.Match();
      Undo.CollapseUndoOperations(group);
    }
  }
}
