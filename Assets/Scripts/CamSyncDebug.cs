using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSyncDebug : MonoBehaviour {
  public Transform charFacing;

  void Update() {
    // Debug.DrawRay(charFacing.position, transform.forward, Color.green, 5);
    Debug.DrawRay(charFacing.position, charFacing.forward, Color.red);
  }
}
