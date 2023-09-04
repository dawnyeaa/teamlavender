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

  public HoldTriggerFire stepPrev, stepNext, incWindow, decWindow;

  public SkateboardStateMachine character;
  public DebugModeStateMachine debugMode;

  private Controls controls;

  public void OnEnable() {
    if (controls != null)
      return;

    controls = new Controls();
    controls.debugFly.SetCallbacks(this);
    controls.debugFly.Enable();
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    stepPrev = gameObject.AddComponent<HoldTriggerFire>();
    stepNext = gameObject.AddComponent<HoldTriggerFire>();
    incWindow = gameObject.AddComponent<HoldTriggerFire>();
    decWindow = gameObject.AddComponent<HoldTriggerFire>();
  }
  
  public void OnDisable() {
    controls?.debugFly.Disable();
    controls = null;
    Destroy(stepPrev);
    Destroy(stepNext);
    Destroy(incWindow);
    Destroy(decWindow);
  }

  public void SetDelegate(int index, Action del) {
    switch (index) {
      case 0:
        stepPrev.setup(del);
        break;
      case 1:
        stepNext.setup(del);
        break;
      case 2:
        incWindow.setup(del);
        break;
      case 3:
        decWindow.setup(del);
        break;
      default:
        return;
    }
  }
  
  public void RemoveDelegate(int index, Action del) {
    switch (index) {
      case 0:
        stepPrev.clearDel(del);
        break;
      case 1:
        stepNext.clearDel(del);
        break;
      case 2:
        incWindow.clearDel(del);
        break;
      case 3:
        decWindow.clearDel(del);
        break;
      default:
        return;
    }
  }

  public void OnLook(InputAction.CallbackContext context) {}

  public void OnMove(InputAction.CallbackContext context) {
    move = context.ReadValue<Vector2>();
  }

  public void OnVert(InputAction.CallbackContext context) {
    vert = context.ReadValue<float>();
  }

  public void OnStepBack(InputAction.CallbackContext context) {
    if (context.performed) {
      stepPrev.fireAction();
      stepPrev.startHold();
    }

    if (context.canceled) {
      stepPrev.endHold();
    }
  }

  public void OnStepForwards(InputAction.CallbackContext context) {
    if (context.performed) {
      stepNext.fireAction();
      stepNext.startHold();
    }

    if (context.canceled) {
      stepNext.endHold();
    }
  }

  public void OnIncreaseFrameWindow(InputAction.CallbackContext context) {
    if (context.performed) {
      incWindow.fireAction();
      incWindow.startHold();
    }

    if (context.canceled) {
      incWindow.endHold();
    }
  }

  public void OnDecreaseFrameWindow(InputAction.CallbackContext context) {
    if (context.performed) {
      decWindow.fireAction();
      decWindow.startHold();
    }

    if (context.canceled) {
      decWindow.endHold();
    }
  }

  public void OnDebugflyMode(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    Time.timeScale = 1;
    debugMode.Deactivate();
    character.ExitDebugMode();
  }
}
