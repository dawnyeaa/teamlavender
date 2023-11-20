using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardCustoUI : MonoBehaviour {
  public Renderer boardCustomiserDeckRenderer;
  public Renderer boardCustoFrameDeck;
  public GameObject[] slotDisplays;
  public UIExtraInput input;
  public float rotationSpeed = 1;
  private Dictionary<string, CustoSlotInfo> startingSelection = new();
  private int currentSlot = 0;
  public GameObject hueObject;
  public HueSlider hueSlider;
  private int currentDeck = 0;
  public float hueSliderSpeed = 0.1f;
  public bool inHueMode = false;
  public CustoDeck[] custoDecks;
  public float[] defaultHues;
  public TextMeshProUGUI deckTitle;
  private float[] hueValues = new float[2];
  private static readonly string[] slotNames = {
    "deck",
    "trucks",
    "wheels"
  };
  private static readonly List<string> hueSlots = new(){
    "trucks",
    "wheels"
  };
  private static readonly string[] hueSlotProperties = {
    "_TruckGradientX",
    "_WheelGradientX"
  };

  void Start() {
    // character = customiserChar.GetComponent<CustomiseCharacter>();
    // var slotsarray = customiserChar.GetComponents<CustomiseSlot>();
    // slots = new();
    // if (!PlayerPrefs.HasKey("pelt")) {
    //   PlayerPrefs.SetFloat("pelt", character.colorT);
    // }
    // else {
    //   character.CustomiseColor(PlayerPrefs.GetFloat("pelt"));
    // }
    // foreach (CustomiseSlot slot in slotsarray) {
    //   slots.Add(slot.slotName, slot);
    //   var selectionKey = $"{slot.slotName}.selection";
    //   var hueKey = $"{slot.slotName}.hue";
    //   if (!PlayerPrefs.HasKey(selectionKey)) {
    //     PlayerPrefs.SetInt(selectionKey, slot.defaultOption);
    //   }
    //   else {
    //     slots[slot.slotName].SetSelected(PlayerPrefs.GetInt(selectionKey));
    //   }
    //   if (!PlayerPrefs.HasKey(hueKey)) {
    //     PlayerPrefs.SetFloat(hueKey, slot.GetT());
    //   }
    //   else {
    //     slots[slot.slotName].CustomiseColor(PlayerPrefs.GetFloat(hueKey));
    //   }
    // }
    // PlayerPrefs.Save();
    if (PlayerPrefs.HasKey("deck")) {
      UpdateDeck(PlayerPrefs.GetInt("deck"));
    }
    else {
      PlayerPrefs.SetInt("deck", 0);
    }
    for (int i = 0; i < hueSlots.Count; ++i) {
      if (PlayerPrefs.HasKey(hueSlots[i])) {
        hueValues[i] = PlayerPrefs.GetFloat(hueSlots[i]);
      }
      else {
        PlayerPrefs.SetFloat(hueSlots[i], defaultHues[i]);
        hueValues[i] = defaultHues[i];
      }
    }
    EnterMenu();
  }

  public void EnterMenu() {
    input.OnMenuRPerformed += NextInCurrentSlot;
    input.OnMenuLPerformed += PrevInCurrentSlot;
    input.OnMenuUPerformed += SelectPrevSlot;
    input.OnMenuDPerformed += SelectNextSlot;
    input.OnMenuBPerformed += ExitMenu;
    // SaveStartingSelection();
  }

  public void ExitMenu() {
    input.OnMenuRPerformed -= NextInCurrentSlot;
    input.OnMenuLPerformed -= PrevInCurrentSlot;
    input.OnMenuUPerformed -= SelectPrevSlot;
    input.OnMenuDPerformed -= SelectNextSlot;
    input.OnMenuBPerformed -= ExitMenu;
    // ResetCharCustoMenu();
    SaveSelected();
    SceneManager.LoadScene(0);
  }

  void Update() {
    if (inHueMode) {
      hueSlider.SetHueT(hueSlider.GetHueT()+input.HueSliderX*hueSliderSpeed*Time.deltaTime);
      SetHue(hueSlots.IndexOf(slotNames[currentSlot]), hueSlider.GetHueT());
    }
    // if (inHueMode) {
    //   hueSlider.SetHueT(hueSlider.GetHueT()+input.HueSliderX*hueSliderSpeed*Time.deltaTime);
    //   if (slotNames[currentSlot] == "pelt") {
    //     character.CustomiseColor(hueSlider.GetHueT());
    //   }
    //   else {
    //     slots[slotNames[currentSlot]].CustomiseColor(hueSlider.GetHueT());
    //   }
    // }
    // customiserChar.Rotate(0, rotationSpeed*input.RSX, 0);
  }

  private void ToggleHueMode() {
    // if (noHueSlots.Contains(slotNames[currentSlot])) return;
    // inHueMode = !inHueMode;
    // selectionObject.SetActive(!inHueMode);
    // hueObject.SetActive(inHueMode);
    // if (inHueMode) {
    //   if (slotNames[currentSlot] == "pelt") {
    //     hueSlider.SetHueT(character.colorT);
    //   }
    //   else {
    //     hueSlider.SetHueT(slots[slotNames[currentSlot]].GetT());
    //   }
    // }
  }

  private void ResetCharCustoMenu() {
    // move it out of hue mode
    if (inHueMode) ToggleHueMode();
    // set it back to 'dome'
    SelectSlot(0);
  }

  public void NextInCurrentSlot() {
    if (inHueMode) return;
    NextDeck();
  }

  public void PrevInCurrentSlot() {
    if (inHueMode) return;
    PrevDeck();
  }

  public void SelectNextSlot() {
    currentSlot = (currentSlot + 1) % slotNames.Length;
    DisplayCurrentSlot();
  }

  public void SelectPrevSlot() {
    currentSlot = (slotNames.Length + (currentSlot - 1)) % slotNames.Length;
    DisplayCurrentSlot();
  }

  private void UpdateDeck(int deck) {
    boardCustomiserDeckRenderer.material.SetTexture("_DeckTex", custoDecks[deck].deckTexture);
    boardCustoFrameDeck.material.SetTexture("_BaseMap", custoDecks[deck].deckTexture);
    deckTitle.text = $"{custoDecks[deck].deckTitle}{(custoDecks[deck].deckArtist.Length != 0 ? "\nby" : "")} {custoDecks[deck].deckArtist}";
  }

  private void NextDeck() {
    currentDeck = (currentDeck + 1) % custoDecks.Length;
    UpdateDeck(currentDeck);
  }

  private void PrevDeck() {
    currentDeck = (custoDecks.Length + (currentDeck - 1)) % custoDecks.Length;
    UpdateDeck(currentDeck);
  }

  private void SelectSlot(int slot) {
    currentSlot = slot;
    DisplayCurrentSlot();
  }

  private void SaveStartingSelection() {
    startingSelection = new();
    // foreach (var slot in slots) {
    //   var info = new CustoSlotInfo {
    //       selection = slot.Value.GetSelected(),
    //       hue = slot.Value.GetT()
    //   };
    //   startingSelection.Add(slot.Key, info);
    // }
    // var skinInfo = new CustoSlotInfo {
    //     selection = 0,
    //     hue = character.colorT
    // };
    // startingSelection.Add("pelt", skinInfo);
  }

  public void SetHue(int index, float t) {
    boardCustomiserDeckRenderer.material.SetFloat(hueSlotProperties[index], t);
  }

  public void SaveSelected() {
    PlayerPrefs.SetInt("deck", currentDeck);
    for (int i = 0; i < hueSlots.Count; ++i) {
      PlayerPrefs.SetFloat(hueSlots[i], hueValues[i]);
    }
    PlayerPrefs.Save();
  }

  public void ResetSelected() {
    // if (startingSelection != null && startingSelection.Count == slotNames.Length) {
    //   character.CustomiseColor(startingSelection["pelt"].hue);
    //   foreach (var slot in slots) {
    //     slots[slot.Key].UpdateSelected(startingSelection[slot.Key].selection);
    //     slots[slot.Key].CustomiseColor(startingSelection[slot.Key].hue);
    //   }
    // }
  }

  private void DisplayCurrentSlot() {
    for (int i = 0; i < slotDisplays.Length; ++i) {
      slotDisplays[i].SetActive(currentSlot == i);
    }
    if (hueSlots.Contains(slotNames[currentSlot])) {
      inHueMode = true;
      // we need the hue slider
      hueObject.SetActive(true);
      hueSlider.SetHueT(hueValues[hueSlots.IndexOf(slotNames[currentSlot])]);
    }
    else {
      inHueMode = false;
      // we need the frame
      hueObject.SetActive(false);
    }
  }
}
