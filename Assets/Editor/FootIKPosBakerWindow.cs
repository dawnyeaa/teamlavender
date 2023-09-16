using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class FootIKPosBakerWindow : EditorWindow {
  Animator animator;
  Transform leftFoot, rightFoot;
  Transform destParent;

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
    destParent = (Transform)EditorGUILayout.ObjectField("Destination Parent", destParent, typeof(Transform), true);
    if (GUILayout.Button("Bake Foot IK To Clips")) {
      BakeFootIKClips();
    }
  }

  private void BakeFootIKClips() {
    if (animator == null) return;
    
    if (!animator.isInitialized)
      animator.Rebind();
    
    var animatorController = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

    foreach (var animatorLayer in animatorController.layers) {
      if (animatorLayer.name != "Base Layer") continue;

      foreach (var childAnimatorState in animatorLayer.stateMachine.states) {
        foreach (var transition in childAnimatorState.state.transitions) {
          transition.mute = true;
        }
      }

      foreach (var childAnimatorState in animatorLayer.stateMachine.states) {
        AnimationClip clip = new();
        
        PlayClipAndRecord(childAnimatorState.state.nameHash);

        clip.SetCurve("LegIK/LeftFoot", typeof(Transform), "localPosition.x", leftFootCurrentCurve[0]);
        clip.SetCurve("LegIK/LeftFoot", typeof(Transform), "localPosition.y", leftFootCurrentCurve[1]);
        clip.SetCurve("LegIK/LeftFoot", typeof(Transform), "localPosition.z", leftFootCurrentCurve[2]);
        clip.SetCurve("LegIK/LeftFoot", typeof(Transform), "localRotation.x", leftFootCurrentCurve[3]);
        clip.SetCurve("LegIK/LeftFoot", typeof(Transform), "localRotation.y", leftFootCurrentCurve[4]);
        clip.SetCurve("LegIK/LeftFoot", typeof(Transform), "localRotation.z", leftFootCurrentCurve[5]);
        clip.SetCurve("LegIK/LeftFoot", typeof(Transform), "localRotation.w", leftFootCurrentCurve[6]);
        
        clip.SetCurve("LegIK/RightFoot", typeof(Transform), "localPosition.x", rightFootCurrentCurve[0]);
        clip.SetCurve("LegIK/RightFoot", typeof(Transform), "localPosition.y", rightFootCurrentCurve[1]);
        clip.SetCurve("LegIK/RightFoot", typeof(Transform), "localPosition.z", rightFootCurrentCurve[2]);
        clip.SetCurve("LegIK/RightFoot", typeof(Transform), "localRotation.x", rightFootCurrentCurve[3]);
        clip.SetCurve("LegIK/RightFoot", typeof(Transform), "localRotation.y", rightFootCurrentCurve[4]);
        clip.SetCurve("LegIK/RightFoot", typeof(Transform), "localRotation.z", rightFootCurrentCurve[5]);
        clip.SetCurve("LegIK/RightFoot", typeof(Transform), "localRotation.w", rightFootCurrentCurve[6]);

        clip.frameRate = fps;

        var newClipName = $"Assets/Animation/Clips/IKGenerated/IK{childAnimatorState.state.name}.anim";
        AssetDatabase.DeleteAsset(newClipName);
        AssetDatabase.CreateAsset(clip, newClipName);
      }
    }
  }

  void PlayClipAndRecord(int clipHash) {
    leftFootCurrentCurve = new AnimationCurve[7];
    rightFootCurrentCurve = new AnimationCurve[7];

    for (int i = 0; i < 7; ++i) {
      leftFootCurrentCurve[i] = new();
      rightFootCurrentCurve[i] = new();
    }

    var layer = animator.GetLayerIndex("Base Layer");

    animator.Play(clipHash, layer);
    animator.Update(0);
    var duration = animator.GetCurrentAnimatorStateInfo(0).length;
    var numFrames = duration*fps;

    for (int f = 0; f <= numFrames; ++f) {
      var currentTime = f/(float)fps;

      var lpos = destParent.InverseTransformPoint(leftFoot.position);
      var rpos = destParent.InverseTransformPoint(rightFoot.position);

      var lrot = Quaternion.Inverse(destParent.rotation) * leftFoot.rotation;
      var rrot = Quaternion.Inverse(destParent.rotation) * rightFoot.rotation;

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