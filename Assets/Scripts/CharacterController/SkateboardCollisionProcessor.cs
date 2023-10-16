using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharacterController
{
    [System.Serializable]
    public class SkateboardCollisionProcessor
    {
        [SerializeField] private float threshold;
        [SerializeField] [ReadOnly] private float peak;

        private Vector3 lastVelocity;

        public void FixedUpdate(SkateboardStateMachine target)
        {
            var rb = target.MainRB;
            var velocity = rb.velocity;

            var diff = (velocity - lastVelocity) / Time.deltaTime;
            var fac = diff.magnitude - threshold;

            if (fac > 0.0f) target.Die(lastVelocity);

            peak = Mathf.Max(peak, fac);
            lastVelocity = velocity;
        }
    }
}