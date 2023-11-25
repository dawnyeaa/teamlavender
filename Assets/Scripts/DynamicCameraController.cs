using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DynamicCameraController : MonoBehaviour {
  public float groundedVertOffset = 1.15f;
  public float airborneVertOffset = 0;
  public float vertOffsetDamping = 1;
  public float airborneZoomFactor = 1.2f;
  public float deadZoomFactor = 1.2f;
  public float zoomFactorDamping = 0.1f;
  public float coolTimeZoomFactor = 1.5f;
  public AnimationCurve zoomFactorOverCoolTimeJump;
  [ReadOnly] private bool isAirborne = false;
  [ReadOnly] private bool isDead = false;
  [ReadOnly] private bool isCoolTimeJump = false;
  [ReadOnly] private float jumpTimer = 0;
  [ReadOnly] private float timeToLand = 0;
  [ReadOnly] private float vertOffsetTarget = 1.15f;

  [ReadOnly] private float currentVertOffset = 1.15f;
  [ReadOnly] private float vertOffsetVelocity = 0;
  [ReadOnly] private float targetZoomFactor = 1f;
  [ReadOnly] private float currentZoomFactor = 1f;
  [ReadOnly] private float zoomFactorSpeed = 0;
  CinemachineFreeLook freeLook;
  float topRadius, midRadius, bottomRadius;
  CinemachineComposer vCamComposer;
  void Start() {
    freeLook = GetComponent<CinemachineFreeLook>();
    vCamComposer = freeLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
    topRadius = freeLook.m_Orbits[0].m_Radius;
    midRadius = freeLook.m_Orbits[1].m_Radius;
    bottomRadius = freeLook.m_Orbits[2].m_Radius;
  }

  void Update() {
    if (isCoolTimeJump) jumpTimer += Time.deltaTime;
    vertOffsetTarget = isAirborne ? airborneVertOffset : groundedVertOffset;

    targetZoomFactor = isAirborne ? airborneZoomFactor : 1;

    targetZoomFactor = isDead ? deadZoomFactor : targetZoomFactor;

    var newOffset = vCamComposer.m_TrackedObjectOffset;
    currentVertOffset = Mathf.SmoothDamp(currentVertOffset, vertOffsetTarget, ref vertOffsetVelocity, vertOffsetDamping);
    vCamComposer.m_TrackedObjectOffset = new(newOffset.x, currentVertOffset, newOffset.z);

    currentZoomFactor = Mathf.SmoothDamp(currentZoomFactor, targetZoomFactor, ref zoomFactorSpeed, zoomFactorDamping);
    currentZoomFactor = isCoolTimeJump ? Mathf.Lerp(1, coolTimeZoomFactor, zoomFactorOverCoolTimeJump.Evaluate(jumpTimer/timeToLand)) : currentZoomFactor;
    freeLook.m_Orbits[0].m_Radius = topRadius * currentZoomFactor;
    freeLook.m_Orbits[1].m_Radius = midRadius * currentZoomFactor;
    freeLook.m_Orbits[2].m_Radius = bottomRadius * currentZoomFactor;
    isCoolTimeJump = jumpTimer < timeToLand;
  }
  public void StartAirborne() {
    isAirborne = true;
  }

  public void EndAirborne() {
    isAirborne = false;
    isCoolTimeJump = false;
  }

  public void StartDead() {
    isDead = true;
  }

  public void EndDead() {
    isDead = false;
  }

  public void OverrideZoom(float zoomFactor) {
    currentZoomFactor = zoomFactor;
    zoomFactorSpeed = 0;
  }

  public void StartCoolTimeJump(float totalTimeToLand) {
    isCoolTimeJump = true;
    jumpTimer = 0;
    timeToLand = totalTimeToLand;
  }
}
