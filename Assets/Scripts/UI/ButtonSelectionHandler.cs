using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectionHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
  private bool setup = false;
  GameObject selectionArrow;
  Selectable selectable;

  void Awake() {
    Setup();
  }

  private void Setup() {
    if (!setup) {
      selectionArrow = transform.GetChild(0).gameObject;
      selectable = GetComponent<Selectable>();
      setup = true;
    }
  }

  public void ManualSelect() {
    Setup();
    selectable.Select();
    selectionArrow.SetActive(true);
  }

  public void Deselect() {
    Setup();
    selectionArrow.SetActive(false);
  }

  public void OnSelect(BaseEventData eventData) {
    Setup();
    selectionArrow.SetActive(true);
  }

  public void OnDeselect(BaseEventData eventData) {
    Setup();
    selectionArrow.SetActive(false);
  }
}
