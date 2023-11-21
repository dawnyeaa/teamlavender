using UnityEngine;
using Unity.Mathematics;

using UnityEditor;
using RootMotion.FinalIK;
using System;

public abstract class SkateboardBaseState : State {
  public readonly SkateboardStateMachine sm;
  protected float LeanSpeed = 0.0f;
  protected float VisFollowSpeed = 0.0f;
  protected Vector3 boardRailSnapVel = Vector3.zero;

  protected SkateboardBaseState(SkateboardStateMachine stateMachine) {
    sm = stateMachine;
  }

  protected void VertBodySpring() {
    // set sphere collision visualiser size
    sm.footRepresentation.localScale = new Vector3(sm.ProjectRadius, sm.ProjectRadius, sm.ProjectRadius) * 2f;

    sm.CurrentProjectLength = sm.ProjectLength;

    // this was the old spherecast that *mostly* worked
    // Physics.SphereCast(sm.transform.position, sm.ProjectRadius, sm.RawDown, out RaycastHit hit, sm.ProjectLength, LayerMask.GetMask("Ground"))

    // box cast from body down - box does not need to be the same radius as the collider
    if (Physics.BoxCast(sm.transform.position, Vector3.one * sm.ProjectRadius, sm.RawDown, out RaycastHit hit, Quaternion.identity, sm.ProjectLength, LayerMask.GetMask("Ground"))) {

      sm.debugFrame.pointOfContact = hit.point;
      sm.debugFrame.contactNormal = hit.normal;

      Vector3 truckRelative = Vector3.Cross(sm.RawDown, Vector3.Cross(sm.Facing.transform.forward, sm.RawDown)).normalized*sm.TruckSpacing;

      Vector3 frontHitPos, backHitPos;
      Vector3 tempNormal;
      float bonusDistance = 0.25f;

      Debug.DrawRay(sm.transform.position + truckRelative, sm.RawDown*(sm.ProjectLength + bonusDistance), Color.white);
      Debug.DrawRay(sm.transform.position - truckRelative, sm.RawDown*(sm.ProjectLength + bonusDistance), Color.white);

      bool frontHit = Physics.Raycast(sm.transform.position + truckRelative, sm.RawDown, out RaycastHit rayHit, sm.ProjectLength + bonusDistance, LayerMask.GetMask("Ground"));
      if (frontHit) {
        // front truck hit!!!
        frontHitPos = rayHit.point;
        tempNormal = rayHit.normal;
        var frontHitDistance = rayHit.distance;
        bool backHit = Physics.Raycast(sm.transform.position - truckRelative, sm.RawDown, out rayHit, sm.ProjectLength + bonusDistance, LayerMask.GetMask("Ground"));
        if (backHit) {
          // back truck hit!!!
          backHitPos = rayHit.point;

          // bool weGood = true;

          // if (Vector3.Dot(sm.MainRB.velocity, sm.Facing.transform.forward) > 0) {
          //   if (frontHitDistance > rayHit.distance) {
          //     weGood = false;
          //   }
          // }
          // else {
          //   if (rayHit.distance > frontHitDistance) {
          //     weGood = false;
          //   }
          // }

          // if (weGood && (Vector3.Dot(tempNormal, rayHit.normal)*0.5+0.5) > sm.LipAngleTolerance) {
          tempNormal = (tempNormal + rayHit.normal)/2f;

          var newForward = backHitPos - frontHitPos;
          var newNormal = Vector3.Cross(newForward, Vector3.Cross(newForward, -tempNormal)).normalized;
          sm.RawDown = -newNormal;
          // }
        }
      }

      // add a force to push the body away from the surface - stronger if closer (like buoyancy)
      sm.CurrentProjectLength = hit.distance;
      float compression = sm.ProjectLength - sm.CurrentProjectLength;

      sm.MainRB.AddForce((-sm.RawDown * (compression * sm.SpringConstant + Vector3.Dot(sm.RawDown, sm.MainRB.velocity) * sm.SpringDamping))*sm.SpringMultiplier, ForceMode.Acceleration);
      if (!sm.Grounded) {
        Vector3 flatMovement = Vector3.ProjectOnPlane(sm.MainRB.velocity, -sm.RawDown).normalized;
        if (flatMovement.magnitude > 0.2f && Mathf.Abs(Vector3.Dot(flatMovement, sm.Facing.transform.forward)) < sm.LandingAngleGive) {
          // sm.EnterDead();
        }
        else {
        }

        if (sm.AirTimeCounter > sm.MinimumAirTime) {
          sm.PointManager.Validate();
        }

        sm.LandEmit.Play();
        SoundEffectsManager.instance.PlaySoundFXClip(sm.LandingHardClip, sm.Board, 1);
        StartRollingSFX();
      }
      sm.Grounded = true;
    }
    else {
      if (sm.Grounded) {
        sm.CharacterAnimator.SetTrigger("startAirborne");
        sm.AirTimeCounter = 0;
        StopRollingSFX();
      }
      sm.Grounded = false;
    }
    if (!sm.Grounded) {
      if (sm.AirTimeCounter > sm.MinimumAirTime)
        // sm.PointManager.AddPoints(Mathf.RoundToInt(Time.fixedDeltaTime*sm.PointsPerAirTimeSecond));
      sm.AirTimeCounter += Time.fixedDeltaTime;
    }
    sm.Down = Vector3.Slerp(sm.Down, sm.RawDown, 1f/Mathf.Pow(2, sm.BoardPositionDamping));
    sm.footRepresentation.localPosition = sm.Down * sm.CurrentProjectLength;
    sm.Board.localPosition = sm.Down * (sm.CurrentProjectLength + sm.ProjectRadius);
    sm.BodyMesh.localPosition = sm.Down * (sm.CurrentProjectLength + sm.ProjectRadius);
  }

  protected void AdjustSpringMultiplier() {
    sm.SpringMultiplier = Mathf.Abs(Vector3.Dot(Vector3.up, -sm.RawDown));
    sm.SpringMultiplier = math.remap(0, 1, sm.SpringMultiplierMin, sm.SpringMultiplierMax, sm.SpringMultiplier);
  }

  protected void SetHipHelperPos() {
    // place hip helper at mainRB height above the board
    var heightVector =  sm.MainRB.transform.position - sm.Board.position;
    var smoothHeight = Vector3.Dot(heightVector, -sm.Down);
    // add crouching offset to height
    
    // smoothHeight -= sm.ProceduralCrouchFactor*sm.MaxProceduralCrouchDistance;
    // Debug.Log(smoothHeight);

    // step height
    sm.HipHeight ??= new ContinuousDataStepper(smoothHeight, sm.HipHelperFPS);
    var height = sm.HipHeight.Tick(smoothHeight, Time.fixedDeltaTime);
    sm.HipHelper.localPosition = new(sm.HipHelper.localPosition.x, height, sm.HipHelper.localPosition.z);
    sm.SmoothHipHelper.localPosition = new(sm.SmoothHipHelper.localPosition.x, smoothHeight, sm.SmoothHipHelper.localPosition.z);
    // sm.BodyMesh.position = sm.HipHelper.position;
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

  protected void CapSpeed() {
    if (sm.MainRB.velocity.magnitude > sm.MaxSpeed)
      sm.MainRB.velocity = sm.MainRB.velocity.normalized * sm.MaxSpeed;
    sm.ProceduralCrouchFactor = sm.MainRB.velocity.magnitude / sm.MaxSpeed;
  }

  protected void CheckWalls() {
    var raycastOrigin = sm.Board.position+sm.ForwardCollisionOriginYOffset*-sm.Down;
    var raycastDirection = sm.Facing.transform.forward;
    var collisionDistance = sm.ForwardCollisionDistance;
    if (Physics.Raycast(raycastOrigin, raycastDirection, out RaycastHit hit, collisionDistance * 2f, LayerMask.GetMask("Ground"))) {
      if (hit.distance < collisionDistance) {
        var right = Vector3.Cross(sm.Down, sm.Facing.transform.forward);
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

  protected void PassSpeedToMotionBlur() {
    sm.RFprops.MotionBlurSize = sm.MaxMotionBlur * sm.MainRB.velocity.magnitude / sm.MaxSpeed;
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

  protected void StartRollingSFX() {
    if (sm.RollingHardClipIndex == -1) {
      sm.RollingHardClipIndex = SoundEffectsManager.instance.PlayLoopingSoundFXClip(sm.RollingHardClip, sm.Board, 1);
    }
  }

  protected void StopRollingSFX() {
    if (sm.RollingHardClipIndex != -1) {
      SoundEffectsManager.instance.StopLoopingSoundFXClip(sm.RollingHardClipIndex);
      sm.RollingHardClipIndex = -1;
    }
  }

  protected void SetSpeedyLines() {
    sm.SpeedyLinesMat.SetFloat("_amount", Mathf.InverseLerp(sm.MinSpeedyLineSpeed, sm.MaxSpeed, sm.MainRB.velocity.magnitude));
  }

  protected void OnSwitch() {
    // if (sm.Grounded)
      // sm.Facing.orientation = (sm.Facing.orientation + 0.5f) % 1;
  }

  protected void SetCrouchingOld() {
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

  protected void OnHopTrickInput(int trickAnimGroup, float verticalForceMult, float horizontalForceMultiplier) {
    sm.CharacterAnimator.SetInteger(Enum.GetName(typeof(TrickAnimationGroup), (TrickAnimationGroup)trickAnimGroup), sm.CurrentAnimTrickIndexes[trickAnimGroup]);
    sm.CurrentHopTrickVerticalMult = verticalForceMult;
    sm.CurrentHopTrickHorizontalMult = horizontalForceMultiplier;
  }

  protected void RollIdleAnimation() {
    if (UnityEngine.Random.value > (1-(Time.fixedDeltaTime/sm.AverageSecondsPerBreath)))
      sm.CharacterAnimator.SetTrigger("breathe");
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

  protected void FaceAlongRail() {
    sm.Facing.transform.rotation = Quaternion.LookRotation((Vector3.Dot(sm.GrindingRail.RailVector.normalized, sm.MainRB.velocity.normalized)*sm.GrindingRail.RailVector).normalized, Vector3.up);
  }

  protected void StartGrindingParticles() {
    sm.GrindParticles.SetActive(true);
  }

  protected void StopGrindingParticles() {
    sm.GrindParticles.SetActive(false);
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

  protected void ResetFacing()
  {
      sm.IsGoofy = false;
      sm.CharacterAnimator.SetFloat("mirrored", 0);
      sm.Facing.localRotation = Quaternion.identity;
  }

  public void Spawn() {
    // find the nearest spawn point
    (Vector3 pos, Quaternion rot) = sm.SpawnPointManager.GetNearestSpawnPoint(sm.transform.position);
    // reset any necessary properties
    sm.MainRB.velocity = Vector3.zero;
    sm.MainRB.angularVelocity = Vector3.zero;

    sm.RawDown = Vector3.down;
    sm.Down = Vector3.down;
    sm.Grounded = false;
    sm.TurnPercent = 0;
    ResetFacing();
    // move to that nearest spawn point;
    sm.MainRB.MovePosition(pos);
    sm.transform.rotation = rot;
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
    sm.debugFrame.downVector = sm.RawDown;
    sm.debugFrame.dampedDownVector = sm.Down;

    sm.DebugFrameHandler.PutFrame(sm.debugFrame);
  }
}