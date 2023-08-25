using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour {
  public float spinSpeed = 1f;
  public float angle = 0f;
  void Update() {
    angle += spinSpeed;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
  }
}
