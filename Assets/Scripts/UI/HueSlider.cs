using UnityEngine;

public class HueSlider : MonoBehaviour {
  [SerializeField, Range(0,1)] private float t = 0;
  [SerializeField] private MeshRenderer huesRenderer;
  [SerializeField] private MeshRenderer hueCircleRenderer;
  [SerializeField] private Vector2 sliderBounds;
  [SerializeField] private RectTransform hueDropper;

  void OnValidate() {
    UpdateHueSlider();
  }

  public float GetHueT() => t;

  public void SetHueT(float newT) {
    t = Mathf.Clamp01(newT);
    UpdateHueSlider();
  }

  private void UpdateHueSlider() {
    hueDropper.anchoredPosition = new(Mathf.Lerp(sliderBounds.x, sliderBounds.y, t), hueDropper.anchoredPosition.y);
    if (Application.isPlaying)
      hueCircleRenderer.material.SetFloat("_T", t);
  }
}
