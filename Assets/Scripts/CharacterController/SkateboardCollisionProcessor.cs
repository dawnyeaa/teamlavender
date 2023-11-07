using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharacterController
{
    [System.Serializable]
    public class SkateboardCollisionProcessor
    {
        [SerializeField] private bool enabled = true;
        [SerializeField] private float speedThresholdKmpH = 30.0f;
        [SerializeField] [ReadOnly] private float peak;
        [SerializeField] [ReadOnly] private float speed;

        private Vector3 lastVelocity;

        public void FixedUpdate(SkateboardStateMachine target)
        {
            if (!enabled) return;
            
            var rb = target.MainRB;
            var velocity = rb.velocity;

            var deltaVelocity = velocity - lastVelocity;
            var deltaSpeedKmpH = Mathf.Abs(Vector3.Dot(rb.transform.forward, deltaVelocity * 3.6f));

            if (deltaSpeedKmpH > speedThresholdKmpH)
            {
                target.Die();
            }

            peak = Mathf.Max(peak, deltaSpeedKmpH);
            speed = velocity.magnitude * 3.6f;
            lastVelocity = velocity;
        }
    }
}