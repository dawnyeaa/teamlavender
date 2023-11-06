using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectionHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
  GameObject selectionArrow;
  Selectable selectable;

  void Awake() {
    selectionArrow = transform.GetChild(0).gameObject;
    selectable = GetComponent<Selectable>();
  }

  public void ManualSelect() {
    selectable.Select();
    selectionArrow.SetActive(true);
  }

  public void Deselect() {
    selectionArrow.SetActive(false);
  }

  public void OnSelect(BaseEventData eventData) {
    selectionArrow.SetActive(true);
  }

  public void OnDeselect(BaseEventData eventData) {
    selectionArrow.SetActive(false);
  }
}
