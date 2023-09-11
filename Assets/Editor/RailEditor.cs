using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rail))]
public class RailEditor : Editor {
  private readonly Color orange = new Color(1, 0.6f, 0);
  public void OnSceneGUI() {
    var t = target as Rail;

    EditorGUI.BeginChangeCheck();
    var newPos = Handles.PositionHandle(t.PointA, Quaternion.identity);
    if (EditorGUI.EndChangeCheck()) {
      t.PointA = newPos;
    }

    EditorGUI.BeginChangeCheck();
    newPos = Handles.PositionHandle(t.PointB, Quaternion.identity);
    if (EditorGUI.EndChangeCheck()) {
      t.PointB = newPos;
    }

    Handles.color = Color.red;
    Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
    Handles.DrawLine(t.PointA, t.PointB, 10);

    Handles.color = orange;

    Handles.ArrowHandleCap(0, Vector3.Lerp(t.PointA, t.PointB, 0.25f), Quaternion.LookRotation((t.RailOutside+Vector3.up)/2f, Vector3.up), 0.5f, EventType.Repaint);
    Handles.ArrowHandleCap(1, Vector3.Lerp(t.PointA, t.PointB, 0.5f), Quaternion.LookRotation((t.RailOutside+Vector3.up)/2f, Vector3.up), 0.5f, EventType.Repaint);
    Handles.ArrowHandleCap(2, Vector3.Lerp(t.PointA, t.PointB, 0.75f), Quaternion.LookRotation((t.RailOutside+Vector3.up)/2f, Vector3.up), 0.5f, EventType.Repaint);
  }
}