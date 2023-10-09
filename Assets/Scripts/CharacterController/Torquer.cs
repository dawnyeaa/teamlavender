using UnityEngine;

public class Torquer : MonoBehaviour {

  public float angularDrag = 0.3f;
  public bool update = true;

  // primary
  [ReadOnly] public float orientation = 0;

  // secondary
  [ReadOnly] public float angularVelocity = 0;
  [ReadOnly] public float torque = 0;

  [ReadOnly] public float lastFixedUpdateTime = 0;
  [ReadOnly] public float lastOrientation = 0;

  void FixedUpdate() {
    if (update) {
      AddTorque(angularVelocity * -angularDrag);
      angularVelocity += torque * Time.fixedDeltaTime;
      lastOrientation = orientation;
      orientation = (orientation + angularVelocity * Time.fixedDeltaTime) % 1;
      ClearTorque();
      lastFixedUpdateTime = Time.time;
    }
    else {
      angularVelocity = 0;
    }
  }

  void Update() {
    if (update) {
      var fixedUpdateElapseRatio = (Time.time - lastFixedUpdateTime)/Time.fixedDeltaTime;
      if (fixedUpdateElapseRatio < 1 && fixedUpdateElapseRatio > 0) {
        ShowDisplayRotation(Mathf.LerpAngle(lastOrientation*360f, orientation*360f, fixedUpdateElapseRatio));
      }
    }
  }

  private void ClearTorque() {
    torque = 0;
  }

  private void ShowRotation() {
    transform.localRotation = Quaternion.Euler(0, orientation*360f, 0);
  }

  private void ShowDisplayRotation(float displayOrientation) {
    transform.localRotation = Quaternion.Euler(0, displayOrientation, 0);
  }

  public void AddTorque(float newTorque) {
    torque += newTorque;
  }

  public void AddForceAtPosition(Vector2 force, Vector2 localPoint) {
    AddTorque(Vec2Cross(force, localPoint)/(2*Mathf.PI));
  }

  public void AddForceAtPosition(Vector3 force, Vector3 globalPoint) {
    var force2 = new Vector2(force.x, force.z);
    var localPoint = globalPoint - transform.position;
    var localPoint2 = new Vector2(localPoint.x, localPoint.z);
    AddTorque(Vec2Cross(force2, localPoint2)/(2*Mathf.PI));
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
