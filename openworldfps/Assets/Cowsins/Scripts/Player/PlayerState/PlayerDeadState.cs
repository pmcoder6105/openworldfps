using UnityEngine;
namespace cowsins
{
    public class PlayerDeadState : PlayerBaseState
    {

        public PlayerDeadState(PlayerStates currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            _ctx.GetComponent<PlayerStats>().LoseControl();
            _ctx.GetComponent<Rigidbody>().isKinematic = true;
        }

        public override void UpdateState()
        {
            CheckSwitchState();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() { _ctx.GetComponent<Rigidbody>().isKinematic = false; }

        public override void CheckSwitchState() { }


    }
}