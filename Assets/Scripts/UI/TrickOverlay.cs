using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrickOverlayElement {
  public string id;
  public GameObject display;
}

public class TrickOverlay : MonoBehaviour {
  public PauseMenuManager pauseMenuManager;
  public InputController input;
  public GameObject overlay;
  public List<TrickOverlayElement> trickOverlays;
  private TutorialDisplay tuteDisplay;

  void Start() {
    tuteDisplay = GetComponent<TutorialDisplay>();
  }

  void OnEnable() {
    input.OnTrickOverlayHidePerformed += HideDisplay;
  }

  void OnDisable() {
    input.OnTrickOverlayHidePerformed -= HideDisplay;
  }

  private void HideDisplay() {
    overlay.SetActive(false);
  }

  public void ShowTrick(string id) {
    overlay.SetActive(true);
    foreach (var trick in trickOverlays) {
      trick.display.SetActive(trick.id == id);
    }
    tuteDisplay.HideTrickHelp();
    pauseMenuManager.ResumeGame();
  }
}
