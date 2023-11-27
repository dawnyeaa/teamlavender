using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineLineBarDisplay : MonoBehaviour {
  public Renderer barRenderer, barShadowRenderer;
  public float falloff = 0.3f;
  private Material barMat, barShadowMat;
  private float lastFramePoints = 0;

  void Start() {
    barMat = barRenderer.material;
    barShadowMat = barShadowRenderer.material;
  }

  void Update() {
    lastFramePoints *= falloff;
  }

  public void UpdatePoints(float deltaPoints, float totalPoints) {
    if (barMat && barShadowMat) {
      barMat.SetFloat("_current", totalPoints);
      barShadowMat.SetFloat("_current", totalPoints);
      lastFramePoints += deltaPoints;
      barMat.SetFloat("_colorAmount", lastFramePoints/5f);
    }
  }
}
