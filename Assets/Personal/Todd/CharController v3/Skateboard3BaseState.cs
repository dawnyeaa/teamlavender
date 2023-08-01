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

  protected Vector3 GetForwards() => stateMachine.BoardRb.transform.forward * (stateMachine.FacingForward ? 1 : -1);

  protected void BallDebug() {
    stateMachine.TruckTurnPercent = Mathf.SmoothDamp(stateMachine.TruckTurnPercent, stateMachine.Input.turn, ref TurnSpeed, stateMachine.TruckTurnDamping);
    float localTruckTurnPercent = stateMachine.TurningEase.Evaluate(stateMachine.TruckTurnPercent)*Mathf.Sign(stateMachine.TruckTurnPercent);

    stateMachine.ball1.position = new Vector3(stateMachine.Input.turn * 2, stateMachine.ball1.position.y, stateMachine.ball1.position.z);
    stateMachine.ball2.position = new Vector3(stateMachine.TruckTurnPercent * 2, stateMachine.ball2.position.y, stateMachine.ball2.position.z);
    stateMachine.ball3.position = new Vector3(localTruckTurnPercent * 2, stateMachine.ball3.position.y, stateMachine.ball3.position.z);
  }

  protected void CalculateTurn() {
    stateMachine.TruckTurnPercent = Mathf.SmoothDamp(stateMachine.TruckTurnPercent, stateMachine.Input.turn, ref TurnSpeed, stateMachine.TruckTurnDamping);
    float localTruckTurnPercent = stateMachine.TurningEase.Evaluate(stateMachine.TruckTurnPercent)*Mathf.Sign(stateMachine.TruckTurnPercent);
    for (int i = 0; i < 2; ++i) {
      Transform truckTransform = i == 0 ? stateMachine.frontAxis : stateMachine.backAxis;
      localTruckTurnPercent *= (i == 0 ? 1 : -1);
      // get the new forward for the truck
      Vector3 accelDir = Quaternion.AngleAxis(localTruckTurnPercent*stateMachine.MaxTruckTurnDeg, truckTransform.up) * truckTransform.parent.forward;
      // rotate the truck transforms - can easily see from debug axes
      truckTransform.rotation = Quaternion.LookRotation(accelDir, truckTransform.up);
      // get the truck's steering axis (right-left)
      Vector3 steeringDir = Quaternion.AngleAxis(localTruckTurnPercent*stateMachine.MaxTruckTurnDeg, truckTransform.up) * truckTransform.parent.right * (i == 0 ? 1 : -1);
      // get the current velocity of the truck
      Vector3 truckWorldVel = stateMachine.BoardRb.GetPointVelocity(truckTransform.position);
      // get the speed in the trucks steering direction (right-left)
      float steeringVel = Vector3.Dot(steeringDir, truckWorldVel);
      // get the desired change in velocity (that would cancel sliding)
      float desiredVelChange = -steeringVel * stateMachine.TruckGripFactor;
      // get, in turn, the desired acceleration that would achieve the change of velocity we want in 1 tick
      float desirecAccel = desiredVelChange / Time.fixedDeltaTime;
      stateMachine.BoardRb.AddForceAtPosition(steeringDir * stateMachine.TruckMass * desirecAccel, truckTransform.position);
    }
  }

  protected void SetFriction() {
    if (stateMachine.Input.braking)
      stateMachine.PhysMat.dynamicFriction = stateMachine.BrakingFriction;
    else
      stateMachine.PhysMat.dynamicFriction = stateMachine.WheelFriction;

  }
}