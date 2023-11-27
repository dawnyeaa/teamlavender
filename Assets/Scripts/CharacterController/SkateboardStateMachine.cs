using UnityEngine;
using UnityEngine.Animations;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using CharacterController;
using Cinemachine;
using UnityEngine.Events;

[RequireComponent(typeof(InputController))]
// [RequireComponent(typeof(WheelController))]
public class SkateboardStateMachine : StateMachine {
  // User Constants - Runtime only
  // [Header("Constants - Only read at runtime")]

  public SkateboardMoveSettings moveSettings;
  
  // User Constants - Live update 
  [Header("Constants - Live update")]
  public float MaxSpeed = 20f;
  public float TurnLockSpeed = 30f;
  public AnimationCurve TurnEaseBySpeed;
  public float MaxTurnDeg = 8.34f;
  public float MaxAnimatedTruckTurnDeg = 15f;
  [Range(0, 1)] public float TurnSpeedConservation = 0.5f;
  public float LeanDamping = 0.64f;
  public float TruckSpacing = 0.205f;
  public float PushForce = 10f;
  public float PushStartMultiplier = 3f;
  public float PushStartEpsilon = 0.1f;
  public AnimationCurve PushForceCurve;
  // public float MaxPushDuration = 1f;
  public float WheelFriction = 0.01f;
  public float BrakingFriction = 0.4f;
  public float GrindingFriction = 0.1f;
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
  public float AirTurnStrength = 1f;
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
  public float LipAngleTolerance = 0.75f;
  public float MaxMotionBlur = 35f;
  public float AverageSecondsPerBreath = 8f;
  public float SmallLandVFXThreshold = 0.1f;
  public int MidTrickPointVFXThreshold = 10;

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
  [ReadOnly] public float TurnPercent;
  [ReadOnly] public float LeanPercent;
  [ReadOnly] public float SpringMultiplier;
  [ReadOnly] public Vector3 RawDown = Vector3.down;
  [ReadOnly] public Vector3 Down = Vector3.down;
  [ReadOnly] public float CurrentProjectLength;
  [ReadOnly] public float AirTimeCounter = 0;
  [ReadOnly] public Rail GrindingRail;
  [ReadOnly] public Vector3 GrindBoardLockPoint;
  [ReadOnly] public Vector3 LastGrindPos;
  [ReadOnly] public PIDController3 GrindOffsetPID;
  [ReadOnly] public int[] CurrentAnimTrickIndexes;
  [ReadOnly] public float CurrentHopTrickVerticalMult;
  [ReadOnly] public float CurrentHopTrickHorizontalMult;
  [ReadOnly] public Dictionary<string, Action<int, float, float>> ComboActions = new() {
    { "ollie", null },
    { "kickflip", null },
    { "heelflip", null },
    { "popShuvit", null },
    { "nollie", null },
    { "nollieKickflip", null },
    { "nollieHeelflip", null },
    { "treFlip", null }
  };
  [ReadOnly] public int RollingHardClipIndex = -1;
  [ReadOnly] public DebugFrame debugFrame;
  [ReadOnly] public float TimeToLand = 0;
  [ReadOnly] public float CurrentJumpAirtime = 0;
  [ReadOnly] public bool IsGoofy = false;
  [ReadOnly] public bool IsNollie = false;
  [ReadOnly] public bool CanDie = true;
  [ReadOnly] public int LandVFXTier = 0;
  [ReadOnly] public Combo CurrentlyPlayingTrick;

  // Objects to link
  [Header("Link Slot Objects")]
  public Rigidbody MainRB;
  public Transform frontAxis, backAxis;
  public Transform Facing;
  public Transform MainCamera { get; private set; }
  public InputController Input { get; private set; }
  public ComboController ComboController { get; private set; }
  public Transform footRepresentation;
  public Transform SmoothHipHelper;
  public Transform HipHelper;
  public Transform BodyMesh;
  public Transform Board;
  public Animator CharacterAnimator;
  public Animator BoardIKTiltAnimator;
  public Animator CharacterLeanAnimator;
  public WheelController WheelSpinAnimationController;
  public Transform RegularModel, RagdollModel;
  public Rigidbody[] RagdollTransformsToPush;
  public PositionConstraint FollowTargetConstraint;
  public PositionConstraint LookAtTargetConstraint;
  public CinemachineFreeLook cinemachineLook;
  public TransformHeirarchyMatch RagdollMatcher;
  public SpawnPointManager SpawnPointManager;
  public PointManager PointManager;
  public PauseMenuManager PauseMenuManager;
  public RailManager RailManager;
  public List<Transform> RailLockTransforms;
  public WheelSpinParticleHandler[] WheelSpinParticles;
  public GameObject GrindParticles;
  public EmitterBundle LandEmit;
  // public MeshRenderer SpeedyLines;
  // public Material SpeedyLinesMat;
  public AudioClip LandingHardClip;
  public AudioClip DeathClip;
  public AudioClip PushClip;
  public AudioClip RollingHardClip;
  public AudioClip FartClip;
  public DebugFrameHandler DebugFrameHandler;
  public CharacterPointHandler PointHandler;
  public RendererFeatureDynamicProperties RFprops;
  public SkateSoundController SFX;
  public DynamicCameraController DynamicCam;
  public TutorialDisplay TuteDisplay;
  public UnityEvent OnLanding;
  public UnityEvent OnSmallLanding;
  public UnityEvent OnMedLanding;
  public UnityEvent OnBigLanding;
  public UnityEvent OnLaunching;
  public UnityEvent OnNosePop;
  public UnityEvent OnTailPop;
  public UnityEvent OnPickupPickup;
  public UnityEvent OnTrick;

  [Space]
  public SkateboardCollisionProcessor collisionProcessor;

  [HideInInspector] public Transform ball1, ball2, ball3;

  private void Start() {
    fixedUpdate = true;
    MainCamera = Camera.main.transform;

    Input = GetComponent<InputController>();
    ComboController = GetComponent<ComboController>();
    // SpeedyLinesMat = SpeedyLines.material;

    CurrentAnimTrickIndexes = new int[Enum.GetValues(typeof(TrickAnimationGroup)).Length];

    SwitchState(new SkateboardMoveState(this));

    Input.OnSlamPerformed += Die;
  }

  public void OnOllieForce() {
    if (IsGoofy ^ IsNollie) {
      OnNosePop?.Invoke();
    }
    else {
      OnTailPop?.Invoke();
    }
    SFX.PopSound();
    CurrentlyPlayingTrick = ComboController.currentlyPlayingCombo;
    MainRB.AddForce((Vector3.up - Down).normalized*OllieForce * CurrentHopTrickVerticalMult + Vector3.Project(MainRB.velocity, Facing.transform.forward) * CurrentHopTrickHorizontalMult, ForceMode.Acceleration);
  }

  public void OnPickup() {
    OnPickupPickup?.Invoke();
  }

  public void OnTrickPeak() {
    OnTrick?.Invoke();
  }

  public void StartPushForce(float duration) {
    // SoundEffectsManager.instance.PlaySoundFXClip(PushClip, transform, 1);
    SFX.PushSound();
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

  public void CompleteTrick() {
    CurrentlyPlayingTrick = null;
  }

  public void TryLandVFXTier(int tier) {
    LandVFXTier = Mathf.Max(tier, LandVFXTier);
  }

  public void Die() => Die(null);
  public void Die(Vector3? velocityOverride) {
    if (CanDie) {
      EnterDead(velocityOverride);
      SlamRumble();
    }
  }

  public async void EnterDead(Vector3? velocityOverride) {
    SwitchState(new SkateboardDeadState(this, velocityOverride));
    await Task.Delay((int)(DeadTime*1000));
    SoundEffectsManager.instance.PlaySoundFXClip(FartClip, transform, 1);
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

  public void HideTuteTrickHint() {
    TuteDisplay.HideTrickHelp();
  }

  public void OnCombo(string name, int trickAnimGroup, float verticalForceMult, float horizontalForceMult) {
    ComboActions[name]?.Invoke(trickAnimGroup, verticalForceMult, horizontalForceMult);
  }
}