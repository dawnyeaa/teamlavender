using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEditor.Animations;
using NUnit.Framework;
using System.Globalization;

public class AnimationMirrorWindow : EditorWindow {
  Animator animator;
  Transform boneParent;
  Transform animationRoot;
  string[] clipNameBlacklist = {
    "BoardLock",
    "BoardTilt",
    "IK",
    "Mirror"
  };
  private Dictionary<string, (Vector3 pos, Quaternion rot)> savedTransforms;
  private readonly Vector3 mirrorPlaneNormal = Vector3.right;

  private Dictionary<string, AnimationCurve[]> boneCurves;
  private Dictionary<string, string> fullPaths;
  private readonly int fps = 30;

  private readonly string[] channelNames = {
    "localPosition.x",
    "localPosition.y",
    "localPosition.z",
    "localRotation.x",
    "localRotation.y",
    "localRotation.z",
    "localRotation.w"
  };

  [MenuItem("Tools/AnimationMirror")]
  public static void ShowWindow() {
    GetWindow<AnimationMirrorWindow>("Animation Mirror");
  }

  void OnGUI() {
    animator = (Animator)EditorGUILayout.ObjectField("Animator", animator, typeof(Animator), true);
    boneParent = (Transform)EditorGUILayout.ObjectField("Parent Transform", boneParent, typeof(Transform), true);
    animationRoot = (Transform)EditorGUILayout.ObjectField("Animation Root Transform", animationRoot, typeof(Transform), true);
    if (GUILayout.Button("Mirror Animation Clips")) {
      MirrorAnimationClips();
    }
  }

  private void MirrorAnimationClips() {
    if (animator == null) return;
    
    if (!animator.isInitialized) {
      animator.Rebind();
    }
    
    var animatorController = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

    CreateAllBoneFullPaths();

    foreach (var animatorLayer in animatorController.layers) {
      if (animatorLayer.name != "Base Layer") continue;

      // foreach (var childAnimatorState in animatorLayer.stateMachine.states) {
      //   var state = childAnimatorState.state;
      //   if (state.motion != null && state.motion.name == "Blend Tree") {
      //     var childMotions = ((BlendTree)state.motion).children;
      //     foreach (var childMotion in childMotions) {
      //       if (childMotion.motion != null)
      //         Debug.Log(childMotion.motion.name);
      //     }
      //   }
      // }

      Dictionary<string, AnimationClip> clips = new();

      foreach (var clip in animatorController.animationClips) {
        if (!clips.ContainsKey(clip.name) && !IsBlacklistedClip(clip.name))
          clips.Add(clip.name, clip);
      }

      // then we do the following for *every animation clip*

      foreach (var baseClip in clips) {
        var clipPath = $"Assets/Animation/Clips/MirrorGenerated/Mirrored{ToMiniTitleCase(baseClip.Key)}.anim";

        var newClip = string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(clipPath, AssetPathToGUIDOptions.OnlyExistingAssets));

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

        // figure out the path for each curve to be applied on (write on hierarchy traversal)

        SetAllBoneCurves(clip);

        clip.frameRate = fps;
        var events = AnimationUtility.GetAnimationEvents(baseClip.Value);
        AnimationUtility.SetAnimationEvents(clip, events);

        if (newClip) {
          AssetDatabase.CreateAsset(clip, clipPath);
        }
        else {
          AssetDatabase.SaveAssets();
        }
      }
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
    boneCurves = new();
    CreateAllCurves();

    var layer = animator.GetLayerIndex("Base Layer");
    
    if (!animator.isInitialized) {
      animator.Rebind();
    }

    animator.Play(clipHash, layer);
    animator.Update(0);
    var currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    var duration = currentStateInfo.length;
    var numFrames = duration*fps;
    // Debug.Log($"duration: {duration}, frames: {numFrames}");

    for (int f = 0; f <= numFrames; ++f) {
      var currentTime = f/(float)fps;

      // flip the pose here
      FlipPose();

      // key all the flipped bones
      KeyAllFlippedBones(currentTime);

      animator.Update(1/((float)fps));
    }

    SetAllTangentsConstant();
  }

  void CreateAllCurves() {
    for (int i = 0; i < boneParent.childCount; ++i) {
      var child = boneParent.GetChild(i);
      CreateCurves(child);
    }
  }

  void CreateCurves(Transform bone) {
    AnimationCurve[] curves = new AnimationCurve[7];
    for (int i = 0; i < 7; ++i) {
      curves[i] = new();
    }
    if (boneCurves.ContainsKey(bone.name)) {
      boneCurves[bone.name] = curves;
    }
    else {
      boneCurves.Add(bone.name, curves);
    }

    for (int i = 0; i < bone.childCount; ++i) {
      var child = bone.GetChild(i);
      CreateCurves(child);
    }
  }

  void KeyAllFlippedBones(float time) {
    for (int i = 0; i < boneParent.childCount; ++i) {
      var child = boneParent.GetChild(i);
      KeyFlippedBones(child, time);
    }
  }

  void KeyFlippedBones(Transform bone, float time) {
    var curves = boneCurves[bone.name];
    boneCurves[bone.name] = KeyPose(curves, time, bone);

    for (int i = 0; i < bone.childCount; ++i) {
      var child = bone.GetChild(i);
      KeyFlippedBones(child, time);
    }
  }

  AnimationCurve[] KeyPose(AnimationCurve[] curves, float time, Transform bone) {
    var lpos = bone.localPosition;
    var lrot = bone.localRotation;
    curves[0].AddKey(time, lpos.x);
    curves[1].AddKey(time, lpos.y);
    curves[2].AddKey(time, lpos.z);
    curves[3].AddKey(time, lrot.x);
    curves[4].AddKey(time, lrot.y);
    curves[5].AddKey(time, lrot.z);
    curves[6].AddKey(time, lrot.w);
    return curves;
  }

  void SetAllTangentsConstant() {
    for (int i = 0; i < boneParent.childCount; ++i) {
      var child = boneParent.GetChild(i);
      SetTangentsConstant(child);
    }
  }

  void SetTangentsConstant(Transform bone) {
    var curves = boneCurves[bone.name];
    foreach (var curve in curves) {
      for (int i = 0; i < curve.length; ++i) {
        AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
        AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
      }
    }

    for (int i = 0; i < bone.childCount; ++i) {
      var child = bone.GetChild(i);
      SetTangentsConstant(child);
    }
  }

  void CreateAllBoneFullPaths() {
    fullPaths = new();

    for (int i = 0; i < animationRoot.childCount; ++i) {
      var child = animationRoot.GetChild(i);
      CreateBoneFullPaths(child, "");
    }
  }

  void CreateBoneFullPaths(Transform bone, string parentPath) {
    var path = (parentPath.Length != 0 ? $"{parentPath}/" : "") + $"{bone.name}";
    fullPaths.Add(bone.name, path);

    for (int i = 0; i < bone.childCount; ++i) {
      var child = bone.GetChild(i);
      if (!parentPath.Contains("JNT") || child.name.Contains("JNT"))
        CreateBoneFullPaths(child, path);
    }
  }

  void SetAllBoneCurves(AnimationClip clip) {
    for (int i = 0; i < boneParent.childCount; ++i) {
      var child = boneParent.GetChild(i);
      if (child.name.Contains("JNT"))
        SetBoneCurves(child, clip);
    }
  }

  void SetBoneCurves(Transform bone, AnimationClip clip) {
    var curves = boneCurves[bone.name];
    for (int i = 0; i < 7; ++i) {
      clip.SetCurve(fullPaths[bone.name], typeof(Transform), channelNames[i], curves[i]);
    }

    for (int i = 0; i < bone.childCount; ++i) {
      var child = bone.GetChild(i);
      if (child.name.Contains("JNT"))
        SetBoneCurves(child, clip);
    }
  }

  void FlipPose() {
    savedTransforms = new Dictionary<string, (Vector3 pos, Quaternion rot)>();
    // first we do a pass to grab the rotations of all the bones

    for (int i = 0; i < boneParent.childCount; ++i) {
      var child = boneParent.GetChild(i);
      if (child.name.Contains("JNT"))
        GetBoneTransforms(child);
    }

    // then we do a pass to apply those rotations to them headwise

    for (int i = 0; i < boneParent.childCount; ++i) {
      var child = boneParent.GetChild(i);
      if (child.name.Contains("JNT"))
        FlipBones(child);
    }
  }

  void GetBoneTransforms(Transform bone) {
    var pos = bone.position;
    var rot = bone.rotation;
    savedTransforms.Add(bone.name, (pos, rot));

    for (int i = 0; i < bone.childCount; ++i) {
      var child = bone.GetChild(i);
      if (child.name.Contains("JNT"))
        GetBoneTransforms(child);
    }
  }

  void FlipBones(Transform bone) {
    var name = bone.name;
    (Vector3 pos, Quaternion rot) sourceTransform;

    bool sideBone = IsSideBone(name);

    if (sideBone) {
      // get the transform of the opposite side's bone, use that flipped
      sourceTransform = savedTransforms[MirrorSideBoneName(name)];
    }
    else {
      sourceTransform = savedTransforms[name];
    }

    bone.position = MirrorVector(sourceTransform.pos, mirrorPlaneNormal);

    (Vector3 fwd, Vector3 up) = (sourceTransform.rot * Vector3.forward, sourceTransform.rot * Vector3.up);

    fwd = MirrorVector(fwd, mirrorPlaneNormal);
    up = MirrorVector(up, mirrorPlaneNormal);

    if (sideBone) {
      // needs to be rotated 180 around x in order to be correct for opposite limb
      fwd *= -1;
      up *= -1;
    }

    bone.rotation = Quaternion.LookRotation(fwd, up);

    for (int i = 0; i < bone.childCount; ++i) {
      var child = bone.GetChild(i);
      if (child.name.Contains("JNT"))
        FlipBones(child);
    }
  }

  bool IsSideBone(string name) {
    var end = name[^2..];
    var start = name[..8];

    return end == "_L" || end == "_R" || start == "JNT_Nose" || start == "JNT_Tail";
  }

  string MirrorSideBoneName(string name) {
    var start = name[..8];
    if (start == "JNT_Nose" || start == "JNT_Tail") {
      var withoutStart = name[8..];
      start = start.Replace("JNT_Nose", "JNT_Temp").Replace("JNT_Tail", "JNT_Nose").Replace("JNT_Temp", "JNT_Tail");
      return start + withoutStart;
    }
    else {
      var withoutEnd = name[..^2];
      var end = name[^2..];
      end = end.Replace("_L", "_SIDE").Replace("_R", "_L").Replace("_SIDE", "_R");

      return withoutEnd + end;
    }
  }

  private Vector3 MirrorVector(Vector3 input, Vector3 mirrorNormal) {
    var projectOntoNormal = Vector3.Project(input, mirrorNormal);

    return input-(2*projectOntoNormal);
  }

  private string ToMiniTitleCase(string s) => s[0].ToString().ToUpperInvariant() + s[1..];
}