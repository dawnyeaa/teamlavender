using UnityEngine;
using Unity.Mathematics;

using UnityEditor;
using RootMotion.FinalIK;

public abstract class SkateboardBaseState : State {
  protected readonly SkateboardStateMachine sm;
  protected float TurnSpeed = 0.0f;
  protected float VisFollowSpeed = 0.0f;

  protected SkateboardBaseState(SkateboardStateMachine stateMachine) {
    sm = stateMachine;
  }

  protected void BodyUprightCorrect() {
    float vertVelocity = Vector3.Dot(Vector3.up, sm.MainRB.velocity);
    bool goingDown = vertVelocity < sm.GoingDownThreshold;

    if (!sm.Grounded && vertVelocity <= 0) {
      sm.CharacterAnimator.SetBool("falling", true);
      float groundMatchDistance = (Time.fixedDeltaTime * 2f * -vertVelocity) + 1.5f;
      if (Physics.Raycast(sm.BodyMesh.position, Vector3.down, out RaycastHit hit, groundMatchDistance, LayerMask.GetMask("Ground"))) {
        sm.debugFrame.predictedLandingPosition = hit.point;
        Debug.DrawRay(sm.BodyMesh.position, Vector3.down * groundMatchDistance, Color.red);
        Vector3 normal = hit.normal;
        sm.Down = Vector3.Slerp(sm.Down, -normal, sm.RightingStrength);
        sm.footRepresentation.localPosition = sm.footRepresentation.localPosition.magnitude * sm.Down;

        sm.FacingParentRB.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(sm.BodyMesh.forward, sm.Down), -sm.Down);
        
        sm.BodyMesh.rotation = sm.FacingParentRB.rotation;
        sm.Board.rotation = sm.FacingParentRB.rotation;
        // sm.Board.localPosition = sm.Down * sm.BodyMesh.localPosition.magnitude;
      }
    }
    else {
      sm.CharacterAnimator.SetBool("falling", false);
    }

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

      sm.debugFrame.pointOfContact = hit.point;
      sm.debugFrame.contactNormal = hit.normal;

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

      // add a force to push the body away from the surface - stronger if closer (like buoyancy)
      sm.CurrentProjectLength = hit.distance;
      float compression = sm.ProjectLength - sm.CurrentProjectLength;

      sm.MainRB.AddForce((-sm.Down * (compression * sm.SpringConstant + Vector3.Dot(sm.Down, sm.MainRB.velocity) * sm.SpringDamping))*sm.SpringMultiplier);
      if (!sm.Grounded) {
        Vector3 flatMovement = Vector3.ProjectOnPlane(sm.MainRB.velocity, -sm.Down).normalized;
        if (flatMovement.magnitude > 0.2f && Mathf.Abs(Vector3.Dot(flatMovement, sm.FacingRB.transform.forward)) < sm.LandingAngleGive) {
          // sm.EnterDead();
        }
        else {
          sm.FacingRB.transform.localRotation = Quaternion.identity;
        }

        if (sm.AirTimeCounter > sm.MinimumAirTime) {
          sm.PointManager.Validate();
        }
      }
      sm.Grounded = true;
    }
    else {
      if (sm.Grounded) {
        sm.CharacterAnimator.SetTrigger("startAirborne");
        sm.AirTimeCounter = 0;
      }
      sm.Grounded = false;
    }
    if (!sm.Grounded) {
      sm.FacingRB.transform.localRotation = Quaternion.identity;
      if (sm.AirTimeCounter > sm.MinimumAirTime)
        sm.PointManager.AddPoints(Mathf.RoundToInt(Time.fixedDeltaTime*sm.PointsPerAirTimeSecond));
      sm.AirTimeCounter += Time.fixedDeltaTime;
    }
    sm.DampedDown = Vector3.Slerp(sm.DampedDown, sm.Down, 1f/Mathf.Pow(2, sm.BoardPositionDamping));
    sm.footRepresentation.localPosition = sm.DampedDown * sm.CurrentProjectLength;
    sm.Board.localPosition = sm.DampedDown * (sm.CurrentProjectLength + sm.ProjectRadius);
    sm.HeadSensZone.SetT(Mathf.Lerp(1-Mathf.Clamp01(Vector3.Dot(sm.DampedDown, Vector3.down)), sm.MainRB.velocity.magnitude/sm.MaxSpeed, sm.HeadZoneSpeedToHorizontalRatio));
  }

  protected void AdjustSpringMultiplier() {
    sm.SpringMultiplier = Mathf.Abs(Vector3.Dot(Vector3.up, -sm.Down));
    sm.SpringMultiplier = math.remap(0, 1, sm.SpringMultiplierMin, sm.SpringMultiplierMax, sm.SpringMultiplier);
  }

  protected void CalculateTurn() {
    if (sm.Grounded) {
      float turnTarget = sm.Input.turn * (1-(sm.MainRB.velocity.magnitude/sm.TurnLockSpeed));
      if (!sm.CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle")) turnTarget *= 1-sm.PushTurnReduction;
      sm.TruckTurnPercent = Mathf.SmoothDamp(sm.TruckTurnPercent, turnTarget, ref TurnSpeed, sm.TruckTurnDamping);
      sm.CharacterAnimator.SetFloat("leanValue", (sm.TruckTurnPercent*0.5f) + 0.5f);
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
        Vector3 truckWorldVel = sm.MainRB.GetPointVelocity(turnForcePosition)+sm.FacingRB.GetPointVelocity(turnForcePosition);
        // get the speed in the trucks steering direction (right-left)
        float steeringVel = Vector3.Dot(steeringDir, truckWorldVel);
        // get the desired change in velocity (that would cancel sliding)
        float desiredVelChange = -steeringVel * sm.TruckGripFactor;
        // get, in turn, the desired acceleration that would achieve the change of velocity we want in 1 tick
        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
        sm.FacingParentRB.rotation = Quaternion.LookRotation(truckOffset.normalized, -sm.DampedDown);
        sm.FacingRB.AddForceAtPosition(steeringDir * desiredAccel, turnForcePosition, ForceMode.Acceleration);
        sm.MainRB.AddForceAtPosition(steeringDir * desiredAccel, turnForcePosition, ForceMode.Acceleration);
      }
    }
    sm.BodyMesh.rotation = sm.FacingRB.transform.rotation;
    sm.Board.rotation = sm.FacingRB.transform.rotation;
  }

  protected void StartPush() {
    if (sm.Grounded) {
      if (sm.PushingAnim && !sm.PushBuffered) sm.PushBuffered = true;
      if (!sm.PushingAnim) {
        sm.PushingAnim = true;
        sm.CharacterAnimator.SetTrigger("push");
      }
    }
  }

  protected void OnSwitch() {
    if (sm.Grounded)
      sm.FacingParentRB.transform.rotation *= Quaternion.AngleAxis(180f, sm.FacingParentRB.transform.up);
  }

  protected void CalculatePush() {
    if (sm.Pushing && sm.CurrentPushT > Mathf.Epsilon) {
      if (sm.Grounded && Vector3.Angle(Vector3.down, sm.Down) < sm.PushingMaxSlope && !sm.Crouching) {
        // we're pushing
        float t = 1-(sm.CurrentPushT/sm.MaxPushT);
        sm.MainRB.AddForce(sm.PushForce * sm.PushForceCurve.Evaluate(t) * sm.FacingRB.transform.forward, ForceMode.Acceleration);
        sm.CurrentPushT -= Time.fixedDeltaTime;
        if (sm.CurrentPushT < 0) {
          if (sm.PushBuffered) {
            sm.PlayingBufferedPush = true;
            sm.PushBuffered = false;
            sm.CharacterAnimator.SetTrigger("push");
          }
          else {
            sm.PlayingBufferedPush = false;
          }
        }
      }
      else {
        sm.Pushing = false;
        sm.PushingAnim = false;
      }
    }
    else {
      sm.Pushing = false;
    }
  }
  
  protected void SetFriction() {
    if (sm.Input.braking)
      sm.PhysMat.dynamicFriction = sm.BrakingFriction;
    else
      sm.PhysMat.dynamicFriction = sm.WheelFriction;
  }

  protected void CapSpeed() {
    if (sm.MainRB.velocity.magnitude > sm.MaxSpeed)
      sm.MainRB.velocity = sm.MainRB.velocity.normalized * sm.MaxSpeed;
  }

  protected void SetCrouching() {
    // if input is crouching, we're crouching (and the recover timer should be reset)
    // if input is not crouching
      // if we've just released it, start the crouch recover timer, and keep crouching
      // if the timer is running, then decrement the timer, and keep crouching
      // if the timer is expired, then stop crouching
    bool localCrouching;
    if (sm.Input.crouching) {
      localCrouching = true;
      sm.UncrouchDelayTimer = sm.UncrouchDelayTime;
    }
    else {
      if (sm.UncrouchDelayTimer > 0) {
        localCrouching = true;
        sm.UncrouchDelayTimer -= Time.fixedDeltaTime;
      }
      else {
        localCrouching = false;
      }
    }
    sm.Crouching = localCrouching && sm.Grounded;
    sm.CharacterAnimator.SetBool("crouching", sm.Crouching);
  }

  protected void OnOllieInput() {
    if (sm.Crouching) {
      sm.CharacterAnimator.SetTrigger("ollie");
    }
  }

  protected void OnKickflipInput() {
    if (sm.Crouching) {
      sm.CharacterAnimator.SetTrigger("kickflip");
    }
  }

  protected void ApplyFrictionForce() {
    Vector3 forwardVelocity = Vector3.Project(sm.MainRB.velocity, sm.FacingRB.transform.forward);
    float frictionMag = sm.PhysMat.dynamicFriction * forwardVelocity.magnitude;
    sm.MainRB.AddForce(-forwardVelocity.normalized*frictionMag, ForceMode.Acceleration);
  }

  protected void StartBrake() {
    if (sm.PlayingBufferedPush) {
      sm.PushingAnim = false;
      sm.PushBuffered = false;
    }
    sm.CharacterAnimator.SetBool("stopping", true);
    if (sm.MainRB.velocity.magnitude > 1f) {
      sm.CharacterAnimator.SetBool("hardStop", true);
    }
    else {
      sm.CharacterAnimator.SetBool("hardStop", false);
    }
  }

  protected void EndBrake() {
    sm.Input.braking = false;
    sm.CharacterAnimator.SetBool("stopping", false);
  }

  public void Spawn() {
    // find the nearest spawn point
    (Vector3 pos, Quaternion rot) = sm.SpawnPointManager.GetNearestSpawnPoint(sm.transform.position);
    // reset any necessary properties
    sm.MainRB.velocity = Vector3.zero;
    sm.FacingRB.velocity = Vector3.zero;
    sm.MainRB.angularVelocity = Vector3.zero;
    sm.FacingRB.angularVelocity = Vector3.zero;

    sm.FacingParentRB.transform.localRotation = Quaternion.identity;
    sm.FacingRB.MoveRotation(Quaternion.identity);

    sm.Down = Vector3.down;
    sm.DampedDown = Vector3.down;
    sm.Grounded = false;
    sm.TruckTurnPercent = 0;
    sm.HeadSensZone.SetT(0);
    // move to that nearest spawn point;
    sm.MainRB.MovePosition(pos);
    sm.FacingParentRB.transform.rotation = rot;
  }

  protected void CreateDebugFrame() {
    sm.debugFrame = new() {
      centerOfMass = Vector3.zero,
      pointOfContact = Vector3.zero,
      predictedLandingPosition = Vector3.zero,
      downVector = Vector3.zero,
      dampedDownVector = Vector3.zero,
      contactNormal = Vector3.zero,
    };
  }

  protected void SaveDebugFrame() {
    sm.debugFrame.centerOfMass = sm.transform.position;
    sm.debugFrame.downVector = sm.Down;
    sm.debugFrame.dampedDownVector = sm.DampedDown;

    sm.DebugFrameHandler.PutFrame(sm.debugFrame);
  }
}