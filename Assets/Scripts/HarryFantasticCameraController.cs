using System;
using UnityEngine;

public class HarryFantasticCameraController : MonoBehaviour
{
    [SerializeField] private SkateboardStateMachine player;
    [SerializeField] private Transform positionSource;
    [SerializeField] private Transform lookAtSource;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 lookAtOffset;

    [SerializeField] private Vector3 smoothing = Vector3.one;
    [SerializeField][Range(0.0f, 1.0f)] private float arg0;
    [SerializeField] private float baseFov = 40.0f;
    [SerializeField] private float dollySlope = 0.1f;
    [SerializeField] private float dollyFov = 70.0f;
    [SerializeField] private Vector3 dollyOffset = Vector3.forward * 7.0f;
    [SerializeField] private float dollySmoothing = 0.2f;
    [SerializeField][Range(0.0f, 1.0f)] private float maxDolly = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)][ReadOnly] private float dolly;

    private Camera target;

    private Vector3 positionTarget;
    private Vector3 lookAtTarget;
    private Quaternion rotationOffset;
    private Quaternion targetRotation;
    
    private void LateUpdate()
    {
        if (!target) target = FindObjectOfType<Camera>();

        GetSourcePosition();

        ApplyDamping();
        ApplyDolly();

        Finalise();
    }

    private void ApplyDolly()
    {
        var forwardSpeed = Vector3.Dot(player.MainRB.velocity, player.Facing.transform.forward);
        var tDolly = 2.0f * Mathf.Atan(forwardSpeed * dollySlope) / Mathf.PI;
        tDolly = Mathf.Min(tDolly, maxDolly);
        
        dolly += (tDolly - dolly) / Mathf.Max(Time.deltaTime, dollySmoothing) * Time.deltaTime;
        
        if (target) target.fieldOfView = Mathf.Lerp(baseFov, dollyFov, dolly);
        positionTarget += targetRotation * dollyOffset * dolly;
    }

    private void ApplyDamping() { }

    private void Finalise()
    {
        transform.position += ComponentWise(f => (f(positionTarget) - f(transform.position)) / Mathf.Max(Time.deltaTime, f(smoothing)) * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(lookAtTarget - Vector3.Lerp(transform.position, positionTarget, arg0), Vector3.up) * rotationOffset;

        if (!target) return;

        target.transform.position = transform.position;
        target.transform.rotation = transform.rotation;
    }

    private void GetSourcePosition()
    {
        targetRotation = Quaternion.Euler(0.0f, Mathf.Atan2(positionSource.forward.x, positionSource.forward.z) * Mathf.Rad2Deg, 0.0f);
        
        if (positionSource) positionTarget = positionSource.position + targetRotation * positionOffset;
        if (lookAtSource) lookAtTarget = lookAtSource.position + targetRotation * lookAtOffset;
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