using System;
using UnityEngine;

public class HarryFantasticCameraController : MonoBehaviour
{
    [SerializeField] private SkateboardStateMachine player;
    [SerializeField] private HarryFantasticCameraControllerSettings settings;
    [Range(0.0f, 1.0f)] [ReadOnly] public float dolly;

    private Camera target;

    private Vector3 positionTarget;
    private Vector3 lookAtTarget;
    private Quaternion rotationOffset;
    private Quaternion targetRotation;

    private Transform positionSource => player.Facing.transform;
    private Transform lookAtSource => player.LookatConstraint.transform;

    private void LateUpdate()
    {
        if (!settings) return;
        if (!target) target = FindObjectOfType<Camera>();

        GetSourcePosition();

        ApplyDamping();
        ApplyDolly();

        Finalise();
    }

    private void ApplyDolly()
    {
        var forwardSpeed = Vector3.Dot(player.MainRB.velocity, player.Facing.transform.forward);
        var tDolly = 2.0f * Mathf.Atan(forwardSpeed * settings.dollySlope) / Mathf.PI;
        tDolly = Mathf.Min(tDolly, settings.maxDolly);
        
        dolly += (tDolly - dolly) / Mathf.Max(Time.deltaTime, settings.dollySmoothing) * Time.deltaTime;
        
        if (target) target.fieldOfView = Mathf.Lerp(settings.baseFov, settings.dollyFov, dolly);
        positionTarget += targetRotation * settings.dollyOffset * dolly;
    }

    private void ApplyDamping() { }

    private void Finalise()
    {
        transform.position += ComponentWise(f => (f(positionTarget) - f(transform.position)) / Mathf.Max(Time.deltaTime, f(settings.translationSmoothing)) * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(lookAtTarget - Vector3.LerpUnclamped(transform.position, positionTarget, settings.lead), Vector3.up) * rotationOffset;

        if (!target) return;

        target.transform.position = transform.position;
        target.transform.rotation = transform.rotation;
    }

    private void GetSourcePosition()
    {
        targetRotation = Quaternion.Euler(0.0f, Mathf.Atan2(positionSource.forward.x, positionSource.forward.z) * Mathf.Rad2Deg, 0.0f);
        
        if (positionSource) positionTarget = positionSource.position + targetRotation * settings.positionOffset;
        if (lookAtSource) lookAtTarget = lookAtSource.position + targetRotation * settings.lookAtOffset;
        rotationOffset = Quaternion.identity;
    }

    private void OnValidate()
    {
        LateUpdate();
    }

    private static Vector3 ComponentWise(Func<Func<Vector3, float>, float> callback) => new()
    {
        x = callback(v => v.x),
        y = callback(v => v.y),
        z = callback(v => v.z),
    };
}