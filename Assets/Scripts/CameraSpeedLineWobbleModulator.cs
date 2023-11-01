using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpeedLineWobbleModulator : MonoBehaviour {
  Quaternion lastRotation;
  public float speedFactorMultiplier = 1;
  public Material simpleOutlineMaterial;
  public float speed;
  void Start() {
    lastRotation = transform.rotation;
  }
  // Update is called once per frame
  void Update() {
    speed = Quaternion.Angle(transform.rotation, lastRotation);
    simpleOutlineMaterial.SetFloat("_CameraWarpingFactor", Mathf.Clamp01(speedFactorMultiplier/Mathf.Max(Mathf.Epsilon, speed)));
    lastRotation = transform.rotation;
  }
}
