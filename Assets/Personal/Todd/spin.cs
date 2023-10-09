using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour {
  public float spinSpeed = 1f;
  public float angle = 0f;
  void Update() {
    if (Time.timeScale > 0) {
      angle += spinSpeed;
      transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
  }
}
