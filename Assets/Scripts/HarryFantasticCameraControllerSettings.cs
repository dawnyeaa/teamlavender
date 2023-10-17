using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Harry's Fantastic Camera Controller Settings")]
public class HarryFantasticCameraControllerSettings : ScriptableObject
{
    public Vector3 positionOffset = new (0.0f, 1.0f, -5.0f);
    public Vector3 lookAtOffset = new (0.0f, 1.5f, 0.0f);

    public Vector3 translationSmoothing = Vector3.one * 0.2f;
    public float rotationSmoothing = 0.05f;
    public float lead = 0.4f;
    public float baseFov = 40.0f;
    public float dollySlope = 0.1f;
    public float dollyFov = 55.0f;
    public Vector3 dollyOffset = new (0.0f, -0.5f, 3.5f);
    public float dollySmoothing = 0.4f;
    [Range(0.0f, 1.0f)] public float maxDolly = 1.0f;
}