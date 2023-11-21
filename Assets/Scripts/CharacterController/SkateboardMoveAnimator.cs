using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharacterController
{
    public class SkateboardMoveAnimator
    {
        private const float DeltaTime = 1.0f / 50.0f;

        private SkateboardMoveState moveState;

        private Lean lean;
        private Lean smoothedLean;

        public SkateboardStateMachine sm => moveState.sm;
        public Transform transform => moveState.transform;
        public SkateboardMoveSettings settings => moveState.settings;
        public Rigidbody body => moveState.body;
        public float steerInput => moveState.steer / settings.maxSteer;
        public Transform hipHelper => sm.HipHelper;

        private Vector3 hipHelperOffset;

        public SkateboardMoveAnimator(SkateboardMoveState moveState)
        {
            this.moveState = moveState;
            hipHelperOffset = hipHelper.transform.localPosition;
        }

        public void Tick()
        {
            lean = Vector3.zero;

            CalcUprightLean();
            CalcTurnLean();

            SmoothLean(ref smoothedLean.translation.x, lean.translation.x);
            SmoothLean(ref smoothedLean.translation.y, lean.translation.y);
            SmoothLean(ref smoothedLean.rotation, lean.rotation);

            ApplyLean(smoothedLean);
        }

        private void SmoothLean(ref float current, float next)
        {
            current = Mathf.Lerp(current, next, Time.deltaTime / Mathf.Max(Time.deltaTime, settings.leanSmoothing));
        }

        private void CalcUprightLean()
        {
            var cross = Vector3.Cross(transform.up, Vector3.up);
            var dot = Vector3.Dot(cross, transform.forward);
            lean += settings.uprightLean * dot;
        }

        private void CalcTurnLean()
        {
            lean -= settings.turnLean * steerInput;
        }

        private void ApplyLean(Lean lean)
        {
            hipHelper.localRotation = Quaternion.Euler(Vector3.forward * lean.rotation);
            hipHelper.localPosition = hipHelperOffset + lean.ComputeLean();
        }

        [System.Serializable]
        public struct Lean
        {
            public Vector2 translation;
            [Range(-90.0f, 90.0f)]
            public float rotation;

            public Lean(Vector2 translation, float rotation)
            {
                this.translation = translation;
                this.rotation = rotation;
            }

            public Vector3 ComputeLean()
            {
                return new Vector3(-translation.x, -Mathf.Abs(translation.y), 0.0f);
            }
            
            public static implicit operator Vector3 (Lean lean) => new (lean.translation.x, lean.translation.y, lean.rotation);
            public static implicit operator Lean (Vector3 v) => new (v, v.z);

            public static Lean operator + (Lean lean, Vector3 v) => (Vector3)lean + v;
            public static Lean operator - (Lean lean, Vector3 v) => (Vector3)lean - v;
            public static Lean operator * (Lean lean, float x) => (Vector3)lean * x;
        }
        
        #if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(Lean))]
        public class LeanPropertyDrawer : PropertyDrawer
        {
            private const int Padding = 2;
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => (EditorGUIUtility.singleLineHeight + Padding) * 2.0f;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var a = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                var b = a;
                b.y += a.height + Padding;

                var labelText = label.text;
                EditorGUI.PropertyField(a, property.FindPropertyRelative(nameof(Lean.translation)), new GUIContent(labelText + " Translation"));
                EditorGUI.PropertyField(b, property.FindPropertyRelative(nameof(Lean.rotation)), new GUIContent(labelText + " Rotation"));
            }
        }
        #endif
    }
}