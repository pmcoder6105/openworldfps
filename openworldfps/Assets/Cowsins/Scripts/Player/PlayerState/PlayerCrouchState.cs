using UnityEngine;
namespace cowsins
{
    public class PlayerCrouchState : PlayerBaseState
    {

        public PlayerCrouchState(PlayerStates currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) { }

        private PlayerMovement player;

        private PlayerStats stats;

        private WeaponController weaponController;

        public override void EnterState()
        {
            player = _ctx.GetComponent<PlayerMovement>();
            stats = _ctx.GetComponent<PlayerStats>();
            weaponController = _ctx.GetComponent<WeaponController>();

            if (!player.allowCrouch)
            {
                SwitchState(_factory.Default());
                return;
            }
            player.events.OnCrouch.Invoke();
            player.StartCrouch();

            weaponController.SetCrouchCamShakeMultiplier();
        }

        public override void UpdateState()
        {
            if (InputManager.crouching)
            {
                _ctx.transform.localScale = Vector3.MoveTowards(_ctx.transform.localScale, player.crouchScale, Time.deltaTime * player.crouchTransitionSpeed * 1.5f);
                if (player.crouchCancelMethod == PlayerMovement.CrouchCancelMethod.Smooth) player.currentSpeed = Mathf.MoveTowards(player.currentSpeed, player.crouchSpeed, Time.deltaTime * player.crouchTransitionSpeed * 1.5f);
            }
            player.isCrouching = true;
            CheckSwitchState();
            player.Look();
        }
        public override void FixedUpdateState()
        {
            HandleMovement();
        }

        public override void ExitState() { weaponController.ResetCrouchCamShakeMultiplier(); }

        public override void CheckSwitchState()
        {

            if (player.ReadyToJump && InputManager.jumping && player.canJumpWhileCrouching && (player.EnoughStaminaToJump && player.grounded || player.wallRunning || player.jumpCount > 0 && player.maxJumps > 1 && player.EnoughStaminaToJump))
                SwitchState(_factory.Jump());

            if (_ctx.GetComponent<PlayerStats>().health <= 0) SwitchState(_factory.Die());

            if (player.canDash && InputManager.dashing && (player.infiniteDashes || player.currentDashes > 0 && !player.infiniteDashes)) SwitchState(_factory.Dash());


            CheckUnCrouch();

        }

        private bool canUnCrouch = false;

        void HandleMovement()
        {
            player.Movement(stats.controllable);
            player.FootSteps();
            player.HandleSpeedLines();
        }

        void CheckUnCrouch()
        {
            if (!InputManager.crouching) // Prevent from uncrouching when there´s a roof and we can get hit with it
            {
                RaycastHit hit;
                bool isObstacleAbove = Physics.Raycast(_ctx.transform.position, _ctx.transform.up, out hit, player.RoofCheckDistance, player.weaponController.hitLayer);

                canUnCrouch = !isObstacleAbove;
            }
            if (canUnCrouch)
            {
                player.events.OnStopCrouch.Invoke(); // Invoke your own method on the moment you are standing up NOT WHILE YOU ARE NOT CROUCHING
                _ctx.GetComponent<PlayerMovement>().StopCrouch();
                if (_ctx.transform.localScale == player.PlayerScale)
                    SwitchState(_factory.Default());
            }
        }

    }
}