using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;

public class ComboDisplayManager : MonoBehaviour {
  public SkateboardStateMachine sm;
  public ComboController comboController;
  public TextMeshProUGUI comboNameDisplay;
  private int currentHalfturns = 0;
  private bool currentFS = true;
  public float comboNameDisplayTime = 5f;
  [ReadOnly, SerializeField] private float comboDisplayTimer = 0;

  void Update() {
    if (comboDisplayTimer >= comboNameDisplayTime)
      ResetComboDisplay();
    else
      comboDisplayTimer += Time.deltaTime;
  }

  public void SetTurnModifiers(int turn, bool fs) {
    currentHalfturns = turn;
    currentFS = fs;
  }

  public void SetComboDisplay() {
    if (comboNameDisplay != null && comboController.currentlyPlayingCombo != null && sm.CurrentlyPlayingTrick != null) {
      var displayName = comboController.currentlyPlayingCombo._ComboDisplayName;
      var turnModifier = "";
      if (currentHalfturns > 0) turnModifier = (currentFS?"FS":"BS") + " " + currentHalfturns*180 + " ";
      comboNameDisplay.text = turnModifier + displayName;
      if (displayName.Length > 0)
        comboDisplayTimer = 0;
    }
  }

  public void ResetComboDisplay() {
    if (comboNameDisplay != null) {
      comboNameDisplay.text = "";
      currentHalfturns = 0;
    }
  }
}
