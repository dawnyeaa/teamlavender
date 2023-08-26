using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum Input {
  Push,
  Brake,
  Switch,
  rs1,
  rs2,
  rs3,
  rs4,
  rs5,
  rs6,
  rs7,
  rs8,
  rs9
}

public class InputController : MonoBehaviour, Controls.IPlayerActions {
  public float turn;
  public bool braking;
  public bool crouching;
  public Vector2 mouseDelta;
  public Vector2 rightStick;
  public Vector2 rightStickDigital;

  public float rightStickDead = 0.2f;

  public Action OnPushPerformed;
  public Action OnSwitchPerformed;
  public Action OnResetPerformed;
  public Action OnOlliePerformed;
  public Action OnSlamPerformed;
  public Action EnterDebugMode;

  public SkateboardStateMachine character;
  public DebugModeStateMachine debugMode;

  private Controls controls;

  public void OnEnable() {
    if (controls != null)
      return;

    controls = new Controls();
    controls.player.SetCallbacks(this);
    controls.player.Enable();
    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }
  
  public void OnDisable() {
    controls.player.Disable();
    controls = null;
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
    if (rightStick.magnitude < rightStickDead)
      rightStickDigital.Set(0, 0);
    else {
      Vector2 stickNormed = rightStick.normalized;
      float stickDeg = Vector2.SignedAngle(Vector2.right, stickNormed);
      if (stickDeg <= 22.5f && stickDeg > -22.5f) {
        rightStickDigital.Set(1, 0);
      }
      else if (stickDeg <= 67.5f && stickDeg > 22.5f) {
        rightStickDigital.Set(1, 1);
      }
      else if (stickDeg <= 112.5f && stickDeg > 67.5f) {
        rightStickDigital.Set(0, 1);
      }
      else if (stickDeg <= 157.5f && stickDeg > 112.5f) {
        rightStickDigital.Set(-1, 1);
      }
      else if (stickDeg > -67.5f && stickDeg <= -22.5f) {
        rightStickDigital.Set(1, -1);
      }
      else if (stickDeg > -112.5f && stickDeg <= -67.5f) {
        rightStickDigital.Set(0, -1);
      }
      else if (stickDeg > -157.5f && stickDeg <= -112.5f) {
        rightStickDigital.Set(-1, -1);
      }
      else if (stickDeg <= -157.5f || stickDeg > 157.5f) {
        rightStickDigital.Set(-1, 0);
      }
    }
  }
  public void OnOllie(InputAction.CallbackContext context) {
    if (context.performed) {
      crouching = true;
    }
    else if (context.canceled) {
      if (crouching == true) OnOlliePerformed?.Invoke();
      crouching = false;
    }
  }

  public void OnDebugdie(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    OnSlamPerformed?.Invoke();
  }

  public void OnDebugreset(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  public void OnDebugchangeScene(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    string namey = "Nudge";
    if (SceneManager.GetActiveScene().name == "Nudge") {
      namey = "CharControllerTest4";
    }

    SceneManager.LoadScene(namey);
  }

  public void OnDebugflyMode(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    Time.timeScale = 0;
    character.EnterDebugMode();
    debugMode.Activate();
  }
  
}
