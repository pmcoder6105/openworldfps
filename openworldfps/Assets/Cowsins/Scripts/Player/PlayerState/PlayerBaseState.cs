namespace cowsins
{
    public abstract class PlayerBaseState
    {
        protected PlayerStates _ctx;
        protected PlayerStateFactory _factory;

        public PlayerBaseState(PlayerStates currentContext, PlayerStateFactory playerStateFactory)
        {
            _ctx = currentContext;
            _factory = playerStateFactory;
        }


        public abstract void EnterState();

        public abstract void UpdateState();

        public abstract void FixedUpdateState();

        public abstract void ExitState();

        public abstract void CheckSwitchState();

        void UpdateStates() { }

        protected void SwitchState(PlayerBaseState newState)
        {
            ExitState();

            newState.EnterState();

            _ctx.CurrentState = newState;
        }
    }
}