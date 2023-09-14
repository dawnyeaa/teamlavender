using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIExtraInput : MonoBehaviour, Controls.IUIActions {

  public Action OnUnpausePerformed;

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
}
