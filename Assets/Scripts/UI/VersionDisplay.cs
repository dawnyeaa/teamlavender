using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class VersionDisplay : MonoBehaviour {
  // Start is called before the first frame update
  void Awake() {
    var textDisplay = GetComponent<TMPro.TextMeshProUGUI>();
    textDisplay.text = $"v{Application.version}";
  }
}
