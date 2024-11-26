/// <summary>
/// This script belongs to cowsins� as a part of the cowsins� FPS Engine. All rights reserved. 
/// </summary>
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace cowsins
{
    public class WeaponEffects : MonoBehaviour
    {
        #region shared
        [System.Serializable]
        public enum BobMethod
        {
            Original, Detailed
        }

        public BobMethod bobMethod;

        [SerializeField] private Transform gunsEffectsTransform, jumpMotionTransform;

        private PlayerMovement PlayerMovement;

        public delegate void Bob();

        public Bob bob;

        #endregion
        #region original
        [SerializeField] private float speed = 1f, distance = 1f;

        private float _distance = 1f;

        private float timer, movement;

        private Vector3 midPoint;

        private Quaternion startRot;

        private Rigidbody rb;

        private float lerpSpeed;
        #endregion
        #region detailed
        [SerializeField] private Vector3 rotationMultiplier;

        [SerializeField] private float translationSpeed;

        [SerializeField] private float rotationSpeed;

        [SerializeField] private Vector3 movementLimit;

        [SerializeField] private Vector3 bobLimit;

        [SerializeField] private float aimingMultiplier;

        // New variables for inclination multipliers
        [SerializeField] private float horizontalInclineMultiplier = 1f;
        [SerializeField] private float forwardInclineMultiplier = 1f;

        private float bobSin { get => Mathf.Sin(bobSpeed); }
        private float bobCos { get => Mathf.Cos(bobSpeed); }

        private float bobSpeed;

        private Vector3 bobPos;

        private Vector3 bobRot;
        #endregion


        #region jumpMotion
        [SerializeField] private AnimationCurve jumpMotion, groundedMotion;

        [SerializeField] private float jumpMotionDistance, jumpMotionRotationAmount;

        [SerializeField, Min(1)] private float evaluationSpeed;

        private float motion = 0, motion2;
        #endregion

        private void Start()
        {
            if (bobMethod == BobMethod.Original)
            {
                midPoint = gunsEffectsTransform.localPosition;
                startRot = gunsEffectsTransform.localRotation;
                bob = OriginalBob;
            }
            else bob = DetailedBob;

            PlayerMovement = GetComponent<PlayerMovement>();
            rb = GetComponent<Rigidbody>();

            PlayerMovement.events.OnJump.AddListener(JumpMotion);
        }

        private void Update()
        {
            if (!PlayerMovement.grounded && !PlayerMovement.wallRunning)
            {
                motion2 = 0;
                motion += Time.deltaTime * evaluationSpeed;
                jumpMotionTransform.localPosition = Vector3.Lerp(jumpMotionTransform.localPosition, new Vector3(0, jumpMotion.Evaluate(motion), 0) * jumpMotionDistance, motion);
                jumpMotionTransform.localRotation = Quaternion.Lerp(jumpMotionTransform.localRotation, Quaternion.Euler(new Vector3(jumpMotion.Evaluate(motion) * jumpMotionRotationAmount, 0, 0)), motion);
            }
            else
            {
                motion = 0;
                motion2 += Time.deltaTime * evaluationSpeed;
                jumpMotionTransform.localPosition = Vector3.Lerp(jumpMotionTransform.localPosition, new Vector3(0, jumpMotion.Evaluate(motion2), 0) * jumpMotionDistance, motion2);
                jumpMotionTransform.localRotation = Quaternion.Lerp(jumpMotionTransform.localRotation, Quaternion.Euler(Vector3.zero), motion2);
            }

            if (!PlayerMovement.grounded && !PlayerMovement.wallRunning) return;
            bob?.Invoke();
        }

        private void JumpMotion()
        {
            motion2 = 0;
            motion = 0;
        }
        private void OriginalBob()
        {
            _distance = distance * rb.linearVelocity.magnitude / 1.5f * Time.deltaTime * aimingMultiplier;
            speed = rb.linearVelocity.magnitude / 1.5f * Time.deltaTime;
            Vector3 localPosition = gunsEffectsTransform.localPosition;
            Quaternion localRotation = gunsEffectsTransform.localRotation;

            if (Mathf.Abs(InputManager.x) == 0 && Mathf.Abs(InputManager.y) == 0)
            {
                timer = Mathf.Lerp(timer, 0, Time.deltaTime);
            }
            else
            {
                movement = Mathf.Sin(timer);
                timer += speed;
                if (timer > Mathf.PI * 2)
                {
                    timer = timer - (Mathf.PI * 2);
                }
            }
            if (movement != 0)
            {
                float translateChange = movement * _distance / 100;
                float totalAxes = Mathf.Abs(InputManager.x) + Mathf.Abs(InputManager.y);
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;
                localPosition.y = midPoint.y + translateChange * 3;
                localPosition.z = midPoint.z + translateChange * 2;
                localPosition.x = startRot.x + translateChange * 2f;

            }
            else
            {
                localPosition.y = midPoint.y;
                localPosition.x = midPoint.x;
            }

            gunsEffectsTransform.localPosition = Vector3.Lerp(gunsEffectsTransform.localPosition, localPosition, Time.deltaTime * 10);
            gunsEffectsTransform.localRotation = Quaternion.Lerp(gunsEffectsTransform.localRotation, localRotation, Time.deltaTime * 10);
        }

        private void DetailedBob()
        {
            bobSpeed += Time.deltaTime * (PlayerMovement.grounded ? rb.linearVelocity.magnitude / 2 : 1) + .01f;
            float mult = PlayerMovement.GetComponent<WeaponController>().isAiming ? aimingMultiplier : 1;

            bobPos.x = (bobCos * bobLimit.x * (PlayerMovement.grounded || PlayerMovement.wallRunning ? 1 : 0)) - (InputManager.x * movementLimit.x);
            bobPos.y = (bobSin * bobLimit.y) - (rb.linearVelocity.y * movementLimit.y);
            bobPos.z = -InputManager.y * movementLimit.z;

            gunsEffectsTransform.localPosition = Vector3.Lerp(gunsEffectsTransform.localPosition, bobPos * mult, Time.deltaTime * translationSpeed);

            bobRot.x = InputManager.x != 0 ? rotationMultiplier.x * Mathf.Sin(2 * bobSpeed) : rotationMultiplier.x * Mathf.Sin(2 * bobSpeed) / 2;
            bobRot.y = InputManager.x != 0 ? rotationMultiplier.y * bobCos : 0;
            bobRot.z = InputManager.x != 0 ? rotationMultiplier.z * bobCos * InputManager.x : 0;

            // New rotation adjustments for horizontal and forward/backward inclines
            if (InputManager.x != 0)
            {
                bobRot.z += horizontalInclineMultiplier * InputManager.x;  // Horizontal incline
            }
            if (InputManager.y != 0)
            {
                bobRot.x += forwardInclineMultiplier * InputManager.y;  // Forward/backward incline
            }

            gunsEffectsTransform.localRotation = Quaternion.Slerp(gunsEffectsTransform.localRotation, Quaternion.Euler(bobRot * mult), Time.deltaTime * rotationSpeed);
        }

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(WeaponEffects))]
    public class WeaponEffectsEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            var myScript = target as WeaponEffects;

            EditorGUILayout.LabelField("WEAPON BOB SETTINGS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bobMethod"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gunsEffectsTransform"));

            if (myScript.bobMethod == WeaponEffects.BobMethod.Original)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("distance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("aimingMultiplier"));
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationMultiplier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("translationSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("movementLimit"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bobLimit"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("aimingMultiplier"));

                // Add fields for the new inclination multipliers
                EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontalInclineMultiplier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("forwardInclineMultiplier"));

                EditorGUI.indentLevel--;

            }
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("JUMP MOTION SETTINGS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpMotionTransform"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpMotion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("groundedMotion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpMotionDistance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpMotionRotationAmount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("evaluationSpeed"));
            EditorGUILayout.Space(5f);

            EditorGUILayout.LabelField("WEAPON SWAY SETTINGS", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Weapon Sway can be modified in each Weapon Prefab ( Root of the Weapon Prefab )", EditorStyles.helpBox);
            EditorGUILayout.Space(5f);
            if (GUILayout.Button("Go to Weapon Prefabs Folder"))
            {
                string folderPath = "Assets/Cowsins/ScriptableObjects/Weapons";
                EditorUtility.FocusProjectWindow();
                Object folder = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
                if (folder != null)
                {
                    EditorGUIUtility.PingObject(folder);
                    Selection.activeObject = folder;
                    EditorUtility.FocusProjectWindow();
                }
                else
                {
                    Debug.LogError($"Folder is empty or not found: {folderPath}. Did you remove or rename the Weapons Folder?");
                }
            }
            EditorGUILayout.Space(5f);
            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}
