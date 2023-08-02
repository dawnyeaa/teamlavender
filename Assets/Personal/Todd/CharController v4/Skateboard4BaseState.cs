using UnityEngine;
using Unity.Mathematics;

public abstract class Skateboard4BaseState : State {
  protected readonly Skateboard4StateMachine sm;
  protected float TurnSpeed = 0.0f;

  protected Skateboard4BaseState(Skateboard4StateMachine stateMachine) {
    this.sm = stateMachine;
  }

  protected void BodyUprightCorrect() {
    bool goingDown = Vector3.Dot(Vector3.up, sm.BoardRb.velocity) < sm.GoingDownThreshold;

    if (goingDown) {
      sm.Down = Vector3.Slerp(sm.Down, Vector3.down, sm.RightingStrength);
      sm.footRepresentation.localPosition = sm.footRepresentation.localPosition.magnitude * sm.Down;
    }
  }

  protected void VertBodySpring() {
    // set sphere collision visualiser size
    sm.footRepresentation.localScale = new Vector3(sm.ProjectRadius, sm.ProjectRadius, sm.ProjectRadius) * 2f;

    sm.CurrentProjectLength = sm.ProjectLength;

    // sphere cast from body down - sphere does not need to be the same radius as the collider
    RaycastHit hit;
    if (Physics.SphereCast(sm.transform.position, sm.ProjectRadius, sm.Down, out hit, sm.ProjectLength, LayerMask.GetMask("Ground"))) {
      Vector3 truckRelative = Vector3.Cross(sm.Down, Vector3.Cross(sm.transform.forward, sm.Down)).normalized*sm.TruckSpacing;

      RaycastHit rayHit;
      Vector3 frontHitPos = Vector3.zero, backHitPos = Vector3.zero;
      float bonusDistance = 0.3f;

      bool frontHit = Physics.Raycast(sm.transform.position + truckRelative, sm.Down, out rayHit, sm.ProjectLength + bonusDistance, LayerMask.GetMask("Ground"));
      if (frontHit) {
        // front truck hit!!!
        frontHitPos = rayHit.point;
        bool backHit = Physics.Raycast(sm.transform.position - truckRelative, sm.Down, out rayHit, sm.ProjectLength + bonusDistance, LayerMask.GetMask("Ground"));
        if (backHit) {
          // back truck hit!!!
          backHitPos = rayHit.point;

          var newForward = backHitPos - frontHitPos;
          var newNormal = Vector3.Cross(newForward, Vector3.Cross(newForward, sm.Down)).normalized;
          sm.Down = -newNormal;
        }
      }

      // add a force to push the body away from the surface - stronger if closer (like buoyancy)
      sm.CurrentProjectLength = hit.distance;
      float compression = sm.ProjectLength - sm.CurrentProjectLength;

      sm.BoardRb.AddForce((-sm.Down * (compression * sm.SpringConstant + Vector3.Dot(sm.Down, sm.BoardRb.velocity) * sm.SpringDamping))*sm.SpringMultiplier);
    }
    
    // the visualiser will be positioned at the max length if it hits nothing
    sm.footRepresentation.localPosition = sm.Down * sm.CurrentProjectLength;
  }

  protected void AdjustSpringMultiplier() {
    sm.SpringMultiplier = Mathf.Abs(Vector3.Dot(Vector3.up, -sm.Down));
    sm.SpringMultiplier = math.remap(0, 1, sm.SpringMultiplierMin, sm.SpringMultiplierMax, sm.SpringMultiplier);
  }

  protected void CalculateTurn() {
    sm.TruckTurnPercent = Mathf.SmoothDamp(sm.TruckTurnPercent, sm.Input.turn, ref TurnSpeed, sm.TruckTurnDamping);
    float localTruckTurnPercent = sm.TurningEase.Evaluate(Mathf.Abs(sm.TruckTurnPercent))*Mathf.Sign(sm.TruckTurnPercent);
    for (int i = 0; i < 2; ++i) {
      Transform truckTransform = i == 0 ? sm.frontAxis : sm.backAxis;
      Vector3 newLeft = Vector3.Cross(sm.transform.forward, sm.Down);
      Vector3 truckOffset = Vector3.Cross(sm.Down, newLeft) * sm.TruckSpacing;
      Vector3 turnForceApplicationY = sm.transform.position + (sm.CurrentProjectLength * (1-sm.TurnForceApplicationHeight) * sm.Down);
      Debug.DrawRay(turnForceApplicationY, truckOffset, Color.magenta);
      Vector3 turnForcePosition = turnForceApplicationY + truckOffset * (i == 0 ? 1 : -1);
      truckTransform.position = turnForcePosition;
      truckTransform.rotation = Quaternion.LookRotation(truckOffset, -sm.Down);
      localTruckTurnPercent *= (i == 0 ? 1 : -1);
      // get the new forward for the truck
      Vector3 accelDir = Quaternion.AngleAxis(localTruckTurnPercent*sm.MaxTruckTurnDeg, truckTransform.up) * truckOffset.normalized;
      // rotate the truck transforms - can easily see from debug axes
      truckTransform.rotation = Quaternion.LookRotation(accelDir, truckTransform.up);
      // get the truck's steering axis (right-left)
      Vector3 steeringDir = Quaternion.AngleAxis(localTruckTurnPercent*sm.MaxTruckTurnDeg, truckTransform.up) * -newLeft * (i == 0 ? 1 : -1);
      // get the current velocity of the truck
      Vector3 truckWorldVel = sm.BoardRb.GetPointVelocity(turnForcePosition);
      // get the speed in the trucks steering direction (right-left)
      float steeringVel = Vector3.Dot(steeringDir, truckWorldVel);
      // get the desired change in velocity (that would cancel sliding)
      float desiredVelChange = -steeringVel * sm.TruckGripFactor;
      // get, in turn, the desired acceleration that would achieve the change of velocity we want in 1 tick
      float desirecAccel = desiredVelChange / Time.fixedDeltaTime;
      sm.BoardRb.AddForceAtPosition(steeringDir * desirecAccel, turnForcePosition, ForceMode.Acceleration);
    }
  }

  protected void OnPush() {
    sm.BoardRb.AddForce(sm.transform.forward * sm.PushForce, ForceMode.Acceleration);
  }
  
  protected void SetFriction() {
    if (sm.Input.braking)
      sm.PhysMat.dynamicFriction = sm.BrakingFriction;
    else
      sm.PhysMat.dynamicFriction = sm.WheelFriction;
  }

  protected void ApplyFrictionForce() {
    Vector3 slidingVelocity = Vector3.ProjectOnPlane(sm.BoardRb.velocity, sm.Down);
    float frictionMag = sm.PhysMat.dynamicFriction * Physics.gravity.magnitude;
    sm.BoardRb.AddForce(-slidingVelocity.normalized*frictionMag, ForceMode.Acceleration);
  }
}