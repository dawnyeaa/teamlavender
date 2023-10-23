using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;

public class AnimationStateMachineGofer : EditorWindow {
  string[] clipNameBlacklist = {
    "BoardLock",
    "BoardTilt",
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

    AnimatorControllerLayer baseLayer = new(), ikLayer = new();
    bool baseAssigned = false, ikAssigned = false;
    foreach (var animatorLayer in animatorController.layers) {
      if (animatorLayer.name == "Base Layer") {
        baseLayer = animatorLayer;
        baseAssigned = true;
      }
      else if (animatorLayer.name == "legIKPos") {
        ikLayer = animatorLayer;
        ikAssigned = true;
      }
    }

    if (baseAssigned && ikAssigned) {
      ForAllSyncedMotions(ikLayer, baseLayer.stateMachine, (layer, state, motion) => {
        if (motion != null) {
          // theres either a clip or a blend tree here

          // if its a blend tree
          if (motion.GetType() == typeof(BlendTree)) {
            var blendTree = (BlendTree)motion;
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
              layer.SetOverrideMotion(state, ArrangeBlendTree(blendTree, true, clip));
          }
          else { // its an animation clip
            var clip = (AnimationClip)motion;
            var blendTree = new BlendTree();
            motion = blendTree;
            layer.SetOverrideMotion(state, motion);
            ArrangeBlendTree(blendTree, true, clip);
            AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(animatorController));
          }
        }
      });

      EditorUtility.SetDirty(animatorController);
      AssetDatabase.SaveAssets();
    }

  }
  Motion ArrangeBlendTree(BlendTree tree, bool startIsRegular, AnimationClip ikClip) {
    Debug.Log(ikClip.name);
    tree.name = "Blend Tree";
    tree.blendParameter = "mirrored";
    var ikmirroredClipPath = $"Assets/Animation/Clips/IKGenerated/IKMirrored{ToMiniTitleCase(ikClip.name[2..])}.anim";

    var ikmirrorClipExists = !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(ikmirroredClipPath, AssetPathToGUIDOptions.OnlyExistingAssets));

    if (!ikmirrorClipExists) return null;

    var ikmirroredClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ikmirroredClipPath);

    for (int i = tree.children.Length-1; i >= 0; --i) {
      tree.RemoveChild(i);
    }

    if (startIsRegular) {
      tree.AddChild(ikClip);
      tree.AddChild(ikmirroredClip);
    }
    else {
      tree.AddChild(ikmirroredClip);
      tree.AddChild(ikClip);
    }

    return tree;
  }

  void ForAllSyncedMotions(AnimatorControllerLayer layer, AnimatorStateMachine stateMachine, Action<AnimatorControllerLayer, AnimatorState, Motion> action) {
    foreach (var childAnimatorState in stateMachine.states) {
      var motion = layer.GetOverrideMotion(childAnimatorState.state);
      action.Invoke(layer, childAnimatorState.state, motion);
    }
    foreach (var childAnimatorStateMachine in stateMachine.stateMachines) {
      ForAllSyncedMotions(layer, childAnimatorStateMachine.stateMachine, action);
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