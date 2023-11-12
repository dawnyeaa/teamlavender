using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ShowSliderValue : MonoBehaviour {
  public Slider slider;
  private TextMeshProUGUI displayText;

  void Awake() {
    displayText = GetComponent<TextMeshProUGUI>();
  }

  public void UpdateSliderDisplay() {
    displayText.text = $"{(int)slider.value}";
  }
}
