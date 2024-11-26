using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace cowsins
{
    public class GameSettingsManager : MonoBehaviour
    {
        [HideInInspector] public int fullScreen;
        [HideInInspector] public int res;
        [HideInInspector] public int maxFrameRate;
        [HideInInspector] public int vsync;
        [HideInInspector] public int graphicsQuality;
        [HideInInspector] public float masterVolume;
        [HideInInspector] public float playerSensX, playerSensY, playerControllerSensX, playerControllerSensY;

        [SerializeField] private TMP_Dropdown frameRateDropdown, resolutionRateDropdown, graphicsDropdown;
        [SerializeField] private Toggle fullScreenToggle, vsyncToggle;
        [SerializeField] private Slider masterVolumeSlider, playerSensXSlider, playerSensYSlider, playerControllerSensXSlider, playerControllerSensYSlider;
        [SerializeField] private TextMeshProUGUI playerSensXDisplay, playerSensYDisplay, playerControllerSensXDisplay, playerControllerSensYDisplay;
        [SerializeField] private AudioMixer masterMixer;

        // Stores all the supported resolutions by your monitor
        private Resolution[] availableResolutions;

        public static GameSettingsManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                // We need to ensure this object overrides the already existing Instance of GameSettingsManager
                Destroy(Instance.gameObject);
                Instance = this;
            }

            DontDestroyOnLoad(gameObject);
            LoadSettings();
            InitializeUI();
        }

        public void SetWindowedScreen() => fullScreen = 0;
        public void SetFullScreen() => fullScreen = 1;

        /// <summary>
        /// Saves the settings to PlayerPrefs
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.SetInt("res", res);
            PlayerPrefs.SetInt("fullScreen", fullScreen);
            PlayerPrefs.SetInt("maxFrameRate", maxFrameRate);
            PlayerPrefs.SetInt("vsync", vsync);
            PlayerPrefs.SetInt("graphicsQuality", graphicsQuality);
            PlayerPrefs.SetFloat("masterVolume", masterVolume);
            PlayerPrefs.SetFloat("playerSensX", playerSensX);
            PlayerPrefs.SetFloat("playerSensY", playerSensY);
            PlayerPrefs.SetFloat("playerControllerSensX", playerControllerSensX);
            PlayerPrefs.SetFloat("playerControllerSensY", playerControllerSensY);
        }

        /// <summary>
        /// Loads, updates UI and applies all the settings
        /// </summary>
        public void LoadSettings()
        {
            availableResolutions = Screen.resolutions;

            masterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
            playerSensX = PlayerPrefs.GetFloat("playerSensX", 4f);
            playerSensY = PlayerPrefs.GetFloat("playerSensY", 4f);
            playerControllerSensX = PlayerPrefs.GetFloat("playerControllerSensX", 35f);
            playerControllerSensY = PlayerPrefs.GetFloat("playerControllerSensY", 35f);

            res = PlayerPrefs.GetInt("res", availableResolutions.Length - 1);
            fullScreen = PlayerPrefs.GetInt("fullScreen", 1);
            int savedFrameRate = PlayerPrefs.GetInt("maxFrameRate", -1);
            maxFrameRate = (savedFrameRate >= 0 && savedFrameRate < frameRateDropdown.options.Count) ? savedFrameRate : frameRateDropdown.options.Count - 1;
            vsync = PlayerPrefs.GetInt("vsync", 0);
            graphicsQuality = PlayerPrefs.GetInt("graphicsQuality", 2);

            ApplySettings();
            UpdateUIElements();
        }

        public void ResetSettings()
        {
            res = availableResolutions.Length - 1;
            fullScreen = 1;
            maxFrameRate = frameRateDropdown.options.Count - 1;
            vsync = 0;
            graphicsQuality = 2;
            masterVolume = 1;
            playerSensX = 4;
            playerSensY = 4;
            playerControllerSensX = 35;
            playerControllerSensY = 35;

            SaveSettings();
            LoadSettings();
        }

        private void InitializeUI()
        {
            PopulateResolutionDropdown();

            frameRateDropdown.onValueChanged.AddListener(delegate { maxFrameRate = frameRateDropdown.value; });
            resolutionRateDropdown.onValueChanged.AddListener(delegate { res = resolutionRateDropdown.value; });
            graphicsDropdown.onValueChanged.AddListener(delegate { graphicsQuality = graphicsDropdown.value; });

            fullScreenToggle.onValueChanged.AddListener(delegate { fullScreen = fullScreenToggle.isOn ? 1 : 0; });
            vsyncToggle.onValueChanged.AddListener(delegate { vsync = vsyncToggle.isOn ? 1 : 0; });

            masterVolumeSlider.onValueChanged.AddListener(delegate
            {
                masterVolume = masterVolumeSlider.value;
                masterMixer.SetFloat("Volume", Mathf.Log10(masterVolume) * 20);
            });

            playerSensXSlider.onValueChanged.AddListener(delegate
            {
                playerSensX = playerSensXSlider.value;
                playerSensXDisplay.text = playerSensX.ToString("F1");
            });

            playerSensYSlider.onValueChanged.AddListener(delegate
            {
                playerSensY = playerSensYSlider.value;
                playerSensYDisplay.text = playerSensY.ToString("F1");
            });

            playerControllerSensXSlider.onValueChanged.AddListener(delegate
            {
                playerControllerSensX = playerControllerSensXSlider.value;
                playerControllerSensXDisplay.text = playerControllerSensX.ToString("F1");
            });

            playerControllerSensYSlider.onValueChanged.AddListener(delegate
            {
                playerControllerSensY = playerControllerSensYSlider.value;
                playerControllerSensYDisplay.text = playerControllerSensY.ToString("F1");
            });
        }

        private void PopulateResolutionDropdown()
        {
            resolutionRateDropdown.ClearOptions();
            availableResolutions = Screen.resolutions;

            List<string> options = new List<string>();
            for (int i = 0; i < availableResolutions.Length; i++)
            {
                Resolution res = availableResolutions[i];
                options.Add($"{res.width} x {res.height} @ {res.refreshRate}Hz");
            }

            resolutionRateDropdown.AddOptions(options);
            resolutionRateDropdown.value = res;
            resolutionRateDropdown.RefreshShownValue();
        }

        private void ApplySettings()
        {
            Application.targetFrameRate = maxFrameRate == 0 ? 60 : (maxFrameRate == 1 ? 120 : (maxFrameRate == 2 ? 230 : 300));

            Resolution selectedResolution = availableResolutions[res];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullScreen == 1);

            QualitySettings.vSyncCount = vsync;
            QualitySettings.SetQualityLevel(graphicsQuality);
        }

        private void UpdateUIElements()
        {
            frameRateDropdown.value = maxFrameRate;
            resolutionRateDropdown.value = res;
            graphicsDropdown.value = graphicsQuality;
            fullScreenToggle.isOn = fullScreen == 1;
            vsyncToggle.isOn = vsync == 1;
            masterVolumeSlider.value = masterVolume;

            playerSensXSlider.value = playerSensX;
            playerSensYSlider.value = playerSensY;
            playerControllerSensXSlider.value = playerControllerSensX;
            playerControllerSensYSlider.value = playerControllerSensY;

            playerSensXDisplay.text = playerSensX.ToString("F1");
            playerSensYDisplay.text = playerSensY.ToString("F1");
            playerControllerSensXDisplay.text = playerControllerSensX.ToString("F1");
            playerControllerSensYDisplay.text = playerControllerSensY.ToString("F1");
        }

        private void OnSceneChanged(Scene current, Scene next)
        {
            availableResolutions = Screen.resolutions;
            PlayerPrefs.Save();
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }
    }
}
