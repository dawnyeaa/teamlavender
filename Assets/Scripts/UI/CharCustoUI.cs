using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharCustoUI : MonoBehaviour {
  public Transform customiserChar;
  public EventSystem eventsys;
  public GameObject[] buttons;
  public UIExtraInput input;
  public GameObject bitsCheckbox;
  public float rotationSpeed = 1;
  private Dictionary<string, CustomiseSlot> slots;
  private int currentSlot = 0;
  private static readonly string[] slotNames = {
    "dome",
    "specs",
    "spitter",
    "top",
    "low-duds",
    "kicks",
    "bits"
  };
  void OnEnable() {
    var slotsarray = customiserChar.GetComponents<CustomiseSlot>();
    slots = new();
    foreach (CustomiseSlot slot in slotsarray) {
      slots.Add(slot.slotName, slot);
    }
    input.OnMenuRPerformed += NextInCurrentSlot;
    input.OnMenuLPerformed += PrevInCurrentSlot;
  }

  void OnDisable() {
    input.OnMenuRPerformed -= NextInCurrentSlot;
    input.OnMenuLPerformed -= PrevInCurrentSlot;
  }

  void Update() {
    GameObject ob = eventsys.currentSelectedGameObject;
    for (int i = 0; i < buttons.Length; ++i) {
      if (ob == buttons[i]) {
        currentSlot = i;
        break;
      }
    }

    customiserChar.Rotate(0, rotationSpeed*input.RSX, 0);
  }

  public void ToggleBits() {
    bitsCheckbox.SetActive(!bitsCheckbox.activeSelf);
    NextInSlot(6);
  }

  public void NextInCurrentSlot() {
    if (currentSlot != 7)
      NextInSlot(currentSlot-1);
  }

  public void PrevInCurrentSlot() {
    if (currentSlot != 7)
      PrevInSlot(currentSlot-1);
  }

  public void NextInSlot(int slot) {
    if (slot < slotNames.Length) {
      slots[slotNames[slot]].SelectNextOption();
    }
  }
  
  public void PrevInSlot(int slot) {
    if (slot < slotNames.Length) {
      slots[slotNames[slot]].SelectPreviousOption();
    }
  }
}
