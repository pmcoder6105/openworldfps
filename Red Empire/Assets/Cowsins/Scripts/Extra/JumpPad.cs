using UnityEngine;

namespace cowsins
{

    public enum ForceModeType
    {
        PlayerOrientation,
        PlayerMovementDirection,
        JumpPadLocalAxis
    }

    public class JumpPad : MonoBehaviour
    {
        [SerializeField] private ForceModeType forceModeType = ForceModeType.PlayerOrientation;

        [SerializeField] private float verticalForceMagnitude = 200f, horizontalForceMagnitude = 200;

        [SerializeField] private AudioClip jumpSound;

        private AudioSource source;

        private void Start()
        {
            source = GetComponent<AudioSource>();
            source.clip = jumpSound;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                Vector3 hForceDirection = Vector3.zero;
                Vector3 vForceDirection = Vector3.zero;

                switch (forceModeType)
                {
                    case ForceModeType.PlayerOrientation:
                        hForceDirection = other.GetComponent<PlayerMovement>().orientation.forward;
                        vForceDirection = other.transform.up;
                        break;
                    case ForceModeType.PlayerMovementDirection:
                        hForceDirection = other.GetComponent<Rigidbody>().linearVelocity.normalized;
                        vForceDirection = other.transform.up;
                        break;
                    case ForceModeType.JumpPadLocalAxis:
                        hForceDirection = transform.right;
                        vForceDirection = transform.up;
                        break;
                    default:
                        vForceDirection = other.transform.up;
                        break;
                }

                playerRigidbody.AddForce(vForceDirection * verticalForceMagnitude, ForceMode.Impulse);
                playerRigidbody.AddForce(hForceDirection * horizontalForceMagnitude, ForceMode.Impulse);

                source.Play();
            }
        }
    }

}