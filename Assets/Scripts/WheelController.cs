using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour {
  public Transform wheel1, wheel2, wheel3, wheel4;
  private Vector3 wheel1DefRot, wheel2DefRot, wheel3DefRot, wheel4DefRot;
  public float rotationDeg = 0;

  void Start() {
    rotationDeg = 0;
    wheel1DefRot = wheel1.localEulerAngles;
    wheel2DefRot = wheel2.localEulerAngles;
    wheel3DefRot = wheel3.localEulerAngles;
    wheel4DefRot = wheel4.localEulerAngles;
  }
  void LateUpdate() {
    rotationDeg = rotationDeg % 360;
    Vector3 rotatey = rotationDeg*Vector3.forward;
    wheel1.localEulerAngles = wheel1DefRot - rotatey;
    wheel2.localEulerAngles = wheel2DefRot - rotatey;
    wheel3.localEulerAngles = wheel3DefRot + rotatey;
    wheel4.localEulerAngles = wheel4DefRot + rotatey;
  }

  public void AddRotation(float ang) {
    rotationDeg = (rotationDeg + ang)%360f;
  }
}
