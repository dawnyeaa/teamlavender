
using UnityEngine;
public class CameraTargetController : MonoBehaviour {
  public Transform facing;
  public bool update = true;

  private Quaternion savedRot;

  void Update() {
    if (update) {
      transform.rotation = facing.rotation;
      savedRot = transform.rotation;
    }
    else {
      transform.rotation = savedRot;
    }
  }
}