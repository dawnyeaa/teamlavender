using UnityEngine;
using UnityEngine.Animations;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Cinemachine;

[RequireComponent(typeof(InputController))]
// [RequireComponent(typeof(WheelController))]
public class SkateboardStateMachine : StateMachine {
  // User Constants - Runtime only
  // [Header("Constants - Only read at runtime")]

  // User Constants - Live update
  [Header("Constants - Live update")]
  public float MaxSpeed = 20f;
  public float TurnLockSpeed = 30f;
  public float PushForce = 10f;
  public AnimationCurve PushForceCurve;
  // public float MaxPushDuration = 1f;
  public float WheelFriction = 0.01f;
  public float BrakingFriction = 0.4f;
  public float GrindingFriction = 0.1f;
  public float MaxTruckTurnDeg = 8.34f;
  public float MaxAnimatedTruckTurnDeg = 15f;
  public float TruckSpacing = 0.205f;
  public float TruckTurnDamping = 0.3f;
  public float SpringConstant = 40f;
  public float SpringMultiplierMin = 0.5f;
  public float SpringMultiplierMax = 1f;
  public float SpringDamping = 1f;
  public float ProjectRadius = 0.25f;
  public float ProjectLength = 1.5f;
  public float ForwardCollisionDistance = 0.43f;
  public float ForwardCollisionOriginYOffset = 0.1f;
  public float WallBounceForce = 0.5f;
  public float RightingStrength = 1f;
  // public float EdgeSafeSpeedEpsilon = 0.1f;
  // public float EdgeSafeAngle = 60f;
  public float GoingDownThreshold = -0.1f;
  public float LandingAngleGive = 0.8f;
  public float AirTurnForce = 1f;
  public AnimationCurve TurningEase;
  [Range(0, 1)] public float TruckGripFactor = 0.8f;
  public float BoardPositionDamping = 1f;
  public float PushingMaxSlope = 5f;
  public float PushTurnReduction = 0.75f;
  public float OllieForce = 1f;
  public float UncrouchDelayTime = 0.2f;
  public float MaxProceduralCrouchDistance = 0.3f;
  public float DeadTime = 3f;
  public float MinHeadZoneSize = 2.4f;
  public float MaxHeadZoneSize = 6f;
  [Range(0, 1)] public float HeadZoneSpeedToHorizontalRatio = 0.5f;
  public float MinimumAirTime = 0.5f;
  public float PointsPerAirTimeSecond = 100f;
  public float GrindingPosSpringConstant = 40f;
  public float GrindingPosSpringDamping = 1f;
  public float GrindOffsetHeight = 1f;
  public float RailStartBoostForce = 10f;
  public float ExitRailForce = 20f;
  public float HipHelperFPS = 12f;
  public float MinWheelSpinParticleSpeed = 1f;
  public float MinWheelSpinParticleChance = 0.1f;
  public float MaxWheelSpinParticleChance = 0.75f;
  public float MinSpeedyLineSpeed = 2f;

  // Internal State Processing
  [Header("Internal State")]
  [ReadOnly] public bool FacingForward = true;
  [ReadOnly] public float Friction;
  [ReadOnly] public bool Grounded = true;
  [ReadOnly] public bool Crouching = false;
  [ReadOnly] public float ProceduralCrouchFactor = 0;
  [ReadOnly] public float UncrouchDelayTimer = 0;
  [ReadOnly] public ContinuousDataStepper HipHeight;
  [ReadOnly] public bool PushingAnim = false;
  [ReadOnly] public bool Pushing = false;
  [ReadOnly] public bool PlayingBufferedPush = false;
  [ReadOnly] public float CurrentPushT = 0;
  [ReadOnly] public float MaxPushT = 0;
  [ReadOnly] public bool PushBuffered = false;
  [ReadOnly] public float TruckTurnPercent;
  [ReadOnly] public float SpringMultiplier;
  [ReadOnly] public Vector3 Down = Vector3.down;
  [ReadOnly] public Vector3 DampedDown = Vector3.down;
  [ReadOnly] public float CurrentProjectLength;
  [ReadOnly] public float AirTimeCounter = 0;
  [ReadOnly] public Rail GrindingRail;
  [ReadOnly] public Vector3 GrindBoardLockPoint;
  [ReadOnly] public Vector3 LastGrindPos;
  [ReadOnly] public PIDController3 GrindOffsetPID;
  [ReadOnly] public int CurrentOllieTrickIndex;
  [ReadOnly] public IDictionary<string, Action> ComboActions = new Dictionary<string, Action>() {
    { "ollie", null },
    { "kickflip", null },
    { "heelflip", null },
    { "popShuvit", null }
  };
  [ReadOnly] public DebugFrame debugFrame;

  // Objects to link
  [Header("Link Slot Objects")]
  public Rigidbody MainRB;
  public Transform frontAxis, backAxis;
  public Transform FacingParent;
  public Torquer Facing;
  public Transform MainCamera { get; private set; }
  public InputController Input { get; private set; }
  public Transform footRepresentation;
  public Transform SmoothHipHelper;
  public Transform HipHelper;
  public Transform BodyMesh;
  public Transform Board;
  public Animator CharacterAnimator;
  public Animator BoardIKTiltAnimator;
  public Transform RegularModel, RagdollModel;
  public Rigidbody[] RagdollTransformsToPush;
  public ParentConstraint LookatConstraint;
  public TransformHeirarchyMatch RagdollMatcher;
  public SpawnPointManager SpawnPointManager;
  public HeadSensWrapper HeadSensZone;
  public PointManager PointManager;
  public PauseMenuManager PauseMenuManager;
  public RailManager RailManager;
  public List<Transform> RailLockTransforms;
  public WheelSpinParticleHandler[] WheelSpinParticles;
  public EmitterBundle LandEmit;
  public MeshRenderer SpeedyLines;
  public Material SpeedyLinesMat;
  public DebugFrameHandler DebugFrameHandler;

  [HideInInspector] public Transform ball1, ball2, ball3;

  private void Start() {
    fixedUpdate = true;
    MainCamera = Camera.main.transform;

    Input = GetComponent<InputController>();
    SpeedyLinesMat = SpeedyLines.material;

    SwitchState(new SkateboardMoveState(this));

    Input.OnSlamPerformed += Die;
  }

  public void OnOllieForce() {
    MainRB.AddForce((Vector3.up - Down).normalized*OllieForce, ForceMode.Acceleration);
  }

  public void OnKickflipForce() {
    MainRB.AddForce((Vector3.up - Down).normalized*OllieForce, ForceMode.Acceleration);
  }

  public void StartPushForce(float duration) {
    Pushing = true;
    MaxPushT = duration;
    CurrentPushT = duration;
  }

  public void BrakeForce() {
    Input.braking = true;
  }

  public void PushingEnd() {
    PushingAnim = false;
    Pushing = false;
  }

  public void Die() {
    EnterDead();
    SlamRumble();
  }

  public async void EnterDead() {
    SwitchState(new SkateboardDeadState(this));
    await Task.Delay((int)(DeadTime*1000));
    SwitchState(new SkateboardMoveState(this));
  }

  public void EnterRail() {
    SwitchState(new SkateboardRailState(this));
  }

  public void ExitRail() {
    SwitchState(new SkateboardMoveState(this));
  }

  public async void SlamRumble() {
    if (Gamepad.current != null) {
      Gamepad.current.SetMotorSpeeds(0.25f, 0.75f);
      await Task.Delay(100);
      Gamepad.current.SetMotorSpeeds(0, 0);
    }
  }

  public void EnterDebugMode() {
    SwitchState(new SkateboardPauseState(this));
  }

  public void ExitDebugMode() {
    SwitchState(new SkateboardMoveState(this));
  }

  public void Pause() {
    SwitchState(new SkateboardPauseState(this));
    PauseMenuManager.PauseGame();
  }

  public void Unpause() {
    SwitchState(new SkateboardMoveState(this));
  }

  public void OnCombo(string name) {
    ComboActions[name]?.Invoke();
  }
}