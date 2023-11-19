using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIExtraInput : MonoBehaviour, Controls.IUIActions {

  public Action OnUnpausePerformed;
  public Action OnMenuRPerformed, OnMenuLPerformed, OnMenuUPerformed, OnMenuDPerformed;
  public Action OnMenuLBPerformed, OnMenuRBPerformed;
  public float RSX = 0;
  public float HueSliderX = 0;

  private Controls controls;

  public void OnEnable() {
    if (controls != null)
      return;

    controls = new Controls();
    controls.UI.SetCallbacks(this);
    controls.UI.Enable();
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }
  
  public void OnDisable() {
    controls.UI.Disable();
    controls = null;
  }

  public void OnUnpause(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    OnUnpausePerformed?.Invoke();
  }

  public void OnMenuL(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    OnMenuLPerformed?.Invoke();
  }

  public void OnMenuR(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    OnMenuRPerformed?.Invoke();
  }

  public void OnMenuU(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    OnMenuUPerformed?.Invoke();
  }

  public void OnMenuD(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    OnMenuDPerformed?.Invoke();
  }

  public void OnMenuLB(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    OnMenuLBPerformed?.Invoke();
  }

  public void OnMenuRB(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    OnMenuRBPerformed?.Invoke();
  }

  public void OnHueSliderX(InputAction.CallbackContext context) {
    HueSliderX = context.ReadValue<float>();
  }

  public void OnRightStickX(InputAction.CallbackContext context) {
    RSX = context.ReadValue<float>();
  }
}
