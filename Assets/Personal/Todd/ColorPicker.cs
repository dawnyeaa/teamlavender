using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColorPicker : UIBehaviour, IDragHandler {
  public CustomiseCharacter1 character;
  RectTransform RectTransform;
  public RectTransform handle;
  public float width, height;
  private bool initComplete = false;
  private Camera canvasCamera;

  protected override void Start() {
    RectTransform = (RectTransform)transform;
    SetWidthAndHeight();
    initComplete = true;

    canvasCamera = GetTopmostCanvas(this).worldCamera;
  }

  protected override void OnRectTransformDimensionsChange() {
    if (initComplete)
      SetWidthAndHeight();
    base.OnRectTransformDimensionsChange();
  }

  Vector2 PositionToUV(Vector2 pos) {
    Vector2 uv = new Vector2();
    uv = pos + new Vector2(width/2f, height/2f);
    uv /= new Vector2(width, height);
    uv.y = Mathf.Lerp(0.625f, 0.875f, uv.y);
    return uv;
  }

  public void OnDrag(PointerEventData data) {
    Vector2 mousePos = data.position;
    Vector2 localMousePosition;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, mousePos, canvasCamera, out localMousePosition);
    if (!RectTransformUtility.RectangleContainsScreenPoint(RectTransform, mousePos, canvasCamera)) {
      localMousePosition = Clamp(localMousePosition, new Vector2(-width/2, -height/2), new Vector2(width/2, height/2));
    }
    handle.localPosition = new Vector3(localMousePosition.x, localMousePosition.y, handle.localPosition.z);
    // character.CustomiseColor(PositionToUV(localMousePosition));
  }

  private void SetWidthAndHeight() {
    Vector3[] fourCornersArray = new Vector3[4];
    RectTransform.GetLocalCorners(fourCornersArray);
    height = (fourCornersArray[1] - fourCornersArray[0]).magnitude;
    width = (fourCornersArray[3] - fourCornersArray[0]).magnitude;
  }

  private static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) {
    return new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
  }

  private static Canvas GetTopmostCanvas(Component component) {
    Canvas[] parentCanvases = component.GetComponentsInParent<Canvas>();
    if (parentCanvases != null && parentCanvases.Length > 0) {
      return parentCanvases[parentCanvases.Length - 1];
    }
    return null;
  }
}
