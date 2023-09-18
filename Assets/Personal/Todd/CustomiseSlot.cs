using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomiseSlot : MonoBehaviour {
  public string slotName;

  [SerializeField]
  private List<GameObject> optionMeshes;
  private List<GameObject> options;
  private int selected;
  public int defaultOption;
  
  private List<Material> optionMaterials;

  void Awake() {
    options = new List<GameObject>(optionMeshes.Count);
    // optionMaterials = new List<Material>();
    selected = defaultOption;
  }

  void Start() {
    InstantiateOptions();
    SetActiveStates();
  }

  private void InstantiateOptions() {
    for (int i = 0; i < optionMeshes.Count; ++i) {
      options.Insert(i, optionMeshes[i]);
      // optionMaterials.Insert(i, options[i].GetComponent<MeshRenderer>().material);
    }
  }

  private void SetActiveStates() {
    for (int i = 0; i < options.Count; ++i) {
      if (options[i] != null) options[i].SetActive(i == selected);
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

  public void CustomiseColor(Vector2 uv) {
    // foreach (Material optionMaterial in optionMaterials) {
    //   optionMaterial.SetVector("_PickSkin", new Vector4(uv.x, uv.y, 0, 0));
    // }
  }
}
