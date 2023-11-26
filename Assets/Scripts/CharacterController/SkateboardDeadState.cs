using Cinemachine;
using UnityEngine;
using UnityEngine.Animations;
public class SkateboardDeadState : SkateboardBaseState
{
  private Vector3? velocity;

  public SkateboardDeadState(SkateboardStateMachine stateMachine, Vector3? velocity) : base(stateMachine)
  {
    this.velocity = velocity;
  }
  
  ConstraintSource CharacterSource, RagdollSource;
  public override void Enter() {
    sm.CanDie = false;
    SoundEffectsManager.instance.PlaySoundFXClip(sm.DeathClip, sm.transform, 1);
    // change to ragdoll here
    sm.FollowTargetConstraint.weight = 1;
    sm.LookAtTargetConstraint.weight = 1;
    sm.RegularModel.gameObject.SetActive(false);
    sm.RagdollModel.gameObject.SetActive(true);
    sm.RagdollMatcher.Match();
    foreach (Rigidbody rb in sm.RagdollTransformsToPush) {
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      Vector3 targetVelocity = velocity ?? sm.MainRB.velocity;
      rb.AddForce(targetVelocity/Time.fixedDeltaTime, ForceMode.Acceleration);
    }
    sm.MainRB.velocity = Vector3.zero;
    sm.PointManager.TrashPending();
    sm.PointHandler.Die();
    sm.DynamicCam.StartDead();
  }

  public override void Tick() {
  }

  public override void Exit() {
    sm.CanDie = true;
    // change back from ragdoll here
    sm.FollowTargetConstraint.weight = 0;
    sm.LookAtTargetConstraint.weight = 0;
    sm.RagdollModel.gameObject.SetActive(false);
    sm.RegularModel.gameObject.SetActive(true);
    sm.LandVFXTier = 0;
    sm.ComboController.ClearCurrentCombo();
    Spawn();
    sm.DynamicCam.EndDead();
    sm.DynamicCam.OverrideZoom(1);
  }
}