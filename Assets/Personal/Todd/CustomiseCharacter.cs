using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomiseCharacter : MonoBehaviour {
  private GameObject body;
  private List<CustomiseSlot> slots;

  void Awake() {
    body = transform.Find("body").gameObject;
    for (int i = 0; i < transform.childCount; ++i) {
      GameObject child = transform.GetChild(i).gameObject;
      if (child != body) {
        Destroy(child);
      }
    }
  }

  void Update() {
    
  }
}
