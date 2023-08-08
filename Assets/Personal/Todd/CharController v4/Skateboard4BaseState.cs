using UnityEngine;
using Unity.Mathematics;

using UnityEditor;

public abstract class Skateboard4BaseState : State {
  protected readonly Skateboard4StateMachine sm;
  protected float TurnSpeed = 0.0f;
  protected float VisFollowSpeed = 0.0f;

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
    if (Physics.SphereCast(sm.transform.position, sm.ProjectRadius, sm.Down, out RaycastHit hit, sm.ProjectLength, LayerMask.GetMask("Ground"))) {
      sm.Grounded = true;
      Vector3 truckRelative = Vector3.Cross(sm.Down, Vector3.Cross(sm.FacingRB.transform.forward, sm.Down)).normalized*sm.TruckSpacing;

      Vector3 frontHitPos, backHitPos;
      Vector3 tempNormal;
      float bonusDistance = 0.25f;

      Debug.DrawRay(sm.transform.position + truckRelative, sm.Down*(sm.ProjectLength + bonusDistance), Color.white);
      Debug.DrawRay(sm.transform.position - truckRelative, sm.Down*(sm.ProjectLength + bonusDistance), Color.white);

      bool frontHit = Physics.Raycast(sm.transform.position + truckRelative, sm.Down, out RaycastHit rayHit, sm.ProjectLength + bonusDistance, LayerMask.GetMask("Ground"));
      if (frontHit) {
        // front truck hit!!!
        frontHitPos = rayHit.point;
        tempNormal = rayHit.normal;
        bool backHit = Physics.Raycast(sm.transform.position - truckRelative, sm.Down, out rayHit, sm.ProjectLength + bonusDistance, LayerMask.GetMask("Ground"));
        if (backHit) {
          // back truck hit!!!
          backHitPos = rayHit.point;

          tempNormal = (tempNormal + rayHit.normal)/2f;

          var newForward = backHitPos - frontHitPos;
          var newNormal = Vector3.Cross(newForward, Vector3.Cross(newForward, -tempNormal)).normalized;
          sm.Down = -newNormal;
        }
      }
      else {
        sm.Grounded = false;
      }

      // add a force to push the body away from the surface - stronger if closer (like buoyancy)
      sm.CurrentProjectLength = hit.distance;
      float compression = sm.ProjectLength - sm.CurrentProjectLength;

      sm.BoardRb.AddForce((-sm.Down * (compression * sm.SpringConstant + Vector3.Dot(sm.Down, sm.BoardRb.velocity) * sm.SpringDamping))*sm.SpringMultiplier);
    }
    else {
      sm.Grounded = false;
    }
    sm.DampedDown = Vector3.Slerp(sm.DampedDown, sm.Down, 1f/Mathf.Pow(2, sm.BoardPositionDamping));
    sm.footRepresentation.localPosition = sm.DampedDown * sm.CurrentProjectLength;
    sm.BodyMesh.localPosition = sm.DampedDown * (sm.CurrentProjectLength + sm.ProjectRadius);
    sm.HeadSensZone.SetT(Mathf.Lerp(1-Mathf.Clamp01(Vector3.Dot(sm.DampedDown, Vector3.down)), sm.BoardRb.velocity.magnitude/sm.MaxSpeed, sm.HeadZoneSpeedToHorizontalRatio));
    sm.HeadSensZone.SetShow(sm.ShowHeadZone);
  }

  protected void AdjustSpringMultiplier() {
    sm.SpringMultiplier = Mathf.Abs(Vector3.Dot(Vector3.up, -sm.Down));
    sm.SpringMultiplier = math.remap(0, 1, sm.SpringMultiplierMin, sm.SpringMultiplierMax, sm.SpringMultiplier);
  }

  protected void CalculateTurn() {
    if (sm.Grounded) {
      float turnTarget = sm.Input.turn * (1-(sm.BoardRb.velocity.magnitude/sm.TurnLockSpeed));
      sm.TruckTurnPercent = Mathf.SmoothDamp(sm.TruckTurnPercent, turnTarget, ref TurnSpeed, sm.TruckTurnDamping);
      float localTruckTurnPercent = sm.TurningEase.Evaluate(Mathf.Abs(sm.TruckTurnPercent))*Mathf.Sign(sm.TruckTurnPercent);
      for (int i = 0; i < 2; ++i) {
        Transform truckTransform = i == 0 ? sm.frontAxis : sm.backAxis;
        Vector3 newLeft = Vector3.Cross(sm.FacingRB.transform.forward, sm.Down);
        Vector3 truckOffset = Vector3.Cross(sm.Down, newLeft) * sm.TruckSpacing;
        Vector3 turnForcePosition = sm.FacingRB.position + truckOffset * (i == 0 ? 1 : -1);
        truckTransform.SetPositionAndRotation(turnForcePosition, Quaternion.LookRotation(truckOffset, -sm.Down));
        localTruckTurnPercent *= i == 0 ? 1 : -1;
        // get the new forward for the truck
        Vector3 accelDir = Quaternion.AngleAxis(localTruckTurnPercent*sm.MaxTruckTurnDeg, truckTransform.up) * truckOffset.normalized;
        // rotate the truck transforms - can easily see from debug axes
        truckTransform.rotation = Quaternion.LookRotation(accelDir, truckTransform.up);
        // get the truck's steering axis (right-left)
        Vector3 steeringDir = Quaternion.AngleAxis(localTruckTurnPercent*sm.MaxTruckTurnDeg, truckTransform.up) * -newLeft * (i == 0 ? 1 : -1);
        // get the current velocity of the truck
        Vector3 truckWorldVel = sm.BoardRb.GetPointVelocity(turnForcePosition)+sm.FacingRB.GetPointVelocity(turnForcePosition);
        // get the speed in the trucks steering direction (right-left)
        float steeringVel = Vector3.Dot(steeringDir, truckWorldVel);
        // get the desired change in velocity (that would cancel sliding)
        float desiredVelChange = -steeringVel * sm.TruckGripFactor;
        // get, in turn, the desired acceleration that would achieve the change of velocity we want in 1 tick
        float desirecAccel = desiredVelChange / Time.fixedDeltaTime;
        sm.FacingParentRB.rotation = Quaternion.LookRotation(truckOffset.normalized, -sm.DampedDown);
        sm.FacingRB.AddForceAtPosition(steeringDir * desirecAccel, turnForcePosition, ForceMode.Acceleration);
        sm.BoardRb.AddForceAtPosition(steeringDir * desirecAccel, turnForcePosition, ForceMode.Acceleration);
        sm.BodyMesh.rotation = sm.FacingRB.transform.rotation;
      }
    }
  }

  protected void StartPush() {
    if (sm.CurrentPushT <= Mathf.Epsilon) {
      // we can start a new push
      sm.CurrentPushT = 1;
    }
  }

  protected void OnSwitch() {
    if (sm.Grounded)
      sm.FacingParentRB.transform.rotation *= Quaternion.AngleAxis(180f, sm.FacingParentRB.transform.up);
  }

  protected void CalculatePush() {
    if (sm.CurrentPushT > Mathf.Epsilon) {
      if (sm.Grounded && Vector3.Angle(Vector3.down, sm.Down) < sm.PushingMaxSlope) {
        // we're pushing
        float t = 1-sm.CurrentPushT;
        sm.BoardRb.AddForce(sm.PushForce * sm.PushForceCurve.Evaluate(t) * sm.FacingRB.transform.forward, ForceMode.Acceleration);
        sm.CurrentPushT -= Time.fixedDeltaTime / sm.MaxPushDuration;
      }
      else {
        sm.CurrentPushT = 0;
      }
    }
  }
  
  protected void SetFriction() {
    if (sm.Input.braking)
      sm.PhysMat.dynamicFriction = sm.BrakingFriction;
    else
      sm.PhysMat.dynamicFriction = sm.WheelFriction;
  }

  protected void CapSpeed() {
    if (sm.BoardRb.velocity.magnitude > sm.MaxSpeed)
      sm.BoardRb.velocity = sm.BoardRb.velocity.normalized * sm.MaxSpeed;
  }

  protected void OnOllie() {
    if (sm.Grounded)
      sm.BoardRb.AddForce((Vector3.up - sm.Down).normalized*sm.OllieForce, ForceMode.Acceleration);
  }

  protected void ApplyFrictionForce() {
    Vector3 forwardVelocity = Vector3.Project(sm.BoardRb.velocity, sm.FacingRB.transform.forward);
    float frictionMag = sm.PhysMat.dynamicFriction * forwardVelocity.magnitude;
    sm.BoardRb.AddForce(-forwardVelocity.normalized*frictionMag, ForceMode.Acceleration);
  }

  public void Spawn() {
    // find the nearest spawn point
    (Vector3 pos, Quaternion rot) = sm.SpawnPointManager.GetNearestSpawnPoint(sm.transform.position);
    // reset any necessary properties
    sm.BoardRb.velocity = Vector3.zero;
    sm.FacingRB.velocity = Vector3.zero;
    sm.BoardRb.angularVelocity = Vector3.zero;
    sm.FacingRB.angularVelocity = Vector3.zero;

    sm.FacingParentRB.rotation = Quaternion.identity;
    sm.FacingRB.MoveRotation(Quaternion.identity);

    sm.Down = Vector3.down;
    sm.DampedDown = Vector3.down;
    sm.Grounded = false;
    sm.TruckTurnPercent = 0;
    sm.HeadSensZone.SetT(0);
    // move to that nearest spawn point;
    sm.BoardRb.MovePosition(pos);
    sm.FacingParentRB.rotation = rot;
  }
}