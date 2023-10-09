using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rail))]
public class RailEditor : Editor {
  private readonly Color orange = new(1, 0.6f, 0);
  public void OnSceneGUI() {
    var t = target as Rail;

    EditorGUI.BeginChangeCheck();
    var newPos = Handles.PositionHandle(t.TransformA.position, Quaternion.identity);
    if (EditorGUI.EndChangeCheck()) {
      t.TransformA.position = newPos;
    }

    EditorGUI.BeginChangeCheck();
    newPos = Handles.PositionHandle(t.TransformB.position , Quaternion.identity);
    if (EditorGUI.EndChangeCheck()) {
      t.TransformB.position  = newPos;
    }

    Handles.color = Color.red;
    Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
    Handles.DrawLine(t.TransformA.position, t.TransformB.position, 10);

    Handles.color = orange;

    Handles.ArrowHandleCap(0, Vector3.Lerp(t.TransformA.position, t.TransformB.position, 0.25f), Quaternion.LookRotation((t.RailOutside+Vector3.up)/2f, Vector3.up), 0.5f, EventType.Repaint);
    Handles.ArrowHandleCap(1, Vector3.Lerp(t.TransformA.position, t.TransformB.position, 0.5f), Quaternion.LookRotation((t.RailOutside+Vector3.up)/2f, Vector3.up), 0.5f, EventType.Repaint);
    Handles.ArrowHandleCap(2, Vector3.Lerp(t.TransformA.position, t.TransformB.position, 0.75f), Quaternion.LookRotation((t.RailOutside+Vector3.up)/2f, Vector3.up), 0.5f, EventType.Repaint);
  }
}