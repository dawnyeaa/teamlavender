using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {
  public SkateboardStateMachine character;
  void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
      // kill em
      character.Die();
    }
  }
}
