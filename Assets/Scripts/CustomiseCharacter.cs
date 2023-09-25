using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomiseCharacter : MonoBehaviour {
  private Dictionary<string, CustomiseSlot> slots;
  
  void Start() {
    var slotsarray = GetComponents<CustomiseSlot>();
    slots = new();
    foreach (CustomiseSlot slot in slotsarray) {
      slots.Add(slot.slotName, slot);
      slot.SetSelected(CharCustoArrangement.instance.selectedSlots[slot.slotName]);
    }
  }
}
