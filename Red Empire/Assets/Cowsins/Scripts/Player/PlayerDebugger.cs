using UnityEngine;

namespace cowsins
{
    public class PlayerDebugger : MonoBehaviour
    {
        private readonly float topMargin = 130f;

        private PlayerStats playerStats;
        private PlayerStates playerStates;
        private PlayerMovement playerMovement;
        private WeaponController weaponController;
        private WeaponStates weaponStates;
        private InteractManager interactManager;
        private Rigidbody rb;

        private void Start()
        {
            playerStats = GetComponent<PlayerStats>();
            playerStates = GetComponent<PlayerStates>();
            playerMovement = GetComponent<PlayerMovement>();
            weaponController = GetComponent<WeaponController>();
            weaponStates = GetComponent<WeaponStates>();
            interactManager = GetComponent<InteractManager>();
            rb = GetComponent<Rigidbody>();
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILDw
        void OnGUI()
        {
            // Define the background box dimensions
            float boxWidth = 320f;

            // Draw the background box
            GUI.Box(new Rect(10, topMargin, boxWidth, 110), "Player Information");

            GUI.Label(new Rect(20, topMargin + 20, boxWidth - 20, 20), "Player Velocity: " + Mathf.Round(rb.linearVelocity.magnitude));

            GUI.Label(new Rect(20, topMargin + 40, boxWidth - 20, 20), "Player Position: " + transform.position);

            GUI.Label(new Rect(20, topMargin + 60, boxWidth - 20, 20), "Player Orientation: " + playerMovement.orientation.forward);

            GUI.Label(new Rect(20, topMargin + 80, boxWidth - 20, 20), "Player State: " + playerStates.CurrentState);

            GUI.Box(new Rect(10, topMargin + 120, boxWidth, 170), "Weapon Information");

            GUI.Label(new Rect(20, topMargin + 140, boxWidth - 20, 20), "Weapon_SO: " + weaponController.weapon);
            GUI.Label(new Rect(20, topMargin + 160, boxWidth - 20, 20), "Weapon Object: " + weaponController.id);
            GUI.Label(new Rect(20, topMargin + 180, boxWidth - 20, 20), "Weapon Total Bullets: " + weaponController.id?.totalBullets);
            GUI.Label(new Rect(20, topMargin + 200, boxWidth - 20, 20), "Weapon Current Bullets: " + weaponController.id?.bulletsLeftInMagazine);
            GUI.Label(new Rect(20, topMargin + 220, boxWidth - 20, 20), "Reloading: " + weaponController.Reloading);
            GUI.Label(new Rect(20, topMargin + 240, boxWidth - 20, 20), "Weapon State: " + weaponStates.CurrentState);
            GUI.Label(new Rect(20, topMargin + 260, boxWidth - 20, 20), "Weapon aiming: " + weaponController.isAiming);
            GUI.Box(new Rect(10, topMargin + 300, boxWidth, 70), "Player Stats Information");

            GUI.Label(new Rect(20, topMargin + 320, boxWidth - 20, 20), "Health: " + Mathf.Round(playerStats.health) + " / " + playerStats.maxHealth);
            GUI.Label(new Rect(20, topMargin + 340, boxWidth - 20, 20), "Shield: " + Mathf.Round(playerStats.shield) + " / " + playerStats.maxShield);

            GUI.Box(new Rect(10, topMargin + 380, boxWidth, 70), "Interact Manager Information");

            GUI.Label(new Rect(20, topMargin + 400, boxWidth - 20, 20), "Highlighted Interactable: " + interactManager.HighlightedInteractable?.name);
            GUI.Label(new Rect(20, topMargin + 420, boxWidth - 20, 20), "Interact Progress: " + interactManager.progressElapsed.ToString("F1"));


            GUI.Box(new Rect(10, topMargin + 460, boxWidth, 300), "Input Manager");
            GUI.Label(new Rect(20, topMargin + 480, boxWidth - 20, 20), "Movement: (" + InputManager.x.ToString("F1") + "," + InputManager.y.ToString("F1") + ")");
            GUI.Label(new Rect(20, topMargin + 500, boxWidth - 20, 20), "Look: (" + InputManager.mousex.ToString("F1") + "," + InputManager.mousey.ToString("F1") + ")");
            GUI.Label(new Rect(20, topMargin + 520, boxWidth - 20, 20), "Gamepad Look: (" + InputManager.controllerx.ToString("F1") + "," + InputManager.controllery.ToString("F1") + ")");
            GUI.Label(new Rect(20, topMargin + 540, boxWidth - 20, 20), "Shooting: " + InputManager.shooting);
            GUI.Label(new Rect(20, topMargin + 560, boxWidth - 20, 20), "Reloading: " + InputManager.reloading);
            GUI.Label(new Rect(20, topMargin + 580, boxWidth - 20, 20), "Aiming: " + InputManager.aiming);
            GUI.Label(new Rect(20, topMargin + 600, boxWidth - 20, 20), "Sprinting: " + InputManager.sprinting);
            GUI.Label(new Rect(20, topMargin + 620, boxWidth - 20, 20), "Crouching: " + InputManager.crouching);
            GUI.Label(new Rect(20, topMargin + 640, boxWidth - 20, 20), "Interacting: " + InputManager.interacting);
        }
#endif
    }

}