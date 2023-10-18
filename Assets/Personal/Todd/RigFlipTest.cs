using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RigFlipTest : MonoBehaviour {
  public Transform parentTransform;
  public bool flipIt = false;
  private Dictionary<string, (Vector3 pos, Quaternion rot)> savedTransforms;
  // we flipping sagitally (xz plane)
  private readonly Vector3 mirrorPlaneNormal = Vector3.right;

  public bool FLIPPY = false;
  public Transform a;
  
  void Update() {
    if (flipIt) {
      flipIt = false;
      FlipRig();
    }
    if (FLIPPY) {
      FLIPPY = !FLIPPY;
      (Vector3 fwd, Vector3 up) = (a.rotation * Vector3.forward, a.rotation * Vector3.up);
      fwd = MirrorVector(fwd, mirrorPlaneNormal);
      up = MirrorVector(up, mirrorPlaneNormal);
      a.rotation = Quaternion.LookRotation(fwd, up);
    }
  }

  void FlipRig() {
    savedTransforms ??= new Dictionary<string, (Vector3 pos, Quaternion rot)>();
    // first we do a pass to grab the rotations of all the bones

    for (int i = 0; i < parentTransform.childCount; ++i) {
      var child = parentTransform.GetChild(i);
      if (!child.name.ToLower().Contains("board")) {
        GetBoneTransforms(child);
      }
    }

    // then we do a pass to apply those rotations to them headwise

    for (int i = 0; i < parentTransform.childCount; ++i) {
      var child = parentTransform.GetChild(i);
      if (!child.name.ToLower().Contains("board")) {
        FlipBones(child);
      }
    }
  }

  void GetBoneTransforms(Transform bone) {
    var pos = bone.position;
    var rot = bone.rotation;
    savedTransforms.Add(bone.name, (pos, rot));

    for (int i = 0; i < bone.childCount; ++i) {
      var child = bone.GetChild(i);
      if (!child.name.ToLower().Contains("board")) {
        GetBoneTransforms(child);
      }
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

    if (sideBone)
      bone.position = sourceTransform.pos;
    else
      bone.position = MirrorVector(sourceTransform.pos, mirrorPlaneNormal);

    (Vector3 fwd, Vector3 up) = (sourceTransform.rot * Vector3.forward, sourceTransform.rot * Vector3.up);
    
    // if (sideBone) {
    //   fwd *= -1;
    //   up *= -1;
    // }

    // fwd = MirrorVector(fwd, mirrorPlaneNormal);
    // up = MirrorVector(up, mirrorPlaneNormal);

    // if (sideBone) {
    //   fwd *= -1;
    //   up *= -1;
    // }

    // bone.rotation = Quaternion.LookRotation(fwd, up);

    for (int i = 0; i < bone.childCount; ++i) {
      var child = bone.GetChild(i);
      if (!child.name.ToLower().Contains("board")) {
        FlipBones(child);
      }
    }
  }

  bool IsSideBone(string name) {
    var end = name[^2..];

    return end == "_L" || end == "_R";
  }

  string MirrorSideBoneName(string name) {
    var withoutEnd = name[..^2];
    var end = name[^2..];
    end = end.Replace("_L", "_SIDE").Replace("_R", "_L").Replace("_SIDE", "_R");

    return withoutEnd + end;
  }

  private Vector3 MirrorVector(Vector3 input, Vector3 mirrorNormal) {
    var projectOntoNormal = Vector3.Project(input, mirrorNormal);

    return input-(2*projectOntoNormal);
  }

  private (Vector3 a, Vector3 b, Vector3 c) Mirror3((Vector3 a, Vector3 b, Vector3 c) input, Vector3 mirrorNormal) {
    return (MirrorVector(input.a, mirrorNormal), MirrorVector(input.b, mirrorNormal), MirrorVector(input.c, mirrorNormal));
  }
}
