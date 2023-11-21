using System.Collections.Generic;
using UnityEngine;

public class CustomiseCharacter : MonoBehaviour {
  public bool isPlayerCharacter = false;
  private Dictionary<string, CustomiseSlot> slots;
  public GameObject characterMesh;
  public GameObject boardMesh;
  [Range(0, 1)] public float colorT;
  public int gradientIndex = 0;
  private Material charMaterial;
  private Material boardMaterial;
  public int deckIndex;
  [Range(0, 1)] public float truckColorT;
  [Range(0, 1)] public float wheelsColorT;

  void Awake() {
    charMaterial = characterMesh.GetComponent<SkinnedMeshRenderer>().material;
    boardMaterial = boardMesh.GetComponent<SkinnedMeshRenderer>().material;
  }
  
  void Start() {
    var slotsarray = GetComponents<CustomiseSlot>();
    slots = new();
    foreach (CustomiseSlot slot in slotsarray) {
      slots.Add(slot.slotName, slot);
      if (isPlayerCharacter) {
        var selectionKey = $"{slot.slotName}.selection";
        var hueKey = $"{slot.slotName}.hue";
        var gradientKey = $"{slot.slotName}.gradient";
        if (PlayerPrefs.HasKey(selectionKey)) {
          slot.UpdateSelected(PlayerPrefs.GetInt(selectionKey));
        }
        if (PlayerPrefs.HasKey(hueKey)) {
          slot.CustomiseColor(PlayerPrefs.GetFloat(hueKey));
        }
        if (PlayerPrefs.HasKey(gradientKey)) {
          slot.SetGradient(PlayerPrefs.GetInt(gradientKey));
        }
      }
    }
    if (isPlayerCharacter) {
      if (PlayerPrefs.HasKey("pelt")) {
        CustomiseColor(PlayerPrefs.GetFloat("pelt"));
      }
      if (PlayerPrefs.HasKey("pelt.gradient")) {
        SetGradient(PlayerPrefs.GetInt("pelt.gradient"));
      }
      if (PlayerPrefs.HasKey("deck")) {
        deckIndex = PlayerPrefs.GetInt("deck");
      }
      if (PlayerPrefs.HasKey("trucks")) {
        boardMaterial.SetFloat("_TruckGradientX", PlayerPrefs.GetFloat("trucks"));
      }
      if (PlayerPrefs.HasKey("wheels")) {
        boardMaterial.SetFloat("_WheelGradientX", PlayerPrefs.GetFloat("wheels"));
      }
      SetDeck(deckIndex);
    }
  }

  void OnValidate() {
    CustomiseColor(colorT);
    // if (Application.isPlaying) {
    //   SetGradient(gradientIndex);
    //   SetDeck(deckIndex);
    // }
  }

  public void CustomiseColor(float newT) {
    colorT = newT;
    if (charMaterial == null) return;
    charMaterial.SetFloat("_GradientX", newT);
  }

  public void SetGradient(int index) {
    gradientIndex = index;
    if (charMaterial == null) return;
    if (GradientsManager.instance)
      charMaterial.SetTexture("_GradientTex", GradientsManager.instance.gradients[gradientIndex]);
  }

  public void SetDeck(int deck) {
    deckIndex = deck;
    boardMaterial.SetTexture("_DeckTex", CustoDeckStorage.instance.custoDecks[deckIndex].deckTexture);
  }

  public void SetCutoutChannel(MaskChannel channel, float threshold) {
    if (charMaterial == null) return;
    switch (channel) {
      case MaskChannel.R:
        charMaterial.SetFloat("_CutoutR", threshold);
        break;
      case MaskChannel.G:
        charMaterial.SetFloat("_CutoutG", threshold);
        break;
      case MaskChannel.B:
        charMaterial.SetFloat("_CutoutB", threshold);
        break;
    }
  }

  public float GetT() => colorT;
}
