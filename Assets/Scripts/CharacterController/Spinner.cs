using UnityEngine;

public class Spinner : MonoBehaviour {

  // primary
  [ReadOnly] public float orientation = 0;

  // secondary
  [ReadOnly] public float angularVelocity = 0;

  [ReadOnly] public bool update = true;

  [ReadOnly] public float lastFixedUpdateTime = 0;
  [ReadOnly] public float lastOrientation = 0;

  public 

  void FixedUpdate() {
    if (update) {
      lastOrientation = orientation;
      orientation = (orientation + (angularVelocity / 360f)) % 1;
      ClearVelocity();
      lastFixedUpdateTime = Time.time;
    }
  }

  void Update() {
    var fixedUpdateElapseRatio = (Time.time - lastFixedUpdateTime)/Time.fixedDeltaTime;
    if (fixedUpdateElapseRatio < 1 && fixedUpdateElapseRatio > 0) {
      ShowDisplayRotation(Mathf.LerpAngle(lastOrientation*360f, orientation*360f, fixedUpdateElapseRatio));
    }
  }

  private void ClearVelocity() {
    angularVelocity = 0;
  }

  private void ShowRotation() {
    transform.localRotation = Quaternion.Euler(0, orientation*360f, 0);
  }

  private void ShowDisplayRotation(float displayOrientation) {
    transform.localRotation = Quaternion.Euler(0, displayOrientation, 0);
  }

  public void AddVelocity(float newVelocity) {
    angularVelocity += newVelocity;
  }

  public Vector2 GetPointVelocity(Vector2 localPoint) {
    var result3 = Vector3.Cross(angularVelocity * ThisIsJustTau.TAU * Vector3.up, new Vector3(localPoint.x, 0, localPoint.y));
    return new Vector2(result3.x, result3.z);
  }

  public Vector3 GetPointVelocity(Vector3 globalPoint) {
    return Vector3.Cross(angularVelocity * ThisIsJustTau.TAU * Vector3.up, globalPoint - transform.position);
  }

  public void Reset() {
    orientation = 0;
    angularVelocity = 0;
  }

  private static float Vec2Cross(Vector2 a, Vector2 b) {
    return a.x*b.y - a.y*b.x;
  }
}
