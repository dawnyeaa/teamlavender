using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ParentMenuCanceller : MonoBehaviour {
  public UnityEvent CancelEvent;
  private bool initialised = false;
  // Start is called before the first frame update
  void OnEnable() {
    if (!initialised) {
      foreach (var obj in GetComponentsInChildren<Selectable>().Select(x => x.gameObject)) {
        obj.AddComponent<MenuCanceller>().CancelEvent = CancelEvent;
      }
      initialised = true;
    }
  }
}
