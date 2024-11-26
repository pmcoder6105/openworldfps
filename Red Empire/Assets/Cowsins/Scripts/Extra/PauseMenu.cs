using UnityEngine;
using System;

namespace cowsins
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject playerUI;
        [SerializeField] private bool disablePlayerUIWhilePaused;
        [SerializeField] private CanvasGroup menu;
        [SerializeField] private float fadeSpeed;

        public static PauseMenu Instance { get; private set; }

        /// <summary>
        /// Returns the Pause State of the game
        /// </summary>
        public static bool isPaused { get; private set; }

        [HideInInspector] public PlayerStats stats;

        public event Action OnPause;
        public event Action OnUnpause;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initially, the game is not paused
            isPaused = false;
            menu.gameObject.SetActive(false);
            menu.alpha = 0;
        }

        private void Update()
        {
            HandlePauseInput();
            if (isPaused)
            {
                HandlePause();
            }
            else
            {
                HandleUnpause();
            }
        }

        private void HandlePauseInput()
        {
            if (InputManager.pausing)
            {
                TogglePause();
            }
        }

        private void HandlePause()
        {
            if (!menu.gameObject.activeSelf)
            {
                menu.gameObject.SetActive(true);
                menu.alpha = 0;
            }

            menu.alpha = Mathf.Min(menu.alpha + Time.deltaTime * fadeSpeed, 1);

            if (disablePlayerUIWhilePaused && !stats.isDead)
            {
                playerUI.SetActive(false);
            }
        }

        private void HandleUnpause()
        {
            menu.alpha = Mathf.Max(menu.alpha - Time.deltaTime * fadeSpeed, 0);

            if (menu.alpha <= 0)
            {
                menu.gameObject.SetActive(false);
            }

            playerUI.SetActive(true);
        }

        public void UnPause()
        {
            isPaused = false;
            stats.CheckIfCanGrantControl();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerUI.SetActive(true);

            OnUnpause?.Invoke();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void TogglePause()
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                stats.LoseControl();
                if (disablePlayerUIWhilePaused && !stats.isDead)
                {
                    playerUI.SetActive(false);
                }

                OnPause?.Invoke();
            }
            else
            {
                stats.CheckIfCanGrantControl();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerUI.SetActive(true);

                OnUnpause?.Invoke();
            }
        }
    }
}
