using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {
  public PickupManager pickupManager;

  void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
      pickupManager.ChooseNextPickup();
    }
  }
}
