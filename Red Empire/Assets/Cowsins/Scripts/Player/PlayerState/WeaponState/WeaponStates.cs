using UnityEngine;
namespace cowsins
{
    public class WeaponStates : MonoBehaviour
    {
        WeaponBaseState _currentState;
        WeaponStateFactory _states;

        public WeaponBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        public WeaponStateFactory _States { get { return _states; } set { _states = value; } }

        [HideInInspector] public CanvasGroup inspectionUI;

        static WeaponStates _instance;
        public static WeaponStates instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;

            _states = new WeaponStateFactory(this);
            _currentState = _states.Default();
            _currentState.EnterState();
        }

        private void Update()
        {
            _currentState.UpdateState();
        }

        private void FixedUpdate()
        {
            _currentState.FixedUpdateState();
        }

        /// <summary>
        /// Force to change a Weapon state by passing the desired new state.
        /// </summary>
        /// <param name="newState"></param>
        public void ForceChangeState(WeaponBaseState newState)
        {
            _currentState.ExitState();
            _currentState = newState;
            _currentState.EnterState();
        }
    }
}