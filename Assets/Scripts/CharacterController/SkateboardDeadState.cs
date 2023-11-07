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
    SoundEffectsManager.instance.PlaySoundFXClip(sm.DeathClip, sm.transform, 1);
    // change to ragdoll here
    sm.FollowTargetConstraint.weight = 1;
    sm.LookAtTargetConstraint.weight = 1;
    sm.cinemachineLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_ScreenY = 0.5f;
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
  }

  public override void Tick() {
  }

  public override void Exit() {
    // change back from ragdoll here
    sm.FollowTargetConstraint.weight = 0;
    sm.LookAtTargetConstraint.weight = 0;
    sm.cinemachineLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_ScreenY = 0.83f;
    sm.RagdollModel.gameObject.SetActive(false);
    sm.RegularModel.gameObject.SetActive(true);
    Spawn();
  }
}