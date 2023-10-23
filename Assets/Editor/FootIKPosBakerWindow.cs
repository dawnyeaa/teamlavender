using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEditor.Animations;
using System.Linq;
using System;

public class FootIKPosBakerWindow : EditorWindow {
  Animator animator;
  Transform leftFoot, rightFoot;
  Transform destParentL, destParentR;
  string[] clipNameBlacklist = {
    "BoardLock",
    "BoardTilt",
    "IK"
  };

  private AnimationCurve[] leftFootCurrentCurve, rightFootCurrentCurve;
  private readonly int fps = 30;

  [MenuItem("Tools/FootIKClipBaker")]
  public static void ShowWindow() {
    GetWindow<FootIKPosBakerWindow>("Foot IK Clip Baker");
  }

  void OnGUI() {
    animator = (Animator)EditorGUILayout.ObjectField("Animator", animator, typeof(Animator), true);
    leftFoot = (Transform)EditorGUILayout.ObjectField("Left Foot", leftFoot, typeof(Transform), true);
    rightFoot = (Transform)EditorGUILayout.ObjectField("Right Foot", rightFoot, typeof(Transform), true);
    destParentL = (Transform)EditorGUILayout.ObjectField("Destination Parent Left", destParentL, typeof(Transform), true);
    destParentR = (Transform)EditorGUILayout.ObjectField("Destination Parent Right", destParentR, typeof(Transform), true);
    if (GUILayout.Button("Bake Foot IK To Clips")) {
      BakeFootIKClips();
    }
  }

  private void BakeFootIKClips() {
    if (animator == null) return;
    
    if (!animator.isInitialized) {
      Debug.Log("rebinding");
      animator.Rebind();
    }
    
    var animatorController = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

    foreach (var animatorLayer in animatorController.layers) {
      if (animatorLayer.name != "Base Layer") continue;

      var allStates = new List<AnimatorState>();

      Dictionary<string, AnimationClip> clips = new();

      foreach (var clip in animatorController.animationClips) {
        if (!clips.ContainsKey(clip.name) && !IsBlacklistedClip(clip.name))
          clips.Add(clip.name, clip);
      }

      foreach (var baseClip in clips) {
        var clipPath = $"Assets/Animation/Clips/IKGenerated/IK{baseClip.Key}.anim";

        var newClip = string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(clipPath));

        AnimationClip clip;

        if (newClip) {
          clip = new();
        }
        else {
          clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        }

        var newState = animatorLayer.stateMachine.AddState("new state");

        newState.motion = baseClip.Value;
        
        PlayClipAndRecord(newState.nameHash);

        animatorLayer.stateMachine.RemoveState(newState);

        clip.ClearCurves();

        clip.SetCurve("LegIK/LeftRotator/LeftFoot", typeof(Transform), "localPosition.x", leftFootCurrentCurve[0]);
        clip.SetCurve("LegIK/LeftRotator/LeftFoot", typeof(Transform), "localPosition.z", leftFootCurrentCurve[2]);
        clip.SetCurve("LegIK/LeftRotator/LeftFoot", typeof(Transform), "localPosition.y", leftFootCurrentCurve[1]);
        clip.SetCurve("LegIK/LeftRotator/LeftFoot", typeof(Transform), "localRotation.x", leftFootCurrentCurve[3]);
        clip.SetCurve("LegIK/LeftRotator/LeftFoot", typeof(Transform), "localRotation.y", leftFootCurrentCurve[4]);
        clip.SetCurve("LegIK/LeftRotator/LeftFoot", typeof(Transform), "localRotation.z", leftFootCurrentCurve[5]);
        clip.SetCurve("LegIK/LeftRotator/LeftFoot", typeof(Transform), "localRotation.w", leftFootCurrentCurve[6]);
        
        clip.SetCurve("LegIK/RightRotator/RightFoot", typeof(Transform), "localPosition.x", rightFootCurrentCurve[0]);
        clip.SetCurve("LegIK/RightRotator/RightFoot", typeof(Transform), "localPosition.y", rightFootCurrentCurve[1]);
        clip.SetCurve("LegIK/RightRotator/RightFoot", typeof(Transform), "localPosition.z", rightFootCurrentCurve[2]);
        clip.SetCurve("LegIK/RightRotator/RightFoot", typeof(Transform), "localRotation.x", rightFootCurrentCurve[3]);
        clip.SetCurve("LegIK/RightRotator/RightFoot", typeof(Transform), "localRotation.y", rightFootCurrentCurve[4]);
        clip.SetCurve("LegIK/RightRotator/RightFoot", typeof(Transform), "localRotation.z", rightFootCurrentCurve[5]);
        clip.SetCurve("LegIK/RightRotator/RightFoot", typeof(Transform), "localRotation.w", rightFootCurrentCurve[6]);

        clip.frameRate = fps;

        if (newClip) {
          AssetDatabase.CreateAsset(clip, clipPath);
        }
        else {
          AssetDatabase.SaveAssets();
        }
      }
    }
  }

  void ForAllValidClips(AnimatorStateMachine stateMachine, Action<AnimationClip> action) {
    ForAllStates(stateMachine, (state) => {
      if (state.motion != null) {

        if (state.motion.GetType() == typeof(BlendTree)) {
          var blendTree = (BlendTree)state.motion;
          foreach (var child in blendTree.children) {
            var clip = (AnimationClip)child.motion;
            if (!IsBlacklistedClip(clip.name))
              action.Invoke(clip);
          }
        }
        else {
          var clip = (AnimationClip)state.motion;
          if (!IsBlacklistedClip(clip.name))
            action.Invoke(clip);
        }
      }
    });
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

  void PlayClipAndRecord(int clipHash) {
    leftFootCurrentCurve = new AnimationCurve[7];
    rightFootCurrentCurve = new AnimationCurve[7];

    for (int i = 0; i < 7; ++i) {
      leftFootCurrentCurve[i] = new();
      rightFootCurrentCurve[i] = new();
    }

    var layer = animator.GetLayerIndex("Base Layer");
    
    if (!animator.isInitialized) {
      animator.Rebind();
    }

    animator.Play(clipHash, layer);
    animator.Update(0);
    var currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    var duration = currentStateInfo.length;
    var numFrames = duration*fps;
    Debug.Log($"duration: {duration}, frames: {numFrames}");

    for (int f = 0; f <= numFrames; ++f) {
      var currentTime = f/(float)fps;

      var lpos = destParentL.InverseTransformPoint(leftFoot.position);
      var rpos = destParentR.InverseTransformPoint(rightFoot.position);

      var lrot = Quaternion.Inverse(destParentL.rotation) * leftFoot.rotation;
      var rrot = Quaternion.Inverse(destParentR.rotation) * rightFoot.rotation;

      leftFootCurrentCurve[0].AddKey(currentTime, lpos.x);
      leftFootCurrentCurve[1].AddKey(currentTime, lpos.y);
      leftFootCurrentCurve[2].AddKey(currentTime, lpos.z);

      leftFootCurrentCurve[3].AddKey(currentTime, lrot.x);
      leftFootCurrentCurve[4].AddKey(currentTime, lrot.y);
      leftFootCurrentCurve[5].AddKey(currentTime, lrot.z);
      leftFootCurrentCurve[6].AddKey(currentTime, lrot.w);
      

      rightFootCurrentCurve[0].AddKey(currentTime, rpos.x);
      rightFootCurrentCurve[1].AddKey(currentTime, rpos.y);
      rightFootCurrentCurve[2].AddKey(currentTime, rpos.z);

      rightFootCurrentCurve[3].AddKey(currentTime, rrot.x);
      rightFootCurrentCurve[4].AddKey(currentTime, rrot.y);
      rightFootCurrentCurve[5].AddKey(currentTime, rrot.z);
      rightFootCurrentCurve[6].AddKey(currentTime, rrot.w);

      animator.Update(1/((float)fps));
    }

    for (int i = 0; i < 7; ++i) {
      for (int j = 0; j < leftFootCurrentCurve[i].length; ++j) {
        AnimationUtility.SetKeyLeftTangentMode(leftFootCurrentCurve[i], j, AnimationUtility.TangentMode.Constant);
        AnimationUtility.SetKeyRightTangentMode(leftFootCurrentCurve[i], j, AnimationUtility.TangentMode.Constant);
        AnimationUtility.SetKeyLeftTangentMode(rightFootCurrentCurve[i], j, AnimationUtility.TangentMode.Constant);
        AnimationUtility.SetKeyRightTangentMode(rightFootCurrentCurve[i], j, AnimationUtility.TangentMode.Constant);
      }
    }
  }
}