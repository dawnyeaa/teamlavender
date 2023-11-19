using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharCustoUI : MonoBehaviour {
  public Transform customiserChar;
  public GameObject[] slotDisplays;
  public UIExtraInput input;
  public float rotationSpeed = 1;
  private CustomiseCharacter character;
  private Dictionary<string, CustomiseSlot> slots;
  private Dictionary<string, CustoSlotInfo> startingSelection = new();
  private int currentSlot = 0;
  private static readonly string[] slotNames = {
    "dome",
    "specs",
    "spitter",
    "top",
    "low-duds",
    "kicks",
    "bits",
    "pelt"
  };
  void Start() {
    character = customiserChar.GetComponent<CustomiseCharacter>();
    var slotsarray = customiserChar.GetComponents<CustomiseSlot>();
    slots = new();
    bool newCustoInstance = CharCustoArrangement.instance.selectedSlots.Count == 0;
    if (newCustoInstance) {
      CharCustoArrangement.instance.skinHue = character.colorT;
    }
    else {
      character.CustomiseColor(CharCustoArrangement.instance.skinHue);
    }
    foreach (CustomiseSlot slot in slotsarray) {
      slots.Add(slot.slotName, slot);
      if (newCustoInstance) {
        CharCustoArrangement.instance.selectedSlots.Add(slot.slotName, slot.defaultOption);
        CharCustoArrangement.instance.hues.Add(slot.slotName, slot.GetT());
      }
      else {
        slots[slot.slotName].SetSelected(CharCustoArrangement.instance.selectedSlots[slot.slotName]);
        slots[slot.slotName].CustomiseColor(CharCustoArrangement.instance.hues[slot.slotName]);
      }
    }
  }

  public void EnterMenu() {
    input.OnMenuRPerformed += NextInCurrentSlot;
    input.OnMenuLPerformed += PrevInCurrentSlot;
    input.OnMenuUPerformed += SelectPrevSlot;
    input.OnMenuDPerformed += SelectNextSlot;
    SaveStartingSelection();
  }

  public void ExitMenu(bool saving = false) {
    input.OnMenuRPerformed -= NextInCurrentSlot;
    input.OnMenuLPerformed -= PrevInCurrentSlot;
    input.OnMenuUPerformed -= SelectPrevSlot;
    input.OnMenuDPerformed -= SelectNextSlot;
    if (saving) SaveSelected();
    else ResetSelected();
  }

  void Update() {
    // customiserChar.Rotate(0, rotationSpeed*input.RSX, 0);
  }

  public void NextInCurrentSlot() {
    NextInSlot(currentSlot);
  }

  public void PrevInCurrentSlot() {
    PrevInSlot(currentSlot);
  }

  public void SelectNextSlot() {
    currentSlot = (currentSlot + 1) % slotNames.Length;
    DisplayCurrentSlot();
  }

  public void SelectPrevSlot() {
    currentSlot = (slotNames.Length + (currentSlot - 1)) % slotNames.Length;
    DisplayCurrentSlot();
  }

  public void NextInSlot(int slot) {
    if (slot < slotNames.Length && slotNames[slot] != "pelt") {
      slots[slotNames[slot]].SelectNextOption();
    }
  }
  
  public void PrevInSlot(int slot) {
    if (slot < slotNames.Length && slotNames[slot] != "pelt") {
      slots[slotNames[slot]].SelectPreviousOption();
    }
  }

  private void SaveStartingSelection() {
    startingSelection = new();
    foreach (var slot in slots) {
      var info = new CustoSlotInfo {
          selection = slot.Value.GetSelected(),
          hue = slot.Value.GetT()
      };
      startingSelection.Add(slot.Key, info);
    }
    var skinInfo = new CustoSlotInfo {
        selection = 0,
        hue = character.colorT
    };
    startingSelection.Add("pelt", skinInfo);
  }

  public void SaveSelected() {
    foreach (var slot in slots) {
      CharCustoArrangement.instance.selectedSlots[slot.Key] = slot.Value.GetSelected();
      CharCustoArrangement.instance.hues[slot.Key] = slot.Value.GetT();
    }
    CharCustoArrangement.instance.skinHue = character.colorT;
  }

  public void ResetSelected() {
    if (startingSelection != null && startingSelection.Count == slotNames.Length) {
      Debug.Log("resetting");
      character.CustomiseColor(startingSelection["pelt"].hue);
      foreach (var slot in slots) {
        Debug.Log($"setting {slot.Key} selection to {startingSelection[slot.Key].selection}");
        Debug.Log($"setting {slot.Key} hue to {startingSelection[slot.Key].hue}");
        slots[slot.Key].UpdateSelected(startingSelection[slot.Key].selection);
        slots[slot.Key].CustomiseColor(startingSelection[slot.Key].hue);
      }
    }
  }

  private void DisplayCurrentSlot() {
    for (int i = 0; i < slotDisplays.Length; ++i) {
      slotDisplays[i].SetActive(currentSlot == i);
    }
  }
}
