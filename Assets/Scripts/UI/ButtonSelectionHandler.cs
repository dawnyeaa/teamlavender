using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectionHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    GameObject selectionArrow;

    void Awake() {
        selectionArrow = transform.GetChild(0).gameObject;
    }

    public void OnSelect(BaseEventData eventData) {
        selectionArrow.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData) {
        selectionArrow.SetActive(false);
    }
}
