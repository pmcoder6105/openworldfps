#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine;
using TMPro;
namespace cowsins
{
    public class DisplayKey : MonoBehaviour
    {
        public static PlayerActions inputActions;
        private TextMeshProUGUI txt;
        private void Awake()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerActions();
                inputActions.Enable();
            }
            txt = GetComponent<TextMeshProUGUI>();
        }

        private void Update() => Repaint();

        public void Repaint()
        {
            string deviceGroup = DeviceDetection.Instance.mode == DeviceDetection.InputMode.Keyboard ? "Keyboard" : "Gamepad";
            txt.text = inputActions.GameControls.Interacting.GetBindingDisplayString(InputBinding.MaskByGroup(deviceGroup));
        }
    }
}