using UnityEngine;
using Unity.Mathematics;

using UnityEditor;
using RootMotion.FinalIK;

public abstract class SkateboardBaseState : State {
  protected readonly SkateboardStateMachine sm;
  protected float TurnSpeed = 0.0f;
  protected float VisFollowSpeed = 0.0f;
  protected Vector3 boardRailSnapVel = Vector3.zero;

  protected SkateboardBaseState(SkateboardStateMachine stateMachine) {
    sm = stateMachine;
  }

  protected void BodyUprightCorrect() {
    float vertVelocity = Vector3.Dot(Vector3.up, sm.MainRB.velocity);
    bool goingDown = vertVelocity < sm.GoingDownThreshold;

    if (!sm.Grounded && vertVelocity <= 0) {
      sm.CharacterAnimator.SetBool("falling", true);
      sm.CharacterAnimator.SetInteger("ollieTrickIndex", 0);
      float groundMatchDistance = (Time.fixedDeltaTime * 2f * -vertVelocity) + 1.5f;
      if (Physics.Raycast(sm.BodyMesh.position, Vector3.down, out RaycastHit hit, groundMatchDistance, LayerMask.GetMask("Ground"))) {
        sm.debugFrame.predictedLandingPosition = hit.point;
        Debug.DrawRay(sm.BodyMesh.position, Vector3.down * groundMatchDistance, Color.red);
        Vector3 normal = hit.normal;
        sm.Down = Vector3.Slerp(sm.Down, -normal, sm.RightingStrength);
        sm.footRepresentation.localPosition = sm.footRepresentation.localPosition.magnitude * sm.Down;

        // sm.FacingParent.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(sm.BodyMesh.forward, sm.Down), -sm.Down);
        sm.FacingParent.rotation = Quaternion.identity;
        
        sm.BodyMesh.rotation = sm.Facing.transform.rotation;
        sm.Board.rotation = sm.Facing.transform.rotation;
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

    // this was the old spherecast that *mostly* worked
    // Physics.SphereCast(sm.transform.position, sm.ProjectRadius, sm.Down, out RaycastHit hit, sm.ProjectLength, LayerMask.GetMask("Ground"))

    // sphere cast from body down - sphere does not need to be the same radius as the collider
    if (Physics.BoxCast(sm.transform.position, Vector3.one * sm.ProjectRadius, sm.Down, out RaycastHit hit, Quaternion.identity, sm.ProjectLength, LayerMask.GetMask("Ground"))) {

      sm.debugFrame.pointOfContact = hit.point;
      sm.debugFrame.contactNormal = hit.normal;

      Vector3 truckRelative = Vector3.Cross(sm.Down, Vector3.Cross(sm.Facing.transform.forward, sm.Down)).normalized*sm.TruckSpacing;

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

      sm.MainRB.AddForce((-sm.Down * (compression * sm.SpringConstant + Vector3.Dot(sm.Down, sm.MainRB.velocity) * sm.SpringDamping))*sm.SpringMultiplier, ForceMode.Acceleration);
      if (!sm.Grounded) {
        Vector3 flatMovement = Vector3.ProjectOnPlane(sm.MainRB.velocity, -sm.Down).normalized;
        if (flatMovement.magnitude > 0.2f && Mathf.Abs(Vector3.Dot(flatMovement, sm.Facing.transform.forward)) < sm.LandingAngleGive) {
          // sm.EnterDead();
        }
        else {
        }

        if (sm.AirTimeCounter > sm.MinimumAirTime) {
          sm.PointManager.Validate();
        }

        sm.LandEmit.Play();
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
        Vector3 newLeft = Vector3.Cross(sm.Facing.transform.forward, sm.Down);
        Vector3 truckOffset = Vector3.Cross(sm.Down, newLeft) * sm.TruckSpacing;
        Vector3 turnForcePosition = sm.Facing.transform.position + truckOffset * (i == 0 ? 1 : -1);
        truckTransform.SetPositionAndRotation(turnForcePosition, Quaternion.LookRotation(truckOffset, -sm.Down));
        localTruckTurnPercent *= i == 0 ? 1 : -1;
        // get the new forward for the truck
        Vector3 accelDir = Quaternion.AngleAxis(localTruckTurnPercent*sm.MaxTruckTurnDeg, truckTransform.up) * truckOffset.normalized;
        // rotate the truck transforms - can easily see from debug axes
        truckTransform.rotation = Quaternion.LookRotation(accelDir, truckTransform.up);
        // get the truck's steering axis (right-left)
        Vector3 steeringDir = Quaternion.AngleAxis(localTruckTurnPercent*sm.MaxTruckTurnDeg, truckTransform.up) * -newLeft * (i == 0 ? 1 : -1);
        // get the current velocity of the truck
        Vector3 truckWorldVel = sm.MainRB.GetPointVelocity(turnForcePosition) + sm.Facing.GetPointVelocity(turnForcePosition);
        // get the speed in the trucks steering direction (right-left)
        float steeringVel = Vector3.Dot(steeringDir, truckWorldVel);
        // get the desired change in velocity (that would cancel sliding)
        float desiredVelChange = -steeringVel * sm.TruckGripFactor;
        // get, in turn, the desired acceleration that would achieve the change of velocity we want in 1 tick
        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
        sm.FacingParent.rotation = Quaternion.LookRotation(Vector3.Cross(sm.DampedDown, Vector3.Cross(sm.MainRB.transform.forward, sm.DampedDown)), -sm.DampedDown);
        sm.Facing.AddForceAtPosition(steeringDir * desiredAccel, turnForcePosition);
        sm.MainRB.AddForceAtPosition(steeringDir * desiredAccel, turnForcePosition, ForceMode.Acceleration);
      }
    }
  }

  protected void CalculateAirTurn() {
    if (!sm.Grounded) {
      // between you and me, i never added this
      // sm.Facing.AddTorque(sm.Input.turn*sm.AirTurnForce);
    }
  }

  protected void SetHipHelperPos() {
    // place hip helper at mainRB height above the board
    var heightVector =  sm.MainRB.transform.position - sm.Board.position;
    var smoothHeight = Vector3.Dot(heightVector, -sm.DampedDown);
    // add crouching offset to height
    
    // smoothHeight -= sm.ProceduralCrouchFactor*sm.MaxProceduralCrouchDistance;
    // Debug.Log(smoothHeight);

    // step height
    sm.HipHeight ??= new ContinuousDataStepper(smoothHeight, sm.HipHelperFPS);
    var height = sm.HipHeight.Tick(smoothHeight, Time.fixedDeltaTime);
    sm.HipHelper.localPosition = new(sm.HipHelper.localPosition.x, height, sm.HipHelper.localPosition.z);
    sm.SmoothHipHelper.localPosition = new(sm.SmoothHipHelper.localPosition.x, smoothHeight, sm.SmoothHipHelper.localPosition.z);
    sm.BodyMesh.position = sm.HipHelper.position;
  }

  protected void SetSpeedyLines() {
    sm.SpeedyLinesMat.SetFloat("_amount", Mathf.InverseLerp(sm.MinSpeedyLineSpeed, sm.MaxSpeed, sm.MainRB.velocity.magnitude));
  }

  protected void ApplyRotationToModels() {
    sm.BodyMesh.rotation = sm.Facing.transform.rotation;
    sm.Board.rotation = sm.Facing.transform.rotation;
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
      sm.Facing.orientation = (sm.Facing.orientation + 0.5f) % 1;
  }

  protected void CalculatePush() {
    if (sm.Pushing && sm.CurrentPushT > Mathf.Epsilon) {
      if (sm.Grounded && Vector3.Angle(Vector3.down, sm.Down) < sm.PushingMaxSlope && !sm.Crouching) {
        // we're pushing
        float t = 1-(sm.CurrentPushT/sm.MaxPushT);
        sm.MainRB.AddForce(sm.PushForce * sm.PushForceCurve.Evaluate(t) * sm.Facing.transform.forward, ForceMode.Acceleration);
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

  protected void CheckWalls() {
    var raycastOrigin = sm.Board.position+sm.ForwardCollisionOriginYOffset*-sm.DampedDown;
    var raycastDirection = sm.Facing.transform.forward;
    var collisionDistance = sm.ForwardCollisionDistance;
    if (Physics.Raycast(raycastOrigin, raycastDirection, out RaycastHit hit, collisionDistance * 2f, LayerMask.GetMask("Ground"))) {
      if (hit.distance < collisionDistance) {
        var right = Vector3.Cross(sm.DampedDown, sm.Facing.transform.forward);
        var verticalWallNormal = Vector3.ProjectOnPlane(hit.normal, right);
        var forwardVelocity = Vector3.Project(sm.MainRB.velocity, sm.Facing.transform.forward);
        // if the hit distance is too close, then we've hit a wall
        // get the forward velocity, and then push back with the force required for that speed
        var wallSteepness = Vector3.Dot(verticalWallNormal.normalized, -forwardVelocity.normalized);
        if (wallSteepness > 0.8f)
          sm.MainRB.AddForce(-(forwardVelocity * (1 + sm.WallBounceForce)) / Time.fixedDeltaTime, ForceMode.Acceleration);
      }
    }
  }

  protected void CheckRails() {
    var vertVelocity = Vector3.Dot(Vector3.up, sm.MainRB.velocity);
    if (vertVelocity < -Mathf.Epsilon) {
      Rail rail = sm.RailManager.CheckRails(sm.Board.position, sm.MainRB.velocity);
      if (rail != null) {
        sm.GrindingRail = rail;
        sm.EnterRail();
      }
    }
  }
  
  protected void SetMovingFriction() {
    if (sm.Input.braking)
      sm.Friction = sm.BrakingFriction;
    else
      sm.Friction = sm.WheelFriction;
  }
  
  protected void SetGrindingFriction() {
    sm.Friction = sm.GrindingFriction;
  }

  protected void CapSpeed() {
    if (sm.MainRB.velocity.magnitude > sm.MaxSpeed)
      sm.MainRB.velocity = sm.MainRB.velocity.normalized * sm.MaxSpeed;
    sm.ProceduralCrouchFactor = sm.MainRB.velocity.magnitude / sm.MaxSpeed;
  }

  protected void SetWheelSpinParticleChance() {
    foreach (WheelSpinParticleHandler spinner in sm.WheelSpinParticles) {
      if (sm.Grounded) {
        spinner.SetChance(math.remap(sm.MinWheelSpinParticleSpeed, sm.MaxSpeed, sm.MinWheelSpinParticleChance, sm.MaxWheelSpinParticleChance, sm.MainRB.velocity.magnitude));
      }
      else {
        spinner.SetChance(0);
      }
    }
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

  protected void OnOllieTrickInput() {
    sm.CharacterAnimator.SetInteger("ollieTrickIndex", sm.CurrentOllieTrickIndex);
  }

  // protected void OnOllieInput() {
  //   // if (sm.Crouching) {
  //   //   sm.CharacterAnimator.SetTrigger("ollie");
  //   // }
  //   sm.CharacterAnimator.SetInteger("ollieTrickIndex", 1);
  // }

  // protected void OnKickflipInput() {
  //   // if (sm.Crouching) {
  //   //   sm.CharacterAnimator.SetTrigger("kickflip");
  //   // }
  //   sm.CharacterAnimator.SetInteger("ollieTrickIndex", 2);
  // }

  protected void ApplyFrictionForce() {
    Vector3 forwardVelocity = Vector3.Project(sm.MainRB.velocity, sm.Facing.transform.forward);
    float frictionMag = sm.Friction * forwardVelocity.magnitude;
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

  protected void FaceAlongRail() {
    sm.FacingParent.rotation = Quaternion.LookRotation((Vector3.Dot(sm.GrindingRail.RailVector.normalized, sm.MainRB.velocity.normalized)*sm.GrindingRail.RailVector).normalized, Vector3.up);
  }

  protected void DisableSpinBody() {
    sm.Facing.update = false;
    sm.Facing.transform.localRotation = Quaternion.identity;
  }

  protected void EnableSpinBody() {
    sm.Facing.update = true;
  }

  protected void StartRailBoost() {
    var signedRailVector = (Vector3.Dot(sm.GrindingRail.RailVector.normalized, sm.MainRB.velocity.normalized)*sm.GrindingRail.RailVector.normalized).normalized;
    sm.MainRB.AddForce(signedRailVector*sm.RailStartBoostForce, ForceMode.Acceleration);
  }

  protected void KeepOnRail() {
    var a = Vector3.ProjectOnPlane(Physics.gravity, sm.GrindingRail.RailVector);
    var v = Vector3.ProjectOnPlane(sm.MainRB.velocity, sm.GrindingRail.RailVector);
    sm.MainRB.AddForce(-a, ForceMode.Acceleration);
    sm.MainRB.AddForce(-v/Time.fixedDeltaTime, ForceMode.Acceleration);
  }

  protected void InitRailPos() {
    sm.GrindOffsetPID = new PIDController3();
  }

  protected void SetRailPos() {
    sm.GrindBoardLockPoint = sm.GrindingRail.GetNearestPoint(sm.Board.position);
    sm.Board.position = sm.GrindBoardLockPoint;
    sm.Board.position -= sm.RailLockTransforms[0].localPosition;
    var signedRailVector = (Vector3.Dot(sm.GrindingRail.RailVector.normalized, sm.MainRB.velocity.normalized)*sm.GrindingRail.RailVector.normalized).normalized;
    var railNormal = (sm.GrindingRail.RailOutside.normalized + Vector3.up + Vector3.up + Vector3.up) * 0.25f;
    sm.Board.rotation = Quaternion.LookRotation(signedRailVector, railNormal);
  }

  protected void StartRailAnim() {
    sm.CharacterAnimator.SetBool("50-50ing", true);
  }

  protected void EndRailAnim() {
    sm.CharacterAnimator.SetBool("50-50ing", false);
  }

  protected void AdjustToTargetRailOffset() {
    var railNormal = (sm.GrindingRail.RailOutside.normalized + 7*Vector3.up) * 0.125f;
    var target = sm.GrindBoardLockPoint + railNormal*sm.GrindOffsetHeight;
    // add the force to go to target
    sm.GrindOffsetPID.proportionalGain = sm.GrindingPosSpringConstant;
    sm.GrindOffsetPID.derivativeGain = sm.GrindingPosSpringDamping;
    sm.MainRB.AddForce(Vector3.ProjectOnPlane(sm.GrindOffsetPID.Update(Time.fixedDeltaTime, sm.MainRB.position, target), sm.GrindingRail.RailVector), ForceMode.Acceleration);
  }

  protected void CheckRailValidity() {
    sm.GrindBoardLockPoint = sm.GrindingRail.GetNearestPoint(sm.Board.position);
    if (sm.GrindBoardLockPoint == sm.LastGrindPos) {
      // end the grind
      sm.ExitRail();
    }
    sm.LastGrindPos = sm.GrindBoardLockPoint;
  }

  protected void PushOffRail() {
    sm.MainRB.AddForce(sm.GrindingRail.RailOutside.normalized*sm.ExitRailForce, ForceMode.Acceleration);
  }

  public void Spawn() {
    // find the nearest spawn point
    (Vector3 pos, Quaternion rot) = sm.SpawnPointManager.GetNearestSpawnPoint(sm.transform.position);
    // reset any necessary properties
    sm.MainRB.velocity = Vector3.zero;
    sm.MainRB.angularVelocity = Vector3.zero;

    sm.FacingParent.localRotation = Quaternion.identity;
    sm.Facing.Reset();

    sm.Down = Vector3.down;
    sm.DampedDown = Vector3.down;
    sm.Grounded = false;
    sm.TruckTurnPercent = 0;
    sm.HeadSensZone.SetT(0);
    // move to that nearest spawn point;
    sm.MainRB.MovePosition(pos);
    sm.FacingParent.rotation = rot;
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