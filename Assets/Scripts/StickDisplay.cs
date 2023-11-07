using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickDisplay : MonoBehaviour {
  private const float ONEONROOT2 = 0.7071067811865475f;
  private readonly static Vector2[] STICK_DIRECTIONS = {
    new(-ONEONROOT2, -ONEONROOT2),
    new(0, -1),
    new(ONEONROOT2, -ONEONROOT2),
    new(-1, 0),
    new(0, 0),
    new(1, 0),
    new(-ONEONROOT2, ONEONROOT2),
    new(0, 1),
    new(ONEONROOT2, ONEONROOT2)
  };
  private readonly static (uint, uint)[] ADJACENT_INDICES = {
    (3, 1),
    (2, 0),
    (5, 1),
    (6, 0),
    (0, 0),
    (8, 2),
    (7, 3),
    (8, 6),
    (7, 5)
  };
  public InputController input;
  public float movementScale = 100f;
  public Vector2 newNumpadPos = Vector2.zero, newRawPos = Vector2.zero;

  public Transform numpadCursor, numpadShadowCursor, rawCursor;
  public TrailRenderer numpadTrail, numpadShadowTrail;
  public float shadowDepth = -0.15f;
  public float shadowOffset = 0.01f;
  private int lastIndex;
  void Update() {
    if (input.rsNumpad == 0) {
      // dont move the pointer, just show the dead representation
    }
    else {
      if (input.rsNumpad != 5) {
        var newIndex = input.rsNumpad-1;
        if (lastIndex == ADJACENT_INDICES[newIndex].Item1 || lastIndex == ADJACENT_INDICES[newIndex].Item2) {
          // then the new index is adjacent to the last one
          var lastNumpadPos = newNumpadPos;
          newNumpadPos = STICK_DIRECTIONS[input.rsNumpad-1]*movementScale;
          for (int i = 1; i < 6; ++i) {
            var slerped = Vector3.Slerp(lastNumpadPos, newNumpadPos, (float)i/6);
            numpadTrail.AddPosition(transform.TransformPoint(slerped)+new Vector3(0, 0, numpadCursor.position.z));
            numpadShadowTrail.AddPosition(transform.TransformPoint(slerped)+new Vector3(shadowOffset, -shadowOffset, shadowDepth));
          }
        }
      }
      newNumpadPos = STICK_DIRECTIONS[input.rsNumpad-1]*movementScale;
      numpadCursor.localPosition = new(newNumpadPos.x, newNumpadPos.y, numpadCursor.localPosition.z);
      numpadShadowCursor.localPosition = new(newNumpadPos.x+shadowOffset, newNumpadPos.y-shadowOffset, shadowDepth);
      lastIndex = input.rsNumpad-1;
    }
    newRawPos = input.rsRaw*movementScale;
    rawCursor.localPosition = new(newRawPos.x, newRawPos.y, rawCursor.localPosition.z);
  }
}
