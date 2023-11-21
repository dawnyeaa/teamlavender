using System;
using CharacterController;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(menuName = "Config/Skateboard Move Settings")]
public class SkateboardMoveSettings : ScriptableObject
{
    [Header("STEERING")]
    public float maxSteer = 3.0f;
    [Range(0.0f, 1.0f)] public float steerInputSmoothing = 0.9f;

    [Space]
    [Header("MOVEMENT GENERAL")]
    public float maxSpeed = 20.0f;
    public float acceleration = 1.0f;
    public float pushDuration = 1.0f;
    public AnimationCurve pushCurve = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);
    public Vector3 localCenterOfMass = new (0.0f, -0.8f, 0.0f);
    public Vector3 inertiaTensor = new (0.3f, 0.4f, 0.04f);
    public float upGravity = 1.5f;
    public float downGravity = 2.0f;
    
    [Header("MOVEMENT CORRECTION")]
    [FormerlySerializedAs("leanSpring")] public float rotationalForce = 150.0f;
    public float rotationalDamping = 10.0f;
    public float rotationalLean = 10.0f;
    public float spinAcceleration = 10.0f;
    public float spinMaxRps = 1.0f;
    public float spinMaxRpsCrouching = 3.0f;
    public float spinDelay = 0.3f;
    [Range(0.0f, 50.0f)] public float rollingResistance;
    [Range(0.0f, 50.0f)] public float airResistance;
    [Range(0.0f, 50.0f)] public float tangentialFriction = 20.0f;

    [Space]
    [Header("GROUND CHECK")]
    public float distanceToGround = 0.13f;
    public Vector3 truckOffset;
    public float truckDepenetrationSpring = 100.0f;
    public float truckDepenetrationDamper = 10.0f;
    public float wheelRadius = 0.12f;

    [Space]
    [Header("BRAKING")]
    [Range(0.0f, 1.0f)] public float dynamicBrake = 0.1f;
    [Range(0.0f, 1.0f)] public float staticBrake = 1.0f;
    public float brakeThreshold = 0.01f;
    [Range(0.0f, 1.0f)] public float lastEvaluatedBrakeThreshold;

    [Space]
    [Header("WALL SETTINGS")]
    public float wallSlideDistance;
    public float wallSlideTorque;
    
    [Space]
    [Header("LANDING PREDICTION")]
    public float predictionMaxTime = 3.0f;
    public float predictionTimestep = 0.02f;
    public float predictionWeight = 0.2f;

    [Header("ANIMATION")]
    [Header("Leaning")]
    public float cameraTargetSpring = 100.0f;
    public float cameraTargetDamp = 10.0f;
    public SkateboardMoveAnimator.Lean uprightLean = new(Vector2.zero, 0.6f);
    public SkateboardMoveAnimator.Lean turnLean = new(Vector2.zero, 0.4f);
    public float leanSmoothing = 1.0f;
}