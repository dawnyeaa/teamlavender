using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomiseSlot : MonoBehaviour {
  public string slotName;

  [SerializeField]
  private List<GameObject> optionMeshes;
  private List<GameObject> options;
  public int selected;
  public readonly int defaultOption;

  void Awake() {
    options = new List<GameObject>(optionMeshes.Count);
    selected = defaultOption;
  }

  void Start() {
    InstantiateOptions();
    SetActiveStates();
  }

  private void InstantiateOptions() {
    for (int i = 0; i < optionMeshes.Count; ++i) {
      options.Insert(i, Instantiate(optionMeshes[i], transform));
    }
  }

  private void SetActiveStates() {
    for (int i = 0; i < options.Count; ++i) {
      options[i].SetActive(i == selected);
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
}
