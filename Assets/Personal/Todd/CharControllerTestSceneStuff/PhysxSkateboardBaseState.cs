using UnityEngine;
using UnityEditor;

public abstract class PhysxSkateboardBaseState : State {
  protected readonly PhysxSkateboardStateMachine stateMachine;
  protected float TurnSpeed = 0.0f;

  protected PhysxSkateboardBaseState(PhysxSkateboardStateMachine stateMachine) {
    this.stateMachine = stateMachine;
  }

  protected void BeginPush() {
    stateMachine.PhysxBody.AddForce(stateMachine.transform.forward * stateMachine.PushStrength);
  }

  protected void CheckGrounded() {
    bool frontOnGround = Physics.Raycast(stateMachine.FrontGroundedChecker.position, -stateMachine.transform.up, stateMachine.GroundedCheckRaycastDistance, LayerMask.GetMask("Ground"));
    bool backOnGround = Physics.Raycast(stateMachine.BackGroundedChecker.position, -stateMachine.transform.up, stateMachine.GroundedCheckRaycastDistance, LayerMask.GetMask("Ground"));
    Debug.DrawRay(stateMachine.FrontGroundedChecker.position, -stateMachine.transform.up, Color.blue);
    Debug.Log("front: " + frontOnGround + ", back: " + backOnGround);
    stateMachine.Grounded = frontOnGround && backOnGround;
  }

  protected void MatchGround() {
    // WE'RE WORKIN AROUND HERE
    if (stateMachine.Grounded) {
      // match character controller's angle to ground (snaps)
      Vector3 projectOrigin = stateMachine.transform.position + stateMachine.transform.up * stateMachine.GroundAngleRaycastOffset;
      RaycastHit hit;
      //, stateMachine.GroundAngleRaycastDistance
      bool itHit = Physics.Raycast(projectOrigin, -stateMachine.transform.up, out hit, LayerMask.GetMask("Ground"));
      if (itHit) {
        Vector3 newNormal = hit.normal;
        Vector3 newRight = Vector3.Cross(newNormal, stateMachine.transform.forward);
        Vector3 newForward = Vector3.Cross(newRight, newNormal);
        // var rot = Quaternion.FromToRotation(stateMachine.transform.up, newNormal);
        // stateMachine.PhysxBody.AddTorque(new Vector3(rot.x, rot.y, rot.z)*10);

        stateMachine.transform.rotation = Quaternion.LookRotation(newForward, newNormal);

        // smooth boards visual transform to not snap
        // Vector3 frontPos = Vector3.zero;
        // Vector3 backPos = Vector3.zero;
        // bool frontHit = Physics.Raycast(stateMachine.FrontGroundedChecker.position, -stateMachine.transform.up, out hit, LayerMask.GetMask("Ground"));
        // if (frontHit) {
        //   frontPos = hit.point;
        // }
        
        // bool backHit = Physics.Raycast(stateMachine.BackGroundedChecker.position, -stateMachine.transform.up, out hit, LayerMask.GetMask("Ground"));
        // if (backHit) {
        //   backPos = hit.point;
        // }

        // if (frontHit && backHit) {
        //   Vector3 newFacing = frontPos - backPos;
        //   stateMachine.SkateboardMeshTransform.rotation = Quaternion.LookRotation(newFacing, newNormal);
        // }
      }
    }
  }

  protected void SetFriction() {
    if (stateMachine.Input.braking) {
      stateMachine.PhysxMat.dynamicFriction = stateMachine.BrakingFriction;
    }
    else
      stateMachine.PhysxMat.dynamicFriction = stateMachine.WheelFriction;
  }

  protected void CalculateTurn() {
    Vector3 forwardVelocity = Vector3.Project(stateMachine.PhysxBody.velocity, stateMachine.transform.forward);
    if (forwardVelocity.magnitude > Mathf.Epsilon && stateMachine.Grounded) {
      stateMachine.Turning = Mathf.SmoothDamp(stateMachine.Turning, stateMachine.Input.turn, ref TurnSpeed, stateMachine.TurnSpeedDamping);
      // stateMachine.transform.rotation *= Quaternion.AngleAxis(stateMachine.Turning*stateMachine.MaxTruckTurnDeg, stateMachine.transform.up);
      float deltaPos = Time.fixedDeltaTime*forwardVelocity.magnitude;
      float rad = stateMachine.TruckSpacing/Mathf.Sin(Mathf.Deg2Rad*stateMachine.Turning*stateMachine.MaxTruckTurnDeg);
      float circum = ThisIsJustTau.TAU*rad;
      Quaternion turny = Quaternion.AngleAxis((deltaPos/circum)*360f, stateMachine.transform.up);
      stateMachine.transform.rotation *= turny;
    }
  }

  protected void AddTurningFriction() {
    // all this shit down here fucking sucks
    // fuck u jack

    Vector3 flatVelocity = Vector3.ProjectOnPlane(stateMachine.PhysxBody.velocity, stateMachine.transform.up);
    Vector3 forwardVelocity = Vector3.Project(flatVelocity, stateMachine.transform.forward);
    if (stateMachine.Grounded) {
      stateMachine.PhysxBody.velocity = forwardVelocity;
    }
  }
}