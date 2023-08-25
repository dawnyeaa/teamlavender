using UnityEngine;
using UnityEngine.Animations;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DebugInputController))]
// [RequireComponent(typeof(WheelController))]
public class DebugModeStateMachine : StateMachine {
  // User Constants - Runtime only
  // [Header("Constants - Only read at runtime")]

  // User Constants - Live update
  // [Header("Constants - Live update")]

  // Internal State Processing
  // [Header("Internal State")]

  // Objects to link
  // [Header("Link Slot Objects")]
  public Transform MainCamera { get; private set; }
  public DebugInputController Input { get; private set; }

  private void Start() {
    // MainCamera = Camera.main.transform;

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