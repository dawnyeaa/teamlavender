using UnityEngine;

public abstract class Skateboard4BaseState : State {
  protected readonly Skateboard4StateMachine sm;
  protected float TurnSpeed = 0.0f;

  protected Skateboard4BaseState(Skateboard4StateMachine stateMachine) {
    this.sm = stateMachine;
  }

  protected void OnPush() {
    sm.BoardRb.AddForce(sm.transform.forward * sm.PushForce, ForceMode.Acceleration);
  }
}