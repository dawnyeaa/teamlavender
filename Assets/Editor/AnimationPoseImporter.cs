using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationPostImporter : AssetPostprocessor {
  void OnPostprocessAnimation(GameObject root, AnimationClip anim) {
    if (!root.name.Contains("Stepped")) return;
    if (!anim.name.Contains("Pose")) return;

    EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(anim);
    foreach (EditorCurveBinding binding in bindings) {
      AnimationCurve curve = AnimationUtility.GetEditorCurve(anim, binding);
      Keyframe[] keys = curve.keys;
      keys[1].value = keys[0].value;
      curve.keys = keys;
      AnimationUtility.SetEditorCurve(anim, binding, curve);
    }
  }
}