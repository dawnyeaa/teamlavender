using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RBCenterOfMassOffset : MonoBehaviour {
  public Vector3 offset;

  void Start() {
    var rb = GetComponent<Rigidbody>();
    rb.centerOfMass = offset;
  }
}
