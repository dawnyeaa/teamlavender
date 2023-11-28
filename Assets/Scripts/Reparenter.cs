using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reparenter : MonoBehaviour {
  public Transform newParent;

  void Start() {
    transform.SetParent(newParent);
  }
}
