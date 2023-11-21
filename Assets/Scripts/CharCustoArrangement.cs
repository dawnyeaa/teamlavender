using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCustoArrangement : MonoBehaviour {
  public static CharCustoArrangement instance;
  public float skinHue = 0;
  public Dictionary<string, int> selectedSlots = new();
  public Dictionary<string, float> hues = new();

  void Awake() {
    DontDestroyOnLoad(gameObject);
    instance ??= this;
  }
}
