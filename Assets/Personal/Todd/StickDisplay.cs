using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickDisplay : MonoBehaviour {
  public InputController input;
  public float movementScale = 100f;
  public Vector2 newpos = Vector2.zero;
  // Update is called once per frame
  void Update() {
    newpos = input.rightStick*movementScale;
    transform.localPosition = new Vector3 (newpos.x, newpos.y, transform.localPosition.z);
  }
}
