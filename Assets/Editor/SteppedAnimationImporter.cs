using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SteppedAnimationImporter : AssetPostprocessor {
  void OnPostprocessAnimation(GameObject root, AnimationClip anim) {
    if (!root.name.Contains("Stepped")) return;

    EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(anim);
    foreach (EditorCurveBinding binding in bindings) {
      AnimationCurve curve = AnimationUtility.GetEditorCurve(anim, binding);
      for (int i = 0; i < curve.length; ++i) {
        AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
        AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
      }
      AnimationUtility.SetEditorCurve(anim, binding, curve);
    }
  }
}