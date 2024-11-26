using UnityEngine;
using UnityEngine.InputSystem;

namespace cowsins
{
    public class DeviceDetection : MonoBehaviour
    {
        public enum InputMode
        {
            Keyboard, Controller
        }

        public InputMode mode { get; private set; }

        public static DeviceDetection Instance;

        private float controllerInputTimeout = .2f; // Time to wait before switching from Controller mode
        private float timeSinceLastControllerInput = 0f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Update() => DetectInputs();

        public void DetectInputs()
        {
            bool keyboardInputReceived = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool mouseInputReceived = Mouse.current != null && (
                Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.middleButton.wasPressedThisFrame ||
                Mouse.current.delta.ReadValue().magnitude > 0.1f
            );
            bool controllerInputReceived = Gamepad.current != null && (
                Gamepad.current.buttonSouth.wasPressedThisFrame ||
                Gamepad.current.buttonNorth.wasPressedThisFrame ||
                Gamepad.current.buttonEast.wasPressedThisFrame ||
                Gamepad.current.buttonWest.wasPressedThisFrame ||
                Gamepad.current.leftTrigger.wasPressedThisFrame ||
                Gamepad.current.rightTrigger.wasPressedThisFrame ||
                Gamepad.current.leftStick.IsPressed() ||
                Gamepad.current.rightStick.IsPressed() ||
                Gamepad.current.dpad.up.wasPressedThisFrame ||
                Gamepad.current.dpad.down.wasPressedThisFrame ||
                Gamepad.current.dpad.left.wasPressedThisFrame ||
                Gamepad.current.dpad.right.wasPressedThisFrame ||
                Gamepad.current.leftStick.ReadValue().magnitude > 0.1f ||
                Gamepad.current.rightStick.ReadValue().magnitude > 0.1f
            );

            // Prioritize Controller input
            if (controllerInputReceived)
            {
                mode = InputMode.Controller;
                timeSinceLastControllerInput = 0f;
            }
            else
            {
                timeSinceLastControllerInput += Time.deltaTime;

                if (timeSinceLastControllerInput > controllerInputTimeout)
                {
                    if (keyboardInputReceived || mouseInputReceived)
                    {
                        mode = InputMode.Keyboard;
                    }
                }
            }
        }
    }
}
