using UnityEngine;
namespace cowsins
{
    public class PlayerJumpState : PlayerBaseState
    {
        public PlayerJumpState(PlayerStates currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) { }

        private PlayerMovement player;

        private PlayerStats stats;

        public override void EnterState()
        {
            player = _ctx.GetComponent<PlayerMovement>();
            stats = _ctx.GetComponent<PlayerStats>();
            player.events.OnJump.Invoke();
            player.Jump();
        }

        public override void UpdateState()
        {
            CheckSwitchState();
            HandleMovement();
            CheckUnCrouch();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() { }

        public override void CheckSwitchState()
        {
            if (player.DetectLadders())
            {
                SwitchState(_factory.Climb());
                return;
            }

            bool canJump = player.ReadyToJump && InputManager.jumping &&
                           (player.EnoughStaminaToJump && player.grounded ||
                            player.wallRunning ||
                            player.jumpCount > 0 && player.maxJumps > 1 && player.EnoughStaminaToJump);

            if (canJump)
            {
                SwitchState(_factory.Jump());
                return;
            }

            if (stats.health <= 0)
            {
                SwitchState(_factory.Die());
                return;
            }

            if (player.grounded || player.wallRunning)
            {
                SwitchState(_factory.Default());
                return;
            }

            bool canDash = player.canDash && InputManager.dashing &&
                           (player.infiniteDashes ||
                            player.currentDashes > 0 && !player.infiniteDashes);

            if (canDash)
            {
                SwitchState(_factory.Dash());
                return;
            }

            bool canCrouch = InputManager.crouchingDown && !player.wallRunning &&
                             player.allowCrouch && player.allowCrouchWhileJumping;

            if (canCrouch)
            {
                SwitchState(_factory.Crouch());
                return;
            }

            // Check Grapple
            if (player.allowGrapple)
            {
                player.HandleGrapple();
                player.UpdateGrappleRenderer();
            }
        }

        void HandleMovement()
        {
            player.Movement(stats.controllable);
            player.Look();
        }

        private bool canUnCrouch = false;

        private void CheckUnCrouch()
        {
            if (!InputManager.crouching)
            {
                // Check if there is a roof above the player to prevent uncrouching
                RaycastHit hit;
                bool isObstacleAbove = Physics.Raycast(_ctx.transform.position, _ctx.transform.up, out hit, player.RoofCheckDistance, player.weaponController.hitLayer);

                canUnCrouch = !isObstacleAbove;
            }

            if (canUnCrouch)
            {
                // Invoke event and stop crouching when it is safe to do so
                player.events.OnStopCrouch.Invoke();
                player.StopCrouch();
            }
        }

    }
}