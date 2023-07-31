using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FuckYeah : MonoBehaviour {
  [Range(-1,1)]
  public float turn = 0;
  public float visDistance = 10f;
  public float truckSpacing = 0.205f;
  public float maxTruckTurnDeg = 8.34f;
  public float wheelRadius = 0.03f;
  public float goSpeed = 3.2f;
  public Animator skateanim;
  public WheelController wheels;

  // Update is called once per frame
  void Update() {
    skateanim.SetFloat("dir", turn);
    SpinWheels(goSpeed, Mathf.PI*2*wheelRadius);
    float deltaPos = Time.deltaTime*goSpeed;
    float rad = truckSpacing/Mathf.Sin(Mathf.Deg2Rad*turn*maxTruckTurnDeg);
    float circum = Mathf.PI*2*rad;
    transform.position += transform.forward*deltaPos;
    transform.rotation = Quaternion.AngleAxis((deltaPos/circum)*360f, transform.up) * transform.rotation;
  }

  void DrawArcGizmo(Vector3 arcStart, Vector3 arcCenterDir, float arcRadius, float arcDegrees, int segCount=30) {
    float startDeg, endDeg;
    Vector3 arcCenterDirSigned = arcCenterDir*(arcDegrees>0?1:-1);
    float arcDegNorm = Mathf.Abs(arcDegrees);
    float segLengthDeg = arcDegrees/segCount;
    Vector3 clockHand = -arcCenterDirSigned*arcRadius;
    Vector3 centerPos = arcStart-clockHand;
    Vector3 circleUp = Vector3.up;
    for (int i = 0; i < segCount; ++i) {
      startDeg = i*segLengthDeg;
      endDeg = startDeg + segLengthDeg;
      Vector3 startPos = (Quaternion.AngleAxis(startDeg, circleUp) * clockHand) + centerPos;
      Vector3 endPos = (Quaternion.AngleAxis(endDeg, circleUp) * clockHand) + centerPos;
      Gizmos.DrawLine(startPos, endPos);
    }
  }

  void OnDrawGizmos() {
    if (turn==0) {
      Gizmos.DrawRay(transform.position, transform.forward*visDistance);
    }
    else {
      float radius = truckSpacing/Mathf.Sin(Mathf.Deg2Rad*turn*maxTruckTurnDeg);
      float circum = Mathf.PI*2*radius;
      float arcDegrees = Mathf.Clamp(visDistance/circum, -1f, 1f)*360f;
      DrawArcGizmo(transform.position, transform.right, Mathf.Abs(radius), arcDegrees);
    }
  }

  void SpinWheels(float speed, float circum) {
    wheels.AddRotation(((Time.deltaTime*speed)/circum)*360f);
  }
}
