using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectionHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
  public Behaviour selectionDisplay;
  public ScrollRectHandler scrollBox;
  private bool setup = false;
  GameObject selectionArrow;
  Selectable selectable;

  void Awake() {
    Setup();
  }

  private void Setup() {
    if (!setup) {
      if (!selectionDisplay)
        selectionArrow = transform.GetChild(0).gameObject;
      selectable = GetComponent<Selectable>();
      setup = true;
    }
  }

  private void SetDisplay(bool active) {
    if (selectionDisplay)
      selectionDisplay.enabled = active;
    else
      selectionArrow.SetActive(active);
  }

  public void ManualSelect() {
    Setup();
    selectable.Select();
    SetDisplay(true);
  }

  public void Deselect() {
    Setup();
    SetDisplay(false);
  }

  public void OnSelect(BaseEventData eventData) {
    Setup();
    SetDisplay(true);
    if (scrollBox)
      scrollBox.ScrollToElement(selectionDisplay.transform as RectTransform);
  }

  public void OnDeselect(BaseEventData eventData) {
    Deselect();
  }
}
