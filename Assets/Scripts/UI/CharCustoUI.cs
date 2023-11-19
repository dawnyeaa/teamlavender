using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class CustoSlotInfo {
  public int selection;
  public float hue;
}

public class CharCustoUI : MonoBehaviour {
  public Transform customiserChar;
  public GameObject[] slotDisplays;
  public UIExtraInput input;
  public float rotationSpeed = 1;
  private CustomiseCharacter character;
  private Dictionary<string, CustomiseSlot> slots;
  private Dictionary<string, CustoSlotInfo> startingSelection = new();
  private int currentSlot = 0;
  public GameObject selectionObject;
  public GameObject hueObject;
  public HueSlider hueSlider;
  public float hueSliderSpeed = 0.1f;
  public bool inHueMode = false;
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

  private static readonly List<string> noHueSlots = new() {
    "specs",
    "spitter",
    "bits"
  };

  void Start() {
    character = customiserChar.GetComponent<CustomiseCharacter>();
    var slotsarray = customiserChar.GetComponents<CustomiseSlot>();
    slots = new();
    if (!PlayerPrefs.HasKey("pelt")) {
      PlayerPrefs.SetFloat("pelt", character.colorT);
    }
    else {
      character.CustomiseColor(PlayerPrefs.GetFloat("pelt"));
    }
    foreach (CustomiseSlot slot in slotsarray) {
      slots.Add(slot.slotName, slot);
      var selectionKey = $"{slot.slotName}.selection";
      var hueKey = $"{slot.slotName}.hue";
      if (!PlayerPrefs.HasKey(selectionKey)) {
        PlayerPrefs.SetInt(selectionKey, slot.defaultOption);
      }
      else {
        slots[slot.slotName].SetSelected(PlayerPrefs.GetInt(selectionKey));
      }
      if (!PlayerPrefs.HasKey(hueKey)) {
        PlayerPrefs.SetFloat(hueKey, slot.GetT());
      }
      else {
        slots[slot.slotName].CustomiseColor(PlayerPrefs.GetFloat(hueKey));
      }
    }
    PlayerPrefs.Save();
  }

  public void EnterMenu() {
    input.OnMenuRPerformed += NextInCurrentSlot;
    input.OnMenuLPerformed += PrevInCurrentSlot;
    input.OnMenuUPerformed += SelectPrevSlot;
    input.OnMenuDPerformed += SelectNextSlot;
    input.OnMenuLBPerformed += ToggleHueMode;
    input.OnMenuRBPerformed += ToggleHueMode;
    SaveStartingSelection();
  }

  public void ExitMenu(bool saving = false) {
    input.OnMenuRPerformed -= NextInCurrentSlot;
    input.OnMenuLPerformed -= PrevInCurrentSlot;
    input.OnMenuUPerformed -= SelectPrevSlot;
    input.OnMenuDPerformed -= SelectNextSlot;
    input.OnMenuLBPerformed -= ToggleHueMode;
    input.OnMenuRBPerformed -= ToggleHueMode;
    ResetCharCustoMenu();
    if (saving) SaveSelected();
    else ResetSelected();
  }

  void Update() {
    if (inHueMode) {
      hueSlider.SetHueT(hueSlider.GetHueT()+input.HueSliderX*hueSliderSpeed*Time.deltaTime);
      if (slotNames[currentSlot] == "pelt") {
        character.CustomiseColor(hueSlider.GetHueT());
      }
      else {
        slots[slotNames[currentSlot]].CustomiseColor(hueSlider.GetHueT());
      }
    }
    // customiserChar.Rotate(0, rotationSpeed*input.RSX, 0);
  }

  private void ToggleHueMode() {
    if (noHueSlots.Contains(slotNames[currentSlot])) return;
    inHueMode = !inHueMode;
    selectionObject.SetActive(!inHueMode);
    hueObject.SetActive(inHueMode);
    if (inHueMode) {
      if (slotNames[currentSlot] == "pelt") {
        hueSlider.SetHueT(character.colorT);
      }
      else {
        hueSlider.SetHueT(slots[slotNames[currentSlot]].GetT());
      }
    }
  }

  private void ResetCharCustoMenu() {
    // move it out of hue mode
    if (inHueMode) ToggleHueMode();
    // set it back to 'dome'
    SelectSlot(0);
  }

  public void NextInCurrentSlot() {
    if (inHueMode) return;
    NextInSlot(currentSlot);
  }

  public void PrevInCurrentSlot() {
    if (inHueMode) return;
    PrevInSlot(currentSlot);
  }

  public void SelectNextSlot() {
    if (inHueMode) return;
    currentSlot = (currentSlot + 1) % slotNames.Length;
    DisplayCurrentSlot();
  }

  public void SelectPrevSlot() {
    if (inHueMode) return;
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

  private void SelectSlot(int slot) {
    currentSlot = slot;
    DisplayCurrentSlot();
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
      PlayerPrefs.SetInt($"{slot.Key}.selection", slot.Value.GetSelected());
      PlayerPrefs.SetFloat($"{slot.Key}.hue", slot.Value.GetT());
    }
    PlayerPrefs.SetFloat("pelt", character.colorT);
    PlayerPrefs.Save();
  }

  public void ResetSelected() {
    if (startingSelection != null && startingSelection.Count == slotNames.Length) {
      character.CustomiseColor(startingSelection["pelt"].hue);
      foreach (var slot in slots) {
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
