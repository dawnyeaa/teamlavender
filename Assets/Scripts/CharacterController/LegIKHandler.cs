using RootMotion.FinalIK;
using UnityEngine;

public class LegIKHandler : MonoBehaviour {
  [Range(0, 1)] public float leftWeight = 1, rightWeight = 1;
  public LimbIK leftIK, rightIK;
  public bool linkLegIKs = false;
  public void OnValidate() {
    if (linkLegIKs) {
      linkLegIKs = false;
      LimbIK[] iks = GetComponents<LimbIK>();
      if (iks.Length < 2) return;

      leftIK = iks[0];
      rightIK = iks[1];

      if (leftIK.solver.goal == AvatarIKGoal.RightFoot) {
        rightIK = iks[0];
        leftIK = iks[1];
      }
    }
  }
  void Update() {
    if (leftIK != null && rightIK != null) {
      leftIK.solver.IKPositionWeight = leftWeight;
      leftIK.solver.IKRotationWeight = leftWeight;
      rightIK.solver.IKPositionWeight = rightWeight;
      rightIK.solver.IKRotationWeight = rightWeight;
    }
  }
}
