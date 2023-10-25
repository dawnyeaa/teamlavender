using UnityEngine;

namespace CharacterController
{
    public class SkateboardMoveAnimator
    {
        private const float DeltaTime = 1.0f / 50.0f;

        private SkateboardMoveState moveState;

        public SkateboardStateMachine sm => moveState.sm;
        public Transform transform => moveState.transform;
        public SkateboardMoveSettings settings => moveState.settings;
        public Rigidbody body => moveState.body;
        public float steer => moveState.steer;

        public Vector3 currentPosition;
        public Quaternion currentRotationOffset;
        public float currentCrouchPercent;

        public Vector3 position;
        public Vector3 velocity;
        public Quaternion rotation;
        public float crouch;
        public float timer;

        public SkateboardMoveAnimator(SkateboardMoveState moveState)
        {
            this.moveState = moveState;
            position = sm.HipHelper.position;
        }

        public void Tick()
        {
            Setup();

            TurnLean();
            HorizontalLean();
            // BoardAndFeetTilt();

            Finalise();
        }

        private void TurnLean()
        {
            position += transform.right * steer * settings.visualLean.x;
            rotation *= Quaternion.Euler(Vector3.forward * steer * settings.visualLean.y);
        }

        private void HorizontalLean()
        {
            var vDot = Vector3.Dot(transform.up, Vector3.up);
            var hDot = Vector3.Dot(transform.right, Vector3.up);
            crouch = Mathf.Lerp(1.0f, crouch, vDot);
            position += Vector3.Lerp(settings.globalHorizontalOffset, Vector3.zero, vDot);

            var localOffset = hDot > 0.0f ? settings.localHorizontalOffsetLeft : settings.localHorizontalOffsetRight;
            position += Vector3.Lerp(transform.rotation * localOffset, Vector3.zero, vDot);

            var rotationOffset = Quaternion.Euler(hDot > 0.0f ? settings.rotationHorizontalOffsetLeft : settings.rotationHorizontalOffsetRight);
            rotation = Quaternion.Slerp(rotationOffset, rotation, vDot);
        }

        private void BoardAndFeetTilt()
        {
            sm.CharacterAnimator.SetFloat("leanValue", 0.5f * steer / sm.MaxAnimatedTruckTurnDeg + 0.5f);
        }

        private void Setup()
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            crouch = sm.Input.crouching ? 1.0f : 0.0f;
        }

        private void Finalise()
        {
            Smooth();
            Itterate();

            var rBasis = Quaternion.Slerp(Quaternion.Euler(settings.hipRBasis), Quaternion.Euler(settings.crouchRBasis), currentCrouchPercent);

            sm.HipHelper.position = currentPosition;
            sm.HipHelper.rotation = Quaternion.LookRotation(transform.forward, Vector3.up) * rBasis * currentRotationOffset;
        }

        private void Itterate()
        {
            var pBasis = sm.HipHelper.parent.TransformPoint(Vector3.Lerp(settings.hipPBasis, settings.crouchPBasis, currentCrouchPercent));

            var force = (pBasis + position - currentPosition) * settings.hipsSpring + (sm.MainRB.velocity - velocity) * settings.hipsDamper;
            currentPosition += velocity * Time.deltaTime;
            velocity += force * Time.deltaTime;
        }

        private void Smooth()
        {
            while (timer > DeltaTime)
            {
                currentRotationOffset = Quaternion.Slerp(rotation, currentRotationOffset, settings.animationSmoothing);
                currentCrouchPercent = Mathf.Lerp(crouch, currentCrouchPercent, settings.animationSmoothing);

                timer -= DeltaTime;
            }

            timer += Time.deltaTime;
        }
    }
}