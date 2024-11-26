using UnityEngine;
namespace cowsins
{
    public class PlayerClimbState : PlayerBaseState
    {

        public PlayerClimbState(PlayerStates currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) { }

        private PlayerMovement player;

        private Rigidbody rb;

        public override void EnterState()
        {
            player = _ctx.GetComponent<PlayerMovement>();
            rb = _ctx.GetComponent<Rigidbody>();
            rb.useGravity = false;
            player.Climbing = true;
            rb.linearVelocity = Vector3.zero;

            player.events.OnSpawn.Invoke();
        }

        public override void UpdateState()
        {
            player.HandleClimbMovement();
            player.VerticalLook();
            // Prevents speedlines from showing up
            player.HandleSpeedLines();
            CheckSwitchState();
        }

        public override void FixedUpdateState() { }

        public override void ExitState()
        {
            player.events.OnEndClimb.Invoke();
            rb.useGravity = true;
            player.Climbing = false;
            player.HandleLadderFinishMotion();
        }

        public override void CheckSwitchState()
        {
            if (InputManager.jumping || player.grounded && InputManager.y < 0) SwitchState(_factory.Default());
            if (player.DetectTopLadder())
            {
                player.Climbing = false;
                rb.useGravity = true;
                player.ReachTopLadder();
                SwitchState(_factory.Default());
            }

        }

    }
}