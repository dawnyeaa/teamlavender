using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour {
  public SkateboardStateMachine sm;
  public PointManager pointManager;
  public List<Pickup> pickups;
  public int currentlyEnabled = 0;

  void Start() {
    SetEnabled();
  }

  void SetEnabled() {
    for (int i = 0; i < pickups.Count; ++i) {
      pickups[i].gameObject.SetActive(currentlyEnabled == i);
    }
  }

  public void ChooseNextPickup() {
    var last = currentlyEnabled;
    var newSelected = Random.Range(1, pickups.Count);
    while (newSelected == last) newSelected = Random.Range(0, pickups.Count);
    currentlyEnabled = newSelected;
    SetEnabled();
  }

  public void Pickup() {
    sm.OnPickup();
    ChooseNextPickup();
    pointManager.UpdatePickupDisplay();
  }

  public void SetFrankMode(bool enabled) {
    foreach (var pickup in pickups) {
      pickup.SetFrankMode(enabled);
    }
  }
}
