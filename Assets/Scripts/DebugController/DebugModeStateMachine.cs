using UnityEngine;
using UnityEngine.Animations;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(DebugInputController))]
// [RequireComponent(typeof(WheelController))]
public class DebugModeStateMachine : StateMachine {
  // User Constants - Runtime only
  // [Header("Constants - Only read at runtime")]

  // User Constants - Live update
  [Header("Constants - Live update")]
  public float Acceleration = 1;
  public float Deceleration = 1;
  public float Speed = 1;
  public float VertSpeed = 1;

  // Internal State Processing
  [Header("Internal State")]
  [ReadOnly] public Vector2 Velocity;

  // Objects to link
  // [Header("Link Slot Objects")]
  public CinemachineVirtualCamera VirtCam;
  public Transform MainCamera { get; private set; }
  public DebugInputController Input { get; private set; }

  private void Start() {
    fixedUpdate = false;
    MainCamera = Camera.main.transform;

    Input = GetComponent<DebugInputController>();
    // Wheels = GetComponent<WheelController>();

    SwitchState(new DebugModeInactiveState(this));
  }

  public void Activate() {
    SwitchState(new DebugModeFlyState(this));
  }

  public void Deactivate() {
    SwitchState(new DebugModeInactiveState(this));
  }
}