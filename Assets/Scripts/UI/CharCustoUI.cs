using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharCustoUI : MonoBehaviour {
  public GameObject customiserChar;
  public EventSystem eventsys;
  public GameObject[] buttons;
  public UIExtraInput input;
  private CustomiseSlot[] slots;
  private int currentSlot = 0;
  void OnEnable() {
    slots = customiserChar.GetComponents<CustomiseSlot>();
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
  }

  public void NextInCurrentSlot() {
    NextInSlot(currentSlot-1);
  }

  public void PrevInCurrentSlot() {
    PrevInSlot(currentSlot-1);
  }

  public void NextInSlot(int slot) {
    if (slot < slots.Length) {
      slots[slot].SelectNextOption();
    }
  }
  
  public void PrevInSlot(int slot) {
    if (slot < slots.Length) {
      slots[slot].SelectPreviousOption();
    }
  }
}
