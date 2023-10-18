using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabiliseMotionBlurCone : MonoBehaviour {
  public float distance = 1f;
  private Camera cam;

  void LateUpdate() {
    cam = cam != null ? cam : Camera.main;
    var camForwards = cam.transform.forward;
    var facing = Vector3.ProjectOnPlane(camForwards, Vector3.up).normalized;
    transform.SetPositionAndRotation(cam.transform.position + facing * distance, Quaternion.LookRotation(Vector3.up, -facing));
  }
}
