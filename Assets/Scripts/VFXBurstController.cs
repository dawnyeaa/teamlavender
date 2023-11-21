using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXBurstController : MonoBehaviour {
  public void Burst() {
    for (int i = 0; i < transform.childCount; ++i) {
      transform.GetChild(i).gameObject.SetActive(false);
      transform.GetChild(i).gameObject.SetActive(true);
    }
  }
}
