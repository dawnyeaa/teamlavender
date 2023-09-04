using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour {
  public Vector3 pointA, pointB;
  public Vector3 GetNearestPoint(Vector3 pos) {
    var v = pointB - pointA;
    var u = pointA - pos;
    var vu = Vector3.Dot(v, u);
    var vv = Vector3.Dot(v, v);
    var t = -vu / vv;
    if (t >= 1) return pointB;
    if (t <= 0) return pointA;
    return Vector3.Lerp(pointA, pointB, t);
  }
}
