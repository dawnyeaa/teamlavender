using Unity.Burst;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

[BurstCompile]
public struct HipsConstraintJob : IWeightedAnimationJob
{
    public ReadWriteTransformHandle constrained;
    public ReadOnlyTransformHandle source;
    public ReadOnlyTransformHandle root;

    public FloatProperty jobWeight { get; set; }

    public void ProcessRootMotion(AnimationStream stream) { }

    public void ProcessAnimation(AnimationStream stream)
    {
        var w = jobWeight.Get(stream);
        if (w <= 0.0f) return;

        var position = constrained.GetPosition(stream);
        var rotation = constrained.GetRotation(stream);
        var rootRotation = root.GetRotation(stream);
        
        var oldPosition = position;
        var oldRotation = rotation;

        position += rootRotation * source.GetLocalPosition(stream);
        rotation = rootRotation * source.GetLocalRotation(stream) * (Quaternion.Inverse(rootRotation) * rotation);
        
        constrained.SetPosition(stream, Vector3.Lerp(oldPosition, position, w));
        constrained.SetRotation(stream, Quaternion.Slerp(oldRotation, rotation, w));
    }
}

[System.Serializable]
public struct HipsConstraintData : IAnimationJobData
{
    public Transform constrainedObject;
    [SyncSceneToStream] public Transform sourceObject;
    [SyncSceneToStream] public Transform rootObject;

    public bool IsValid()
    {
        return !(constrainedObject == null || sourceObject == null || rootObject == null);
    }

    public void SetDefaultValues()
    {
        constrainedObject = null;
        sourceObject = null;
        rootObject = null;
    }
}

public class HipsConstraintBinder : AnimationJobBinder<HipsConstraintJob, HipsConstraintData>
{
    public override HipsConstraintJob Create(Animator animator, ref HipsConstraintData data, Component component)
    {
        return new HipsConstraintJob()
        {
            constrained = ReadWriteTransformHandle.Bind(animator, data.constrainedObject),
            source = ReadOnlyTransformHandle.Bind(animator, data.sourceObject),
            root = ReadOnlyTransformHandle.Bind(animator, data.rootObject),
        };
    }

    public override void Destroy(HipsConstraintJob job) { }
}

[DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Hip Constraint")]
public class HipsConstraint : RigConstraint<HipsConstraintJob, HipsConstraintData, HipsConstraintBinder> { }