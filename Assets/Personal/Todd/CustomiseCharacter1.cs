using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomiseCharacter1 : MonoBehaviour {
  private GameObject body;
  private List<CustomiseSlot> slots;

  private Material bodyMaterial;

  void Awake() {
    body = transform.Find("body").gameObject;
    for (int i = 0; i < transform.childCount; ++i) {
      GameObject child = transform.GetChild(i).gameObject;
      if (child != body) {
        Destroy(child);
      }
    }

    slots = GetComponents<CustomiseSlot>().ToList<CustomiseSlot>();
  }

  void Start() {
    bodyMaterial = body.GetComponent<MeshRenderer>().material;
  }

  void Update() {
    
  }

  public void CustomiseColor(float newT) {
    bodyMaterial.SetFloat("_T", newT);
    // foreach (CustomiseSlot slot in slots) {
    //   slot.CustomiseColor(newT);
    // }
  }

  public void CustomiseColor(Vector2 uv, int slot, int maskIndex) {
    // TODO
  }
}
