using UnityEngine;

namespace cowsins
{
    public class WeaponAnimator : MonoBehaviour
    {
        private PlayerMovement player;
        private WeaponController wc;
        private InteractManager interactManager;
        private Rigidbody rb;

        void Start()
        {
            player = GetComponent<PlayerMovement>();
            wc = GetComponent<WeaponController>();
            interactManager = GetComponent<InteractManager>();
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (wc.inventory[wc.currentWeapon] == null) return;

            Animator currentAnimator = wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>();

            if (player.wallRunning && !wc.Reloading)
            {
                CowsinsUtilities.PlayAnim("walking", currentAnimator);
                return;

            }
            if (wc.Reloading || wc.shooting || player.isCrouching || !player.grounded || rb.linearVelocity.magnitude < 0.1f || wc.isAiming
                || currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unholster")
                || currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("reloading")
                || currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("shooting"))
            {
                CowsinsUtilities.StopAnim("walking", currentAnimator);
                CowsinsUtilities.StopAnim("running", currentAnimator);
                return;
            }

            if (rb.linearVelocity.magnitude > player.crouchSpeed && !wc.shooting && player.currentSpeed < player.runSpeed && player.grounded && !interactManager.inspecting) CowsinsUtilities.PlayAnim("walking", currentAnimator);
            else CowsinsUtilities.StopAnim("walking", currentAnimator);

            if (player.currentSpeed >= player.runSpeed && player.grounded) CowsinsUtilities.PlayAnim("running", currentAnimator);
            else CowsinsUtilities.StopAnim("running", currentAnimator);
        }

        public void StopWalkAndRunMotion()
        {
            if (!wc) return; // Ensure there is a reference for the Weapon Controller before running the following code
            Animator weapon = wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>();
            CowsinsUtilities.StopAnim("inspect", weapon);
            CowsinsUtilities.StopAnim("walking", weapon);
            CowsinsUtilities.StopAnim("running", weapon);
        }
    }

}