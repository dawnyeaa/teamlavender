using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour {
  public Vector3 Position {
    get { return (TransformB.position-TransformA.position)/2f; }
  }
  public Vector3 RailVector {
    get { return TransformB.position-TransformA.position; }
  }
  public Vector3 RailVectorNorm {
    get { return RailVector.normalized; }
  }
  public Vector3 RailOutside {
    get { return Vector3.Cross(Vector3.up, RailVector.normalized) * (outsideRight ? 1 : -1); }
  }

  public Transform TransformA, TransformB;
  public bool outsideRight = true;
  public Vector3 GetNearestPoint(Vector3 pos) {
    var v = RailVector;
    var u = TransformA.position - pos;
    var vu = Vector3.Dot(v, u);
    var vv = Vector3.Dot(v, v);
    var t = -vu / vv;
    if (t >= 1) return TransformB.position;
    if (t <= 0) return TransformA.position;
    return Vector3.Lerp(TransformA.position, TransformB.position, t);
  }
}
