using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomiseCharacter : MonoBehaviour {
  private Dictionary<string, CustomiseSlot> slots;
  public GameObject characterMesh;
  private float colorT;
  private Material charMaterial;
  
  void Start() {
    var slotsarray = GetComponents<CustomiseSlot>();
    slots = new();
    foreach (CustomiseSlot slot in slotsarray) {
      slots.Add(slot.slotName, slot);
      if (CharCustoArrangement.instance != null)
        slot.SetSelected(CharCustoArrangement.instance.selectedSlots[slot.slotName]);
    }
    charMaterial = characterMesh.GetComponent<SkinnedMeshRenderer>().material;
  }

  public void CustomiseColor(float newT) {
    colorT = newT;
    if (charMaterial == null) return;
    charMaterial.SetFloat("_GradientX", newT);
  }

  public float GetT() => colorT;
}
