using UnityEngine;

[ExecuteAlways]
public class HueSlider : MonoBehaviour {
  [SerializeField, Range(0,1)] private float t = 0;
  [SerializeField] private MeshRenderer huesRenderer;
  [SerializeField] private MeshRenderer hueCircleRenderer;
  [SerializeField] private Vector2 sliderBounds;
  [SerializeField] private RectTransform hueDropper;

  void OnValidate() {
    hueDropper.anchoredPosition = new(Mathf.Lerp(sliderBounds.x, sliderBounds.y, t), hueDropper.anchoredPosition.y);
    hueCircleRenderer.material.SetFloat("_T", t);
  }

  public float GetHueT() => t;

  public void SetHueT(float newT) => t = newT;
}
