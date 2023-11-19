using System.Collections.Generic;
using UnityEngine;

public class CustomiseCharacter : MonoBehaviour {
  public bool isPlayerCharacter = false;
  private Dictionary<string, CustomiseSlot> slots;
  public GameObject characterMesh;
  [Range(0, 1)] public float colorT;
  private Material charMaterial;

  void Awake() {
    charMaterial = characterMesh.GetComponent<SkinnedMeshRenderer>().material;
  }
  
  void Start() {
    var slotsarray = GetComponents<CustomiseSlot>();
    slots = new();
    foreach (CustomiseSlot slot in slotsarray) {
      slots.Add(slot.slotName, slot);
      if (isPlayerCharacter && CharCustoArrangement.instance != null && CharCustoArrangement.instance.selectedSlots.Count > 0) {
        slot.UpdateSelected(CharCustoArrangement.instance.selectedSlots[slot.slotName]);
        slot.CustomiseColor(CharCustoArrangement.instance.hues[slot.slotName]);
      }
    }
  }

  void OnValidate() {
    CustomiseColor(colorT);
  }

  public void CustomiseColor(float newT) {
    colorT = newT;
    if (charMaterial == null) return;
    charMaterial.SetFloat("_GradientX", newT);
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
