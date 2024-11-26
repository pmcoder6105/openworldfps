using UnityEngine;
namespace cowsins
{
    public class Flashlight : Attachment
    {
        [Header("FLASHLIGHT")]
        [Tooltip("Light for the flashlight")] public Light lightSource;
        [Tooltip("SFX for turning on and off.")] public AudioClip turnOnSFX, turnOffSFX;

        private bool turnedOn;

        public bool TurnedOn { get { return turnedOn; } }

        private void Start()
        {
            turnedOn = false;
        }

        public void CheckIfCanTurnOn(bool cond)
        {
            // Check if we can turn it on
            if (cond)
            {
                // Conditions are met, turn off
                turnedOn = false;
                return;
            }
            // Turn on
            turnedOn = true;
        }

        /// <summary>
        /// Forces the flashlight to turn on.
        /// </summary>
        /// <param name="cond"></param>
        public void EnableFlashLight(bool cond)
        {
            // If the condition is met, enable the flashlight light, if not, disable it and turn it off.
            if (cond) lightSource.gameObject.SetActive(true);
            else
            {
                turnedOn = false;
                lightSource.gameObject.SetActive(false);
            }
        }
    }
}