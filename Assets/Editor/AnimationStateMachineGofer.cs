using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;

public class AnimationStateMachineGofer : EditorWindow {
  string[] clipNameBlacklist = {
    "BoardLock",
    "BoardTilt",
    "IK",
    "Mirror"
  };

  [MenuItem("Tools/AnimationStateMachineGofer")]
  public static void ShowWindow() {
    GetWindow<AnimationStateMachineGofer>("AnimatorGofer");
  }

  void OnGUI() {
    if (GUILayout.Button("Place Mirrored Animations in Animator")) {
      PlaceMirroredAnimations();
    }
  }

  private void PlaceMirroredAnimations() {
    UnityEngine.Object w = AssetDatabase.LoadAssetAtPath("Assets/Animation/Character.controller", typeof(AnimatorController));
    var animatorController = w as AnimatorController;

    foreach (var animatorLayer in animatorController.layers) {
      if (animatorLayer.name != "Base Layer") continue;

      ForAllStates(animatorLayer.stateMachine, (state) => {
        if (state.motion != null) {
          // theres either a clip or a blend tree here

          // if its a blend tree
          if (state.motion.GetType() == typeof(BlendTree)) {
            var blendTree = (BlendTree)state.motion;
            var childMotions = blendTree.children;
            bool found = false;
            AnimationClip clip = new();
            foreach (var childMotion in childMotions) {
              if (childMotion.motion != null && !IsBlacklistedClip(childMotion.motion.name)) {
                found = true;
                clip = (AnimationClip)childMotion.motion;
                break;
              }
            }
            if (found)
              ArrangeBlendTree(blendTree, true, clip);
          }
          else { // its an animation clip
            var clip = (AnimationClip)state.motion;
            var blendTree = new BlendTree();
            state.motion = blendTree;
            ArrangeBlendTree(blendTree, true, clip);
            AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(animatorController));
          }
        }
      });

    }

    EditorUtility.SetDirty(animatorController);
    AssetDatabase.SaveAssets();

  }
  void ArrangeBlendTree(BlendTree tree, bool startIsRegular, AnimationClip regularClip) {
    tree.name = "Blend Tree";
    tree.blendParameter = "mirrored";
    var mirroredClipPath = $"Assets/Animation/Clips/MirrorGenerated/Mirrored{ToMiniTitleCase(regularClip.name)}.anim";

    var clipExists = !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(mirroredClipPath, AssetPathToGUIDOptions.OnlyExistingAssets));

    if (!clipExists) return;

    var mirroredClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(mirroredClipPath);

    for (int i = tree.children.Length-1; i >= 0; --i) {
      tree.RemoveChild(i);
    }

    if (startIsRegular) {
      tree.AddChild(regularClip);
      tree.AddChild(mirroredClip);
    }
    else {
      tree.AddChild(mirroredClip);
      tree.AddChild(regularClip);
    }

  }

  void ForAllStates(AnimatorStateMachine stateMachine, Action<AnimatorState> action) {
    foreach (var childAnimatorState in stateMachine.states) {
      action.Invoke(childAnimatorState.state);
    }
    foreach (var childAnimatorStateMachine in stateMachine.stateMachines) {
      ForAllStates(childAnimatorStateMachine.stateMachine, action);
    }
  }

  bool IsBlacklistedClip(string name) {
    foreach (var phrase in clipNameBlacklist) {
      if (name.Contains(phrase))
        return true;
    }
    return false;
  }

  private string ToMiniTitleCase(string s) => s[0].ToString().ToUpperInvariant() + s[1..];
}