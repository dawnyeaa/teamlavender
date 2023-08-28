using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DebugInputController : MonoBehaviour, Controls.IDebugFlyActions {
  public Vector2 move;
  public float vert;

  public Action OnStepNextPerformed;
  public Action OnStepPrevPerformed;
  public Action OnIncFrameWindowPerformed;
  public Action OnDecFrameWindowPerformed;

  public SkateboardStateMachine character;
  public DebugModeStateMachine debugMode;

  private Controls controls;

  public void OnEnable() {
    if (controls != null)
      return;

    controls = new Controls();
    controls.debugFly.SetCallbacks(this);
    controls.debugFly.Enable();
    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }
  
  public void OnDisable() {
    controls.debugFly.Disable();
    controls = null;
  }

  public void OnLook(InputAction.CallbackContext context) {}

  public void OnMove(InputAction.CallbackContext context) {
    move = context.ReadValue<Vector2>();
  }

  public void OnVert(InputAction.CallbackContext context) {
    vert = context.ReadValue<float>();
  }

  public void OnStepBack(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    OnStepPrevPerformed?.Invoke();
  }

  public void OnStepForwards(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    OnStepNextPerformed?.Invoke();
  }

  public void OnIncreaseFrameWindow(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    OnIncFrameWindowPerformed?.Invoke();
  }

  public void OnDecreaseFrameWindow(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    OnDecFrameWindowPerformed?.Invoke();
  }

  public void OnDebugflyMode(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    Time.timeScale = 1;
    debugMode.Deactivate();
    character.ExitDebugMode();
  }
}
