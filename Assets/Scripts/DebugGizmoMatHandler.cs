using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGizmoMatHandler : MonoBehaviour {
  public MeshRenderer mRenderer;
  public Color color;
  private Material cachedMatInstance;
  void Awake() {
    cachedMatInstance = mRenderer.material;
  }

  public void SetColor(Color setColor) {
    color = setColor;
    cachedMatInstance.SetColor("_BaseColor", color);
  }
}
