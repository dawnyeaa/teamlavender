using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {
  public PickupManager pickupManager;
  public GameObject milky, ciggy;
  private bool frankMode = true;

  void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
      pickupManager.Pickup();
    }
  }

  public void SetFrankMode(bool newFrankMode) {
    frankMode = newFrankMode;
    UpdateFrankMode();
  }

  public void UpdateFrankMode() {
    milky.SetActive(frankMode);
    ciggy.SetActive(!frankMode);
  }
}
