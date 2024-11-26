using UnityEngine;

namespace cowsins
{
    public class CameraEffects : MonoBehaviour
    {

        [SerializeField] private Transform playerCamera;

        [SerializeField] private float tiltSpeed, tiltAmount;
        [SerializeField, Tooltip("Maximum Head Bob")] private float headBobAmplitude = 0.2f;
        [SerializeField, Tooltip("Speed to reach the Maximum Head Bob ( headBobAmplitude)")] private float headBobFrequency = 2f;
        [SerializeField, Tooltip("Maximum Breathing Amount")] private float breathingAmplitude = 0.2f;
        [SerializeField, Tooltip("Breathing Speed")] private float breathingFrequency = 2f;
        [SerializeField, Tooltip("Enables Rotation for the Breathing Effect")] private bool applyBreathingRotation;

        private PlayerMovement player;
        private Rigidbody playerRigidbody;

        private Vector3 origPos;
        private Quaternion origRot;

        private void Awake()
        {
            origPos = playerCamera.localPosition;
            origRot = playerCamera.localRotation;

            player = GetComponent<PlayerMovement>();
            playerRigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!PlayerStats.Controllable) return;

            UpdateTilt();

            UpdateHeadBob();
            UpdateBreathing();
        }

        private void UpdateTilt()
        {
            if (player.currentSpeed == 0) return;

            Quaternion rot = CalculateTilt();
            playerCamera.localRotation = Quaternion.Lerp(playerCamera.localRotation, rot, Time.deltaTime * tiltSpeed);
        }

        private Quaternion CalculateTilt()
        {
            float x = InputManager.x;
            float y = InputManager.y;

            Vector3 vector = new Vector3(y, 0, -x).normalized * tiltAmount;

            return Quaternion.Euler(vector);
        }

        private void UpdateHeadBob()
        {
            if (playerRigidbody.linearVelocity.magnitude < player.walkSpeed || InputManager.jumping)
            {
                playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, origPos, Time.deltaTime * 2f);
                playerCamera.localRotation = Quaternion.Lerp(playerCamera.localRotation, origRot, Time.deltaTime * 2f);
                return;
            }

            float angle = Time.timeSinceLevelLoad * headBobFrequency;
            float distanceY = headBobAmplitude * Mathf.Sin(angle) / 400f;
            float distanceX = headBobAmplitude * Mathf.Cos(angle) / 100f;

            playerCamera.position = new Vector3(playerCamera.position.x, playerCamera.position.y + Vector3.up.y * distanceY, playerCamera.position.z);
            playerCamera.Rotate(distanceX, 0, 0, Space.Self);
        }

        private void UpdateBreathing()
        {
            float angle = Time.timeSinceLevelLoad * breathingFrequency;
            float distance = breathingAmplitude * Mathf.Sin(angle) / 400f;
            float distanceRot = breathingAmplitude * Mathf.Cos(angle) / 100f;

            playerCamera.position = new Vector3(playerCamera.position.x, playerCamera.position.y + Vector3.up.y * distance, playerCamera.position.z);

            if (applyBreathingRotation)
            {
                playerCamera.Rotate(distanceRot, 0, 0, Space.Self);
            }
        }
    }
}
