using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringyBody : MonoBehaviour {
  PIDController controller = new PIDController();
  public Transform target;
  public Transform constrainedObject;
  public float pushStrength = 1f;
  public float offset;
  public float proportionalGain, derivativeGain;
  public float acceleration;
  public float velocity;
  public Vector2 bounds = new Vector2(-0.05f, 0.05f);
  public float squatHeight;
  public float squatT;
  private Vector3 lastFramePos;
  void Awake() {
    lastFramePos = constrainedObject.position;
  }
  void FixedUpdate() {
    constrainedObject.position = (Vector3.Dot(lastFramePos-target.position, target.up)*target.up)+target.position;
    controller.proportionalGain = proportionalGain;
    controller.derivativeGain = derivativeGain;
    acceleration = 0f;
    float input = controller.Update(Time.fixedDeltaTime, constrainedObject.localPosition.y, Mathf.SmoothStep(0, squatHeight, squatT));
    constrainedObject.localPosition = new Vector3(0, constrainedObject.localPosition.y, 0);
    AddAcceleration(input * pushStrength);
    ResolveMovement();
    lastFramePos = constrainedObject.position;
    ClampMovement();
  }

  void AddAcceleration(float newAcceleration) {
    acceleration += newAcceleration;
  }

  void ResolveMovement() {
    velocity += acceleration * Time.deltaTime;
    constrainedObject.position += velocity * Time.deltaTime * constrainedObject.up;
  }

  void ClampMovement() {
    constrainedObject.localPosition = new Vector3(constrainedObject.localPosition.x, Mathf.Clamp(constrainedObject.localPosition.y, bounds.x, bounds.y), constrainedObject.localPosition.z);
  }

  void BodyRotationDamp() {

  }
}
