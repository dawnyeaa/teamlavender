using UnityEngine;
using UnityEngine.Animations;
public class SkateboardDeadState : SkateboardBaseState {
  public SkateboardDeadState(SkateboardStateMachine stateMachine) : base(stateMachine) {}
  ConstraintSource CharacterSource, RagdollSource;
  public override void Enter() {
    // change to ragdoll here
    CharacterSource = sm.LookatConstraint.GetSource(0);
    CharacterSource.weight = 0;
    RagdollSource = sm.LookatConstraint.GetSource(1);
    RagdollSource.weight = 1;
    sm.LookatConstraint.SetSource(0, CharacterSource);
    sm.LookatConstraint.SetSource(1, RagdollSource);
    sm.RegularModel.gameObject.SetActive(false);
    sm.RagdollModel.gameObject.SetActive(true);
    sm.RagdollMatcher.Match();
    foreach (Rigidbody rb in sm.RagdollTransformsToPush) {
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      Vector3 targetVelocity = sm.BoardRb.velocity;
      rb.AddForce(targetVelocity/Time.fixedDeltaTime, ForceMode.Acceleration);
    }
    sm.BoardRb.velocity = Vector3.zero;
    sm.PointManager.TrashPending();
  }

  public override void Tick() {
  }

  public override void Exit() {
    // change back from ragdoll here
    CharacterSource.weight = 1;
    sm.LookatConstraint.SetSource(0, CharacterSource);
    RagdollSource.weight = 0;
    sm.LookatConstraint.SetSource(1, RagdollSource);
    sm.RagdollModel.gameObject.SetActive(false);
    sm.RegularModel.gameObject.SetActive(true);
    Spawn();
  }
}