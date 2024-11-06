using UnityEngine;
using UnityEngine.InputSystem;
namespace cowsins
{
    public class ShowAndHide : MonoBehaviour
    {
        [SerializeField] private GameObject panel;

        private bool input, holding;

        private void Awake() => holding = false;
        private void Update()
        {
            input = Keyboard.current.qKey.isPressed;
            if (!input) holding = false;

            if (input && !holding)
            {
                holding = true;
                if (panel.activeSelf == false) panel.SetActive(true);
                else panel.SetActive(false);
            }
        }
    }
}