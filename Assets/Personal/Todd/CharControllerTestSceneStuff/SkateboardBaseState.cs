using UnityEngine;
using UnityEditor;

public abstract class SkateboardBaseState : State {
  protected readonly SkateboardStateMachine stateMachine;
  protected float TurnSpeed = 0.0f;

  protected SkateboardBaseState(SkateboardStateMachine stateMachine) {
    this.stateMachine = stateMachine;
  }

  protected void BeginPush() {
    if ((stateMachine.Decelerating && stateMachine.DecelTime >= stateMachine.PushCooldown) || (!stateMachine.Pushing && !stateMachine.Decelerating)) {
      stateMachine.PushTime = 0;
      stateMachine.Pushing = true;
      stateMachine.Decelerating = false;
    }
  }

  protected void BeginSlowdown() {
    stateMachine.CoastSpeed = stateMachine.CurrentSpeed;
    stateMachine.DecelTime = 0;
    stateMachine.Decelerating = true;
    stateMachine.Pushing = false;
  }

  protected void Accelerate() {
    // acceleration stuff first
    if (stateMachine.Pushing) {
      stateMachine.PushTime += Time.deltaTime;
      float pushRemapped = stateMachine.PushAccelCurve.Evaluate(stateMachine.PushTime/stateMachine.PushDuration);
      float velocityPushFactor = stateMachine.PushSpeedFalloff.Evaluate(stateMachine.CurrentSpeed/stateMachine.MaxSpeed);
      float newSpeed = stateMachine.CurrentSpeed+(stateMachine.PushAccel*Time.deltaTime*pushRemapped*velocityPushFactor);
      // float cappedSpeed = Mathf.Min(newSpeed, stateMachine.MaxSpeed);
      // float pushExcess = (newSpeed-cappedSpeed)*stateMachine.ExcessPushStrength;
      stateMachine.CurrentSpeed = newSpeed;
      if (stateMachine.PushTime >= stateMachine.PushDuration) {
        BeginSlowdown();
      }
    }
    // then decelerate
    // if we're slowing down
    else if (stateMachine.Decelerating) {
      stateMachine.DecelTime += Time.deltaTime;
      stateMachine.CurrentSpeed = (stateMachine.Mass*stateMachine.CoastSpeed)/((stateMachine.Drag*stateMachine.DragMultiplier*stateMachine.CoastSpeed*stateMachine.DecelTime)+stateMachine.Mass);
      if (stateMachine.CurrentSpeed <= 0) {
        stateMachine.CurrentSpeed = 0;
        stateMachine.Decelerating = false;
      }
    }
  }

  protected void Brake() {
    // if (stateMachine.Input.braking) {
    //   stateMachine.CurrentSpeed -= stateMachine.BrakingStrength;
    //   stateMachine.CurrentSpeed = stateMachine.CurrentSpeed <= 0 ? 0 : stateMachine.CurrentSpeed;
    // }
  }

  protected void CalculateTurn() {
    if (stateMachine.CurrentSpeed > 0) {
      stateMachine.Turning = Mathf.SmoothDamp(stateMachine.Turning, stateMachine.Input.turn, ref TurnSpeed, stateMachine.TurnSpeedDamping);
      float deltaPos = Time.deltaTime*stateMachine.CurrentSpeed;
      float rad = stateMachine.TruckSpacing/Mathf.Sin(Mathf.Deg2Rad*stateMachine.Turning*stateMachine.MaxTruckTurnDeg);
      float circum = ThisIsJustTau.TAU*rad;
      stateMachine.Facing += deltaPos/circum;
      stateMachine.DragMultiplier = 1.0f + Mathf.Abs(stateMachine.Turning)*stateMachine.TurnSlowdown;
    }
  }

  protected void ApplyGravity() {
    stateMachine.FallingSpeed += Physics.gravity.magnitude * Time.deltaTime;
    stateMachine.transform.position += stateMachine.FallingSpeed * Vector3.down * Time.deltaTime;
  }

  protected void MatchGround() {
    Vector3 projectOrigin = stateMachine.transform.position + Vector3.up * stateMachine.ProjectOffset;
    // stateMachine.bALL.localPosition = Vector3.up * stateMachine.ProjectOffset;
    // stateMachine.bALL.localScale = Vector3.one * stateMachine.ProjectRadius;
    RaycastHit hit;
    bool itHit = Physics.SphereCast(projectOrigin, stateMachine.ProjectRadius, -Vector3.up, out hit, stateMachine.ProjectOffset, LayerMask.GetMask("Ground"));
    Vector3 newNormal = Vector3.up;
    if (itHit && hit.distance <= stateMachine.ProjectOffset) {
      stateMachine.FallingSpeed = 0;
      stateMachine.transform.position = new Vector3(stateMachine.transform.position.x, hit.point.y, stateMachine.transform.position.z);
      newNormal = hit.normal;
    }
    // Debug.DrawRay(projectOrigin, -stateMachine.transform.up * stateMachine.ProjectOffset, new Color(255/255f, 174/255f, 12/255f), 5f);
    // Debug.DrawRay(stateMachine.transform.position, newNormal * 2, Color.blue, 5f);

    Vector3 newRight = Vector3.Cross(newNormal, stateMachine.transform.forward);
    Vector3 newForward = Vector3.Cross(newRight, newNormal);
    // Debug.DrawRay(stateMachine.transform.position, newRight, Color.red, 5f);
    // Debug.DrawRay(stateMachine.transform.position, newForward, Color.black, 5f);
    stateMachine.transform.rotation = Quaternion.LookRotation(newForward, newNormal);

    stateMachine.FrontGradientOrigin.localPosition = new Vector3(stateMachine.FrontGradientOrigin.localPosition.x, stateMachine.GradientProjectOffset, stateMachine.FrontGradientOrigin.localPosition.z);
    stateMachine.BackGradientOrigin.localPosition = new Vector3(stateMachine.BackGradientOrigin.localPosition.x, stateMachine.GradientProjectOffset, stateMachine.BackGradientOrigin.localPosition.z);
    
    Vector3 frontPos = Vector3.zero;
    Vector3 backPos = Vector3.zero;
    bool frontHit = Physics.Raycast(stateMachine.FrontGradientOrigin.position, -stateMachine.transform.up, out hit, 2f*stateMachine.GradientProjectOffset, LayerMask.GetMask("Ground"));
    if (frontHit) {
      frontPos = hit.point;
      stateMachine.FrontGradientTrack.position = frontPos;
    }
    
    bool backHit = Physics.Raycast(stateMachine.BackGradientOrigin.position, -stateMachine.transform.up, out hit, 2f*stateMachine.GradientProjectOffset, LayerMask.GetMask("Ground"));
    if (backHit) {
      backPos = hit.point;
      stateMachine.BackGradientTrack.position = backPos;
    }

    if (frontHit && backHit) {
      Vector3 newFacing = frontPos - backPos;
      stateMachine.SkateboardMeshTransform.rotation = Quaternion.LookRotation(newFacing, newNormal);
    }
  }

  protected void Move() {
    stateMachine.transform.rotation = Quaternion.AngleAxis(stateMachine.Facing*360f, stateMachine.transform.up);
    stateMachine.transform.position += stateMachine.CurrentSpeed * Time.fixedDeltaTime * stateMachine.transform.forward;
  }

  protected void SpinWheels() {
    float circum = ThisIsJustTau.TAU*stateMachine.WheelRadius;
    stateMachine.Wheels.AddRotation(((Time.deltaTime*stateMachine.CurrentSpeed)/circum)*360f);
  }

  protected void BodyRotationDamp() {
    stateMachine.Body.rotation = stateMachine.LastFrameRotation;
    Quaternion targetRotation = stateMachine.RotationTarget.rotation;
    float catchupMultiplier = Mathf.Pow(2f-Vector3.Dot(stateMachine.Body.forward, stateMachine.RotationTarget.forward), 4f);
    stateMachine.Body.rotation = Quaternion.RotateTowards(stateMachine.Body.rotation, targetRotation, stateMachine.RotationSpeed * catchupMultiplier * Time.deltaTime);
    stateMachine.LastFrameRotation = stateMachine.Body.rotation;
  }
}