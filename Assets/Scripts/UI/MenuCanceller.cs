using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MenuCanceller : MonoBehaviour, ICancelHandler {
  public UnityEvent CancelEvent;
  public ButtonSelectionHandler buttonSelectionHandler;
  public void OnCancel(BaseEventData eventData) {
    if (buttonSelectionHandler == null) buttonSelectionHandler = GetComponent<ButtonSelectionHandler>();
    buttonSelectionHandler?.Deselect();
    CancelEvent.Invoke();
  }
}
