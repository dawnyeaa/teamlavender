using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MenuCanceller : MonoBehaviour, ICancelHandler {
  public UnityEvent CancelEvent;
  public void OnCancel(BaseEventData eventData) {
    Debug.Log("cancelling");
    CancelEvent.Invoke();
  }
}
