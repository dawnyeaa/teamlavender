using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour, Controls.IPlayerActions {
  public float turn;
  public bool braking;
  public Vector2 mouseDelta;
  public Vector2 rightStick;

  public Action OnPushPerformed;
  public Action OnSwitchPerformed;
  public Action OnResetPerformed;

  private Controls controls;

  private void OnEnable() {
    if (controls != null)
      return;

    controls = new Controls();
    controls.player.SetCallbacks(this);
    controls.player.Enable();
    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }
  
  private void OnDisable() {
    controls.player.Disable();
  }

  public void OnMove(InputAction.CallbackContext context) {
    turn = context.ReadValue<float>();
  }

  public void OnPush(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    OnPushPerformed?.Invoke();
  }

  public void OnBrake(InputAction.CallbackContext context) {
    if (context.performed) {
      braking = true;
    }
    else if (context.canceled) {
      braking = false;
    }
  }

  public void OnLook(InputAction.CallbackContext context) {
    mouseDelta = context.ReadValue<Vector2>();
  }

  public void OnSwitch(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    OnSwitchPerformed?.Invoke();
  }

  public void OnRightStick(InputAction.CallbackContext context) {
    rightStick = context.ReadValue<Vector2>();
  }

  public void OnDebugreset(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }
  
}
