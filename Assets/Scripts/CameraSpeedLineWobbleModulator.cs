using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpeedLineWobbleModulator : MonoBehaviour {
  Quaternion lastRotation;
  public float speedFactorMultiplier = 1;
  public float speed;
  private RendererFeatureDynamicProperties RFprops;
  void Start() {
    RFprops = GetComponent<RendererFeatureDynamicProperties>();
    lastRotation = transform.rotation;
  }
  // Update is called once per frame
  void Update() {
    speed = Quaternion.Angle(transform.rotation, lastRotation);
    RFprops.LineWobbleCameraFactor = Mathf.Clamp01(speedFactorMultiplier/Mathf.Max(Mathf.Epsilon, speed));
    lastRotation = transform.rotation;
  }
}
