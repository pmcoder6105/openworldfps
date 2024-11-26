using UnityEngine;
using UnityEngine.Events;

namespace cowsins
{
    public class Experience : MonoBehaviour
    {
        [SerializeField] private float minXp, maxXp;

        [SerializeField, Tooltip("Sound on picking up ")] private AudioClip pickUpSFX;

        public UnityEvent onCollect;

        [SerializeField] private Transform graphics;

        [Tooltip("Apply the selected effect")]
        public bool rotates, translates;

        [Tooltip("Change the speed of the selected effect"), SerializeField]
        private float rotationSpeed, translationSpeed;

        private float timer = 0f;

        private void OnTriggerEnter(Collider other)
        {
            if (ExperienceManager.instance == null) return; // If we are not using XP in our game we should not pick up XP or we should not be able to.

            onCollect?.Invoke();

            // Generate a random amount of XP.
            float amount = Random.Range(minXp, maxXp);
            // Add the experience to the player.
            ExperienceManager.instance.AddExperience(amount);
            UIController.addXP?.Invoke();

            // Play SFX
            SoundManager.Instance.PlaySound(pickUpSFX, 0, 0, false, 0);
            UIController.instance.UpdateXPPanel();
            // Destroy the XP.
            Destroy(this.gameObject);
        }

        private void Update() => Movement();

        private void Movement()
        {
            if (!rotates && !translates) return;
            if (rotates) graphics.Rotate(Vector3.up * rotationSpeed * Time.deltaTime); // Rotate over time
            if (translates) // Go up and down
            {
                timer += Time.deltaTime * translationSpeed; // Timer that controls the movement
                float translateMotion = Mathf.Sin(timer) / 2000f;
                graphics.transform.localPosition = new Vector3(graphics.transform.localPosition.x, graphics.transform.localPosition.y + translateMotion, graphics.transform.localPosition.z);
            }
        }

    }
}
