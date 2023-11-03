using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

public enum Input {
  northButton,
  eastButton,
  southButton,
  westButton,
  lb,
  lt,
  rb,
  rt,
  dpadUp,
  dpadRight,
  dpadDown,
  dpadLeft,
  rsClick,
  lsClick,
  rs1,
  rs2,
  rs3,
  rs4,
  rs6,
  rs7,
  rs8,
  rs9,
  ls1,
  ls2,
  ls3,
  ls4,
  ls6,
  ls7,
  ls8,
  ls9
}

public class InputController : MonoBehaviour, Controls.IPlayerActions {
  public static InputController instance;
  private const float ONEONROOT2 = 0.7071067811865475f;
  public float turn;
  public bool braking;
  public bool crouching;
  public bool nolliecrouching;
  public Vector2 mouseDelta;
  public Vector2 rightStick;
  public Vector2 rightStickDigital;

  public float rightStickDead = 0.2f;
  public float comboStickDead = 0.2f;
  public float comboStickMidRadius = 0.1f;
  public float comboStickMidVelocityThreshold = 0.1f;
  public float comboStickMidToleranceDuration = 0.1f;
  public float comboStickMidTimer = 0;

  private Vector2 rightStickLast = new(0, 0);
  private Vector2 leftStickLast = new(0, 0);
  private double rightStickLastTime = 0, leftStickLastTime = 0;

  public int rsNumpad = 0;
  public Vector2 rsRaw = new(0, 0);

  private int rsLastNumpad = -1;
  private int lsLastNumpad = -1;

  public Action OnPushPerformed;
  public Action OnSwitchPerformed;
  public Action OnResetPerformed;
  public Action OnOlliePerformed;
  public Action OnSlamPerformed;
  public Action OnPausePerformed;
  public Action OnStartBraking, OnEndBraking;
  public Action OnShowDebugPointsPerformed;
  public Action EnterDebugMode;

  public SkateboardStateMachine character;
  public DebugModeStateMachine debugMode;

  public ComboController comboController;

  private Controls controls;

  public void OnEnable() {
    if (controls != null)
      return;

    comboController =
    comboController != null ? comboController : character.GetComponent<ComboController>();

    controls = new Controls();
    controls.player.SetCallbacks(this);
    controls.player.Enable();
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }
  
  public void OnDisable() {
    if (controls != null) {
      controls.player.Disable();
      controls = null;
    }
  }

  public void Awake() {
    instance ??= this;
  }

  void FixedUpdate() {
    if (rsNumpad == 5) {
      comboStickMidTimer += Time.fixedDeltaTime;
      if (comboStickMidTimer > comboStickMidToleranceDuration) {
        OnOllieCrouch(false);
        OnNollieCrouch(false);
        comboController.ClearBuffer();
      }
    }
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
      OnStartBraking?.Invoke();
    }
    else if (context.canceled) {
      OnEndBraking?.Invoke();
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
    // rightStickDigital = ParseStickDigitalDir(rightStick, rightStickDead);
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

  public void OnNollieCrouch(InputAction.CallbackContext context) {
    if (context.performed) {
      nolliecrouching = true;
    }
    else if (context.canceled) {
      nolliecrouching = false;
    }
  }

  public void OnNollieCrouch(bool start) {
    if (start) {
      nolliecrouching = true;
    }
    else {
      nolliecrouching = false;
    }
  }

  public void OnOllieCrouch(InputAction.CallbackContext context) {
    if (context.performed) {
      crouching = true;
    }
    else if (context.canceled) {
      crouching = false;
    }
  }

  public void OnOllieCrouch(bool start) {
    if (start) {
      crouching = true;
    }
    else {
      crouching = false;
    }
  }

  public void OnComboInputButton(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    
    var buttonName = context.control.name;
    Input input;

    switch (buttonName) {
      case "buttonNorth":
        input = Input.northButton;
        break;
      case "buttonEast":
        input = Input.eastButton;
        break;
      case "buttonSouth":
        input = Input.southButton;
        break;
      case "buttonWest":
        input = Input.westButton;
        break;
      case "rightShoulder":
        input = Input.rb;
        break;
      case "rightTrigger":
        input = Input.rt;
        break;
      case "leftShoulder":
        input = Input.lb;
        break;
      case "leftTrigger":
        input = Input.lt;
        break;
      case "up":
        input = Input.dpadUp;
        break;
      case "right":
        input = Input.dpadRight;
        break;
      case "down":
        input = Input.dpadDown;
        break;
      case "left":
        input = Input.dpadLeft;
        break;
      case "leftStickPress":
        input = Input.lsClick;
        break;
      case "rightStickPress":
        input = Input.rsClick;
        break;
      default:
        // unknown input
        return;
    }

    comboController.AddToBuffer(input);
  }

  public void OnComboInputStick(InputAction.CallbackContext context) {
    var stick = context.ReadValue<Vector2>();
    // check if left or right stick (assume right if unknown)
    var stickName = context.control.name;
    var isLeftStick = stickName.ToLower().Contains("left");

    Vector2 stickDiff;
    double stickTimeDelta;
    if (isLeftStick) {
      stickDiff = leftStickLast - stick;
      stickTimeDelta = context.time - leftStickLastTime;
      leftStickLast = stick;
      leftStickLastTime = context.time;
    }
    else {
      stickDiff = rightStickLast - stick;
      stickTimeDelta = context.time - rightStickLastTime;
      rightStickLast = stick;
      rightStickLastTime = context.time;
    }
    var stickVelocity = stickDiff.magnitude/stickTimeDelta;
    
    var stickNumpad = ParseStickNumpadNotation(stick, comboStickDead, comboStickMidRadius, comboStickMidVelocityThreshold, (float)stickVelocity);
    var stickIndex = stickNumpad - (stickNumpad > 5 ? 1 : 0);

    var lastNumpad = isLeftStick ? lsLastNumpad : rsLastNumpad;
    var stickNumpadValueOffset = (isLeftStick ? Input.ls1 : Input.rs1) - 1;
    bool isStickValueNew = stickNumpad != lastNumpad;

    if (isStickValueNew && stickNumpad != 0) {
      if (stickNumpad == 5) {
        comboStickMidTimer = 0;
      }
      else {
        OnOllieCrouch(stickNumpad == 2);
        OnNollieCrouch(stickNumpad == 8);
        comboController.AddToBuffer(stickIndex+stickNumpadValueOffset);
      }
    }

    if (isLeftStick) {
      lsLastNumpad = stickNumpad;
    }
    else {
      rsRaw = stick;
      rsNumpad = stickNumpad;
      rsLastNumpad = stickNumpad;
    }
  }

  public void OnPause(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    OnPausePerformed?.Invoke();
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

  public void OnDebugpointsDisplay(InputAction.CallbackContext context) {
    if (!context.performed)
      return;
    OnShowDebugPointsPerformed?.Invoke();
  }

  public void OnDebugflyMode(InputAction.CallbackContext context) {
    if (!context.performed)
      return;

    Time.timeScale = 0;
    character.EnterDebugMode();
    debugMode.Activate();
  }

  public static int ParseStickNumpadNotation(Vector2 stick, 
                                             float stickDeadRadius, 
                                             float stickMidRadius, 
                                             float stickMidVelocityThreshold,
                                             float stickVelocity) {
    Vector2[] STICK_DIRECTIONS = {
      new Vector2(-ONEONROOT2, -ONEONROOT2),
      new Vector2(0, -1),
      new Vector2(ONEONROOT2, -ONEONROOT2),
      new Vector2(-1, 0),
      new Vector2(1, 0),
      new Vector2(-ONEONROOT2, ONEONROOT2),
      new Vector2(0, 1),
      new Vector2(ONEONROOT2, ONEONROOT2)
    };

    if (stick.magnitude >= stickDeadRadius) {
      // its a regular direction
      Vector2 stickNormed = stick.normalized;
      float[] stickDots = {
        Vector2.Dot(stickNormed, STICK_DIRECTIONS[0]),
        Vector2.Dot(stickNormed, STICK_DIRECTIONS[1]),
        Vector2.Dot(stickNormed, STICK_DIRECTIONS[2]),
        Vector2.Dot(stickNormed, STICK_DIRECTIONS[3]),
        Vector2.Dot(stickNormed, STICK_DIRECTIONS[4]),
        Vector2.Dot(stickNormed, STICK_DIRECTIONS[5]),
        Vector2.Dot(stickNormed, STICK_DIRECTIONS[6]),
        Vector2.Dot(stickNormed, STICK_DIRECTIONS[7])
      };

      int maxIndex = 0;
      float max = float.MinValue;
      for (int i = 0; i < stickDots.Length; ++i) {
        if (stickDots[i] > max) {
          max = stickDots[i];
          maxIndex = i;
        }
      }

      // maps 0...7 to 1...9, skipping 5
      return maxIndex + (maxIndex >= 4 ? 2 : 1);
    }
    else if (stick.magnitude < stickMidRadius && stickVelocity < stickMidVelocityThreshold) {
      // its at 5
      return 5;
    }
    // otherwise its just in the dead zone, return code 0
    return 0;
  }

  private static Vector2 ParseStickDigitalDir(Vector2 stick, 
                                             float stickDeadRadius, 
                                             float stickMidRadius, 
                                             float stickMidVelocityThreshold,
                                             float stickVelocity) {
    Vector2 result = new();

    switch (ParseStickNumpadNotation(stick, stickDeadRadius, stickMidRadius, stickMidVelocityThreshold, stickVelocity)) {
      case 1:
        result.Set(-1, -1);
        break;
      case 2:
        result.Set(0, -1);
        break;
      case 3:
        result.Set(1, -1);
        break;
      case 4:
        result.Set(-1, 0);
        break;
      case 5:
        break;
      case 6:
        result.Set(1, 0);
        break;
      case 7:
        result.Set(-1, 1);
        break;
      case 8:
        result.Set(0, 1);
        break;
      case 9:
        result.Set(1, 1);
        break;
    }
    return result;
  }
}
