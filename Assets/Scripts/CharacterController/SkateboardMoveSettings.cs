using System;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(menuName = "Config/Skateboard Move Settings")]
public class SkateboardMoveSettings : ScriptableObject
{
    public float maxSteer = 3.0f;
    [Range(0.0f, 1.0f)] public float steerInputSmoothing = 0.9f;

    [Space]
    public float maxSpeed = 20.0f;
    public float acceleration = 1.0f;
    public float pushDuration = 1.0f;
    public AnimationCurve pushCurve = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);
    public Vector3 localCenterOfMass = new (0.0f, -0.8f, 0.0f);
    public Vector3 inertiaTensor = new (0.3f, 0.4f, 0.04f);
    
    [FormerlySerializedAs("leanSpring")] public float rotationalForce = 150.0f;
    public float rotationalDamping = 10.0f;
    public float rotationalLean = 10.0f;
    public float spinAcceleration = 10.0f;
    public float spinMaxRps = 10.0f;
    public float spinDelay = 0.3f;
    [Range(0.0f, 50.0f)] public float rollingResistance;
    [Range(0.0f, 50.0f)] public float airResistance;
    [Range(0.0f, 50.0f)] public float tangentialFriction = 20.0f;

    [Space]
    public float distanceToGround = 0.13f;
    public Vector3 truckOffset;
    public float truckDepenetrationSpring = 100.0f;
    public float truckDepenetrationDamper = 10.0f;
    public float wheelRadius = 0.12f;

    [Space]
    [Range(0.0f, 1.0f)] public float dynamicBrake = 0.1f;
    [Range(0.0f, 1.0f)] public float staticBrake = 1.0f;
    public float brakeThreshold = 0.01f;
    [Range(0.0f, 1.0f)] public float lastEvaluatedBrakeThreshold;

    [Space]
    public float wallSlideDistance;
    public float wallSlideTorque;
    
    [Space]
    public float predictionMaxTime = 3.0f;
    public float predictionTimestep = 0.02f;
    public float predictionWeight = 0.2f;

    [Header("Animation")]
    [Range(0.0f, 1.0f)]public float animationSmoothing = 0.8f;
    public Vector2 visualLean;
    public Vector3 hipPBasis = new (0.0f, 1.1619f, 0.0f);
    public Vector3 hipRBasis = new (0.0f, 0.0f, 0.0f);
    public Vector3 crouchPBasis = new (0.0f, 1.1619f, 0.0f);
    public Vector3 crouchRBasis = new (0.0f, 0.0f, 0.0f);
    public Vector3 globalHorizontalOffset;
    public Vector3 localHorizontalOffsetLeft;
    public Vector3 localHorizontalOffsetRight;
    public Vector3 rotationHorizontalOffsetLeft;
    public Vector3 rotationHorizontalOffsetRight;
    public bool swapBasis;

    private void OnValidate()
    {
        if (swapBasis)
        {
            swapBasis = false;
            swap(ref hipPBasis, ref crouchPBasis);
            swap(ref hipRBasis, ref crouchRBasis);
        }

        void swap<T>(ref T a0, ref T b0)
        {
            var a1 = b0;
            var b1 = a0;

            a0 = a1;
            b0 = b1;
        }
    }
}