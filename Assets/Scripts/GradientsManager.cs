using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientsManager : MonoBehaviour {
  public static GradientsManager instance;
  public Texture2D[] gradients;

  void Awake() {
    instance = instance != null ? instance : this;
  }
}
