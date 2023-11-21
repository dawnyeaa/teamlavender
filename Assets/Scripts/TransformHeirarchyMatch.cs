using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TransformHeirarchyMatch : MonoBehaviour {
  [SerializeField] private Transform sourceTransform;

  public void Match() {
    if (sourceTransform == null)
      return;
    CopyTransform(sourceTransform, transform);
    MatchChildren(sourceTransform, transform);
  }

  private static void MatchChildren(Transform source, Transform destination) {
    for (int i = 0; i < source.childCount && i < destination.childCount; ++i) {
      CopyTransform(source.GetChild(i), destination.GetChild(i));
      MatchChildren(source.GetChild(i), destination.GetChild(i));
    }
  }

  private static void CopyTransform(Transform source, Transform destination) {
    #if UNITY_EDITOR
    Undo.RecordObject(destination, "transform positions");
    #endif
    destination.SetLocalPositionAndRotation(source.localPosition, source.localRotation);
  }

  public void Match(Transform sourceTransform) {
    if (!sourceTransform) return;
    this.sourceTransform = sourceTransform;
    Match();
  }

}
