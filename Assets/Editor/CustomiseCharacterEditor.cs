using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CustomiseCharacter))]
public class CustomiseCharacterEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    var chara = target as CustomiseCharacter;
    chara.CustomiseColor(EditorGUILayout.Slider(chara.GetT(), 0, 1));
  }
}