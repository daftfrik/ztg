using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using TMPro;

namespace Core {
    /// <summary>
    /// Manages game settings like audio, graphics, and controls.
    /// Attach this to your Settings Panel GameObject.
    /// </summary>
    public class SettingsManager : MonoBehaviour {
        [Header("Audio Settings")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle muteToggle;

        [Header("Graphics Settings")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Toggle vsyncToggle;

        [Header("Gameplay Settings")]
        [SerializeField] private Slider mouseSensitivitySlider;
        [SerializeField] private Toggle invertYToggle;

        [Header("UI References")]
        [SerializeField] private Text masterVolumeText;
        [SerializeField] private Text musicVolumeText;
        [SerializeField] private Text sfxVolumeText;
        [SerializeField] private Text mouseSensitivityText;

        [Header("Back Button")]
        [SerializeField] private Button backButton;
        [SerializeField] private SceneController sceneController;

        private Resolution[] _availableResolutions;
        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string SfxVolumeKey = "SFXVolume";
        private const string ResolutionKey = "Resolution";
        private const string QualityKey = "Quality";
        private const string FullscreenKey = "Fullscreen";
        private const string VsyncKey = "VSync";
        private const string MouseSensitivityKey = "MouseSensitivity";
        private const string InvertYKey = "InvertY";
        private const string MuteKey = "Mute";

        private void Start() {
            InitializeSettings();
            LoadSettings();
            SetupEventListeners();
        }

        private void InitializeSettings() {
            // Auto-find SceneController if not assigned
            if (sceneController == null) {
                sceneController = FindAnyObjectByType<SceneController>();
            }

            // Setup resolution dropdown
            SetupResolutionDropdown();

            // Setup quality dropdown
            SetupQualityDropdown();
        }

        private void SetupResolutionDropdown() {
            if (resolutionDropdown == null) return;

            _availableResolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> resolutionOptions = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < _availableResolutions.Length; i++) {
                string option = _availableResolutions[i].width + " x " + _availableResolutions[i].height;
                resolutionOptions.Add(option);

                if (_availableResolutions[i].width == Screen.currentResolution.width &&
                    _availableResolutions[i].height == Screen.currentResolution.height) {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(resolutionOptions);
            resolutionDropdown.value = currentResolutionIndex;
        }

        private void SetupQualityDropdown() {
            if (qualityDropdown == null) return;

            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
            qualityDropdown.value = QualitySettings.GetQualityLevel();
        }

        private void SetupEventListeners() {
            // Audio listeners
            masterVolumeSlider?.onValueChanged.AddListener(SetMasterVolume);
            musicVolumeSlider?.onValueChanged.AddListener(SetMusicVolume);
            sfxVolumeSlider?.onValueChanged.AddListener(SetSfxVolume);
            muteToggle?.onValueChanged.AddListener(SetMute);

            // Graphics listeners
            resolutionDropdown?.onValueChanged.AddListener(SetResolution);
            qualityDropdown?.onValueChanged.AddListener(SetQuality);
            fullscreenToggle?.onValueChanged.AddListener(SetFullscreen);
            vsyncToggle?.onValueChanged.AddListener(SetVSync);

            // Gameplay listeners
            mouseSensitivitySlider?.onValueChanged.AddListener(SetMouseSensitivity);
            invertYToggle?.onValueChanged.AddListener(SetInvertY);

            // Back button
            backButton?.onClick.AddListener(() => {
                SaveSettings();
                sceneController?.ShowMainMenu();
            });
        }

        #region Audio Settings
        public void SetMasterVolume(float volume) {
            if (audioMixer != null) {
                audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
            }
            UpdateVolumeText(masterVolumeText, volume);
        }

        public void SetMusicVolume(float volume) {
            if (audioMixer != null) {
                audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
            }
            UpdateVolumeText(musicVolumeText, volume);
        }

        public void SetSfxVolume(float volume) {
            if (audioMixer != null) {
                audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
            }
            UpdateVolumeText(sfxVolumeText, volume);
        }

        public void SetMute(bool isMuted) {
            AudioListener.pause = isMuted;
        }

        private void UpdateVolumeText(Text volumeText, float volume) {
            if (volumeText != null) {
                volumeText.text = Mathf.RoundToInt(volume * 100) + "%";
            }
        }
        #endregion

        #region Graphics Settings
        public void SetResolution(int resolutionIndex) {
            if (resolutionIndex >= 0 && resolutionIndex < _availableResolutions.Length) {
                Resolution resolution = _availableResolutions[resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
        }

        public void SetQuality(int qualityIndex) {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetFullscreen(bool isFullscreen) {
            Screen.fullScreen = isFullscreen;
        }

        public void SetVSync(bool enableVSync) {
            QualitySettings.vSyncCount = enableVSync ? 1 : 0;
        }
        #endregion

        #region Gameplay Settings
        public void SetMouseSensitivity(float sensitivity) {
            // You can implement this based on your input system
            if (mouseSensitivityText != null) {
                mouseSensitivityText.text = sensitivity.ToString("F1");
            }
        }

        public void SetInvertY(bool invert) {
            // You can implement this based on your input system
            Debug.Log($"Invert Y-axis: {invert}");
        }
        #endregion

        #region Save/Load Settings
        public void SaveSettings() {
            // Audio settings
            PlayerPrefs.SetFloat(MasterVolumeKey, masterVolumeSlider?.value ?? 0.75f);
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolumeSlider?.value ?? 0.75f);
            PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolumeSlider?.value ?? 0.75f);
            PlayerPrefs.SetInt(MuteKey, muteToggle?.isOn == true ? 1 : 0);

            // Graphics settings
            PlayerPrefs.SetInt(ResolutionKey, resolutionDropdown?.value ?? 0);
            PlayerPrefs.SetInt(QualityKey, qualityDropdown?.value ?? 2);
            PlayerPrefs.SetInt(FullscreenKey, fullscreenToggle?.isOn == true ? 1 : 0);
            PlayerPrefs.SetInt(VsyncKey, vsyncToggle?.isOn == true ? 1 : 0);

            // Gameplay settings
            PlayerPrefs.SetFloat(MouseSensitivityKey, mouseSensitivitySlider?.value ?? 1.0f);
            PlayerPrefs.SetInt(InvertYKey, invertYToggle?.isOn == true ? 1 : 0);

            PlayerPrefs.Save();
            Debug.Log("[SettingsManager] Settings saved!");
        }

        public void LoadSettings() {
            // Audio settings
            float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 0.75f);
            float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.75f);
            float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.75f);
            bool isMuted = PlayerPrefs.GetInt(MuteKey, 0) == 1;

            if (masterVolumeSlider != null) {
                masterVolumeSlider.value = masterVolume;
                SetMasterVolume(masterVolume);
            }
            if (musicVolumeSlider != null) {
                musicVolumeSlider.value = musicVolume;
                SetMusicVolume(musicVolume);
            }
            if (sfxVolumeSlider != null) {
                sfxVolumeSlider.value = sfxVolume;
                SetSfxVolume(sfxVolume);
            }
            if (muteToggle != null) {
                muteToggle.isOn = isMuted;
                SetMute(isMuted);
            }

            // Graphics settings
            int resolutionIndex = PlayerPrefs.GetInt(ResolutionKey, _availableResolutions.Length - 1);
            int qualityIndex = PlayerPrefs.GetInt(QualityKey, 2);
            bool isFullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
            bool enableVSync = PlayerPrefs.GetInt(VsyncKey, 1) == 1;

            if (resolutionDropdown != null && resolutionIndex < _availableResolutions.Length) {
                resolutionDropdown.value = resolutionIndex;
            }
            if (qualityDropdown != null) {
                qualityDropdown.value = qualityIndex;
            }
            if (fullscreenToggle != null) {
                fullscreenToggle.isOn = isFullscreen;
            }
            if (vsyncToggle != null) {
                vsyncToggle.isOn = enableVSync;
            }

            // Gameplay settings
            float mouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, 1.0f);
            bool invertY = PlayerPrefs.GetInt(InvertYKey, 0) == 1;

            if (mouseSensitivitySlider != null) {
                mouseSensitivitySlider.value = mouseSensitivity;
                SetMouseSensitivity(mouseSensitivity);
            }
            if (invertYToggle != null) {
                invertYToggle.isOn = invertY;
            }

            Debug.Log("[SettingsManager] Settings loaded!");
        }

        public void ResetToDefaults() {
            // Reset all settings to default values
            if (masterVolumeSlider != null) masterVolumeSlider.value = 0.75f;
            if (musicVolumeSlider != null) musicVolumeSlider.value = 0.75f;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = 0.75f;
            if (muteToggle != null) muteToggle.isOn = false;
            
            if (qualityDropdown != null) qualityDropdown.value = 2;
            if (fullscreenToggle != null) fullscreenToggle.isOn = true;
            if (vsyncToggle != null) vsyncToggle.isOn = true;
            
            if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = 1.0f;
            if (invertYToggle != null) invertYToggle.isOn = false;

            Debug.Log("[SettingsManager] Settings reset to defaults!");
        }
        #endregion

        private void OnDestroy() {
            // Clean up listeners
            masterVolumeSlider?.onValueChanged.RemoveAllListeners();
            musicVolumeSlider?.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider?.onValueChanged.RemoveAllListeners();
            muteToggle?.onValueChanged.RemoveAllListeners();
            resolutionDropdown?.onValueChanged.RemoveAllListeners();
            qualityDropdown?.onValueChanged.RemoveAllListeners();
            fullscreenToggle?.onValueChanged.RemoveAllListeners();
            vsyncToggle?.onValueChanged.RemoveAllListeners();
            mouseSensitivitySlider?.onValueChanged.RemoveAllListeners();
            invertYToggle?.onValueChanged.RemoveAllListeners();
            backButton?.onClick.RemoveAllListeners();
        }
    }
}