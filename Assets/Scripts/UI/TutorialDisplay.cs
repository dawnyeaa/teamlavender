using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TutorialDisplay : MonoBehaviour {
  public GameObject starterTrickDisplay;
  public GameObject helpScreenPrompt;
  public GameObject helpScreen;
  public InputController input;
  private bool showingTrickDisplay = false;
  private bool showingHelpScreenPrompt = false;
  private bool showingHelpScreen = false;
  private bool overriding = false;
  private bool newbie = true;

  void Start() {
    ShowTrickDisplay();
  }

  void OnEnable() {
    input.OnToggleHelpPerformed += ToggleHelpScreen;
  }

  void OnDisable() {
    input.OnToggleHelpPerformed -= ToggleHelpScreen;
  }

  private async void ShowTrickDisplay() {
    await Task.Delay((int)(5000));
    if (newbie) {
      showingTrickDisplay = true;
      SetTutorialDisplays();
    }
    ShowHelpButton();
  }

  private async void ShowHelpButton() {
    await Task.Delay((int)(5000));
    showingHelpScreenPrompt = true;
    SetTutorialDisplays();
  }
  public void ToggleHelpScreen() {
    showingHelpScreen = !showingHelpScreen;
    showingHelpScreenPrompt = true;
    SetTutorialDisplays();
  }
  public void OverrideHideAll() {
    overriding = true;
    starterTrickDisplay.SetActive(false);
    helpScreenPrompt.SetActive(false);
    helpScreen.SetActive(false);
  }
  public void OverrideShowAll() {
    overriding = false;
    SetTutorialDisplays();
  }
  public void HideTrickHelp() {
    newbie = false;
    showingTrickDisplay = false;
    SetTutorialDisplays();
  }

  private void SetTutorialDisplays() {
    if (!overriding) {
      starterTrickDisplay.SetActive(showingTrickDisplay);
      helpScreenPrompt.SetActive(showingHelpScreenPrompt);
      helpScreen.SetActive(showingHelpScreen);
    }
  }
}
