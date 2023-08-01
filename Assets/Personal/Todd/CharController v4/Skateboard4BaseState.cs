using UnityEngine;

public abstract class Skateboard4BaseState : State {
  protected readonly Skateboard4StateMachine sm;
  protected float TurnSpeed = 0.0f;

  protected Skateboard4BaseState(Skateboard4StateMachine stateMachine) {
    this.sm = stateMachine;
  }

  protected void VertBodySpring() {
    // set sphere collision visualiser size
    sm.footRepresentation.localScale = new Vector3(sm.ProjectRadius, sm.ProjectRadius, sm.ProjectRadius) * 2f;

    // sphere cast from body down - sphere does not need to be the same radius as the collider
    RaycastHit hit;
    if (Physics.SphereCast(sm.transform.position, sm.ProjectRadius, sm.Down, out hit, sm.ProjectLength, LayerMask.GetMask("Ground"))) {
      // body is now magnetised to surface (match angle)
      sm.Down = -hit.normal;

      // add a force to push the body away from the surface - stronger if closer (like buoyancy)
      float length = hit.distance;
      float compression = sm.ProjectLength - length;
      sm.BoardRb.AddForce(-sm.Down * (compression * sm.SpringConstant + Vector3.Dot(sm.Down, sm.BoardRb.velocity) * sm.SpringDamping));

      // set the sphere collision visualiser where it collides 
      sm.footRepresentation.localPosition = sm.Down * hit.distance;
      // sm.footRepresentation.position = hit.point;
    }
    else {
      // the visualiser will be positioned at the max length if it hits nothing
      sm.footRepresentation.localPosition = sm.Down * sm.ProjectLength;
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