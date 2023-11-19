using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaskChannel {
  None,
  R,
  G,
  B
}

public class CustomiseSlot : MonoBehaviour {
  public string slotName;

  [SerializeField]
  private List<GameObject> optionMeshes;
  [SerializeField]
  private List<float> cutoutThresholds;
  [SerializeField]
  private MaskChannel cutoutChannel;
  private List<GameObject> options;
  private CustomiseCharacter character;
  [ReadOnly] private int selected;
  public int defaultOption;
  [Range(0, 1)] public float t;
  
  private List<Material> optionMaterials;

  void Awake() {
    options = new List<GameObject>(optionMeshes.Count);
    optionMaterials = new List<Material>(optionMeshes.Count);
    
    character = GetComponent<CustomiseCharacter>();

    selected = defaultOption;
  }

  void Start() {
    InstantiateOptions();
    SetActiveStates();
    CustomiseColor(t);
  }

  void OnValidate() {
    CustomiseColor(t);
  }

  private void InstantiateOptions() {
    for (int i = 0; i < optionMeshes.Count; ++i) {
      options.Insert(i, optionMeshes[i]);
      if (!optionMeshes[i]) continue;
      var skinnedRenderer = optionMeshes[i].GetComponent<SkinnedMeshRenderer>();
      var renderer = optionMeshes[i].GetComponent<MeshRenderer>();
      Material mat;
      if (skinnedRenderer) {
        mat = skinnedRenderer.material;
      }
      else if (renderer) {
        mat = renderer.material;
      }
      else {
        continue;
      }
      optionMaterials.Insert(i, mat);
    }
  }

  private void SetActiveStates() {
    for (int i = 0; i < options.Count; ++i) {
      if (options[i] != null) options[i].SetActive(i == selected);
    }
    if (selected < cutoutThresholds.Count && cutoutChannel != MaskChannel.None) {
      character.SetCutoutChannel(cutoutChannel, cutoutThresholds[selected]);
    }
  }

  public void SelectNextOption() {
    selected = selected == (options.Count-1) ? 0 : selected + 1;
    SetActiveStates();
  }

  public void SelectPreviousOption() {
    selected = selected == 0 ? options.Count-1 : selected - 1;
    SetActiveStates();
  }

  public int GetSelected() {
    return selected;
  }

  public void SetSelected(int option) {
    selected = option;
  }

  public void UpdateSelected(int option) {
    SetSelected(option);
    SetActiveStates();
  }

  public void CustomiseColor(float newT) {
    t = newT;
    if (optionMaterials == null) return;
    foreach (var optionMaterial in optionMaterials) {
      optionMaterial.SetFloat("_GradientX", newT);
    }
  }

  public float GetT() => t;
}
