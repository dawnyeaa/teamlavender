using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour {
  public Vector3 Position {
    get { return (PointB-PointA)/2f; }
  }
  public Vector3 RailVector {
    get { return PointB-PointA; }
  }
  public Vector3 RailOutside {
    get { return Vector3.Cross(Vector3.up, RailVector.normalized) * (outsideRight ? 1 : -1); }
  }

  public Vector3 PointA = -Vector3.forward, PointB = Vector3.forward;
  public bool outsideRight = true;
  public Vector3 GetNearestPoint(Vector3 pos) {
    var v = PointB - PointA;
    var u = PointA - pos;
    var vu = Vector3.Dot(v, u);
    var vv = Vector3.Dot(v, v);
    var t = -vu / vv;
    if (t >= 1) return PointB;
    if (t <= 0) return PointA;
    return Vector3.Lerp(PointA, PointB, t);
  }
}
