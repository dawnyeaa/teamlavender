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
  public InputController input;
  public float movementScale = 100f;
  public Vector2 newNumpadPos = Vector2.zero, newRawPos = Vector2.zero;

  public Transform numpadCursor, rawCursor;
  void Update() {
    if (input.rsNumpad == 0) {
      // dont move the pointer, just show the dead representation
    }
    else {
      newNumpadPos = STICK_DIRECTIONS[input.rsNumpad-1]*movementScale;
      numpadCursor.localPosition = new(newNumpadPos.x, newNumpadPos.y, numpadCursor.localPosition.z);
    }
    newRawPos = input.rsRaw*movementScale;
    rawCursor.localPosition = new(newRawPos.x, newRawPos.y, rawCursor.localPosition.z);
  }
}
