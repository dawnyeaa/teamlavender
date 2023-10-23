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
    public float predictionMaxTime = 3.0f;
    public float predictionTimestep = 0.02f;
    public float predictionWeight = 0.2f;
}