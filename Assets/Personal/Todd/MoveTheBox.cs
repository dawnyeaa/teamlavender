using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MoveTheBox : MonoBehaviour {
  // Update is called once per frame
  void Update() {
    float x = 3*Mathf.Cos(Time.time);
    float y = Mathf.Sin(2*Time.time);
    transform.position = new Vector3(x, 0, y);
  }
}
