using UnityEngine;
using Unity.Mathematics;

public abstract class Skateboard4BaseState : State {
  protected readonly Skateboard4StateMachine sm;
  protected float TurnSpeed = 0.0f;
  // protected Vector3 RayTurnSpeed = Vector3.zero;

  protected Skateboard4BaseState(Skateboard4StateMachine stateMachine) {
    this.sm = stateMachine;
  }

  protected void BodyUprightCorrect() {
    // float bodyFloorPivotRatio = 0.5f;
    // Vector3 pivotPoint = Vector3.Lerp(sm.transform.position, sm.footRepresentation.position, bodyFloorPivotRatio);

    // Vector3 inwardRadiusVector = pivotPoint - sm.transform.position;

    // Vector3 relativeTargetPosition = (pivotPoint + Vector3.up*inwardRadiusVector.magnitude) - sm.transform.position;

    // Vector3 rotationAxis = Vector3.Cross(relativeTargetPosition.normalized, inwardRadiusVector.normalized);
    // Vector3 forceTangent = Vector3.Cross(inwardRadiusVector.normalized, rotationAxis);

    // Vector3 currentUp = sm.transform.TransformDirection(-sm.Down);
    // Debug.DrawRay(sm.transform.position, currentUp, Color.red);
    // Debug.Log(Vector3.Dot(currentUp, Vector3.up));
    // float uprightAlignment = math.remap(-1, 1, 0, 1, Vector3.Dot(currentUp, Vector3.up));

    // Vector3 forceVector = forceTangent * ((1-uprightAlignment) * sm.RightingStrength - );

    // Vector3 bodyCounterForcePoint = pivotPoint + inwardRadiusVector;
    // sm.alignPivot.position = pivotPoint;
    
    // sm.BoardRb.AddForceAtPosition(forceVector, sm.transform.position, ForceMode.Acceleration);
    // sm.BoardRb.AddForceAtPosition(-forceVector, bodyCounterForcePoint, ForceMode.Acceleration);
    bool goingDown = Vector3.Dot(Vector3.up, sm.BoardRb.velocity) < sm.GoingDownThreshold;

    if (goingDown) {
      sm.Down = Vector3.Slerp(sm.Down, Vector3.down, sm.RightingStrength);
      sm.footRepresentation.localPosition = sm.footRepresentation.localPosition.magnitude * sm.Down;
    }
  }

  protected void VertBodySpring() {
    // set sphere collision visualiser size
    sm.footRepresentation.localScale = new Vector3(sm.ProjectRadius, sm.ProjectRadius, sm.ProjectRadius) * 2f;

    // sphere cast from body down - sphere does not need to be the same radius as the collider
    RaycastHit hit;
    if (Physics.SphereCast(sm.transform.position, sm.ProjectRadius, sm.Down, out hit, sm.ProjectLength, LayerMask.GetMask("Ground"))) {
      // // if (sm.BoardRb.velocity.magnitude < sm.EdgeSafeSpeedEpsilon) {
      // if (Vector3.Angle(-sm.Down, hit.normal.normalized) > sm.EdgeSafeAngle) {
      //   return;
      // }
      // // }

      // body is now magnetised to surface (match angle)
      // sm.Down = -hit.normal.normalized;
      // sm.Down = Vector3.SmoothDamp(sm.Down, Vector3.down, ref RayTurnSpeed, 1f);
      // these both have problems lmao
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
      float length = hit.distance;
      float compression = sm.ProjectLength - length;

      sm.BoardRb.AddForce((-sm.Down * (compression * sm.SpringConstant + Vector3.Dot(sm.Down, sm.BoardRb.velocity) * sm.SpringDamping))*sm.SpringMultiplier);

      // set the sphere collision visualiser where it collides 
      sm.footRepresentation.localPosition = sm.Down * hit.distance;
      // sm.footRepresentation.position = hit.point;
      return;
    }
    
    // the visualiser will be positioned at the max length if it hits nothing
    sm.footRepresentation.localPosition = sm.Down * sm.ProjectLength;
  }

  protected void AdjustSpringConstant() {
    // sm.SpringConstant = Mathf.Lerp(sm.CrouchingSpringConstant, sm.DefaultSpringConstant, Mathf.Abs(Vector3.Dot(Vector3.up, -sm.Down)));
    // sm.SpringConstant = Mathf.Lerp(sm.CrouchingSpringConstant, sm.DefaultSpringConstant, Mathf.Abs(Vector3.Dot(Vector3.up, sm.BoardRb.velocity)));
    sm.SpringMultiplier = Mathf.Abs(Vector3.Dot(Vector3.up, -sm.Down));
    sm.SpringMultiplier = math.remap(0, 1, sm.SpringMultiplierMin, sm.SpringMultiplierMax, sm.SpringMultiplier);
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