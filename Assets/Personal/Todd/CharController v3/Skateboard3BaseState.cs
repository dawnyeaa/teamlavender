using UnityEngine;
using UnityEditor;

public abstract class Skateboard3BaseState : State {
  protected readonly Skateboard3StateMachine stateMachine;
  protected float TurnSpeed = 0.0f;

  protected Skateboard3BaseState(Skateboard3StateMachine stateMachine) {
    this.stateMachine = stateMachine;
  }

  protected void OnPush() {
    Vector3 pushDirection = GetForwards();
    stateMachine.BoardRb.AddForce(stateMachine.PushForce*pushDirection);
  }

  protected Vector3 GetForwards() {
    return stateMachine.BoardRb.transform.forward * (stateMachine.FacingForward ? 1 : -1);
  }

  protected void CalculateTurn() {
    Vector3 forwardVelocity = Vector3.Project(stateMachine.BoardRb.velocity, GetForwards());
    if (forwardVelocity.magnitude > Mathf.Epsilon && stateMachine.Grounded) {
      stateMachine.Turning = Mathf.SmoothDamp(stateMachine.Turning, stateMachine.Input.turn, ref TurnSpeed, stateMachine.TurnSpeedDamping);
      float rad = stateMachine.TruckSpacing/Mathf.Sin(Mathf.Deg2Rad*stateMachine.Turning*stateMachine.MaxTruckTurnDeg);
      float forceMag = stateMachine.BoardRb.mass * ((forwardVelocity.magnitude*forwardVelocity.magnitude)/rad);
      Vector3 right = Vector3.Cross(GetForwards(), stateMachine.BoardRb.transform.up);
      Vector3 centriForce = forceMag * -right;
      stateMachine.BoardRb.AddForce(centriForce);
      //*********************** DO TORQUE TOMORROW *******************************
      // stateMachine.BoardRb.AddTorque(forceMag*stateMachine.BoardRb.transform.up);
    }
  }

  protected void SetFriction() {
    if (stateMachine.Input.braking)
      stateMachine.PhysMat.dynamicFriction = stateMachine.BrakingFriction;
    else
      stateMachine.PhysMat.dynamicFriction = stateMachine.WheelFriction;

  }
}