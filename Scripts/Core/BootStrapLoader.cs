using System;
using System.Threading.Tasks;
using Core.GameState;
using UnityEngine;

namespace Core {
    public class BootstrapLoader : MonoBehaviour {
        [SerializeField] private BootstrapConfiguration config;
        [SerializeField] private SystemManager systemManager;

        private void Awake() {
            if (config == null) {
                Debug.LogError("Bootstrap Configuration is missing! Please assign it in the inspector.");
                return;
            }

            InitializeManagers();
        }

        private async void Start() {
            if (config == null) return;

            if (config.showLoadingScreen) {
                GameEvents.RequestLoadingStart("Initializing Game...");
            }

            try {
                await LoadInitialScene();
                systemManager?.NotifyBootstrapComplete();
            } catch (Exception e) {
                Debug.LogError($"Bootstrap failed: {e.Message}");
                // Make sure to end loading screen on error
                if (config.showLoadingScreen) {
                    GameEvents.RequestLoadingEnd();
                }
            }
        }

        private void InitializeManagers() {
            if (config.managerPrefabs == null || config.managerPrefabs.Length == 0) {
                if (config.enableDebugLogs)
                    Debug.LogWarning("No manager prefabs configured in Bootstrap Configuration.");
                return;
            }

            foreach (var prefab in config.managerPrefabs) {
                if (prefab == null) {
                    Debug.LogWarning("Null prefab found in manager prefabs list!");
                    continue;
                }

                var componentType = prefab.GetComponent<MonoBehaviour>()?.GetType();
                if (componentType == null) continue;

                if (FindAnyObjectByType(componentType) == null) {
                    Instantiate(prefab);
                } else if (config.enableDebugLogs) {
                    Debug.Log($"Manager {prefab.name} already exists, skipping initialization.");
                }
            }

            systemManager?.NotifySystemsInitialized();
        }

        private async Task LoadInitialScene() {
            if (string.IsNullOrEmpty(config.initialSceneName)) {
                Debug.LogError("Initial scene name is not configured!");
                return;
            }

            float startTime = Time.time;

            // Only load scene if we're not already in it
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != config.initialSceneName) {
                await SceneLoader.LoadSceneAsync(config.initialSceneName);
            }

            float elapsed = Time.time - startTime;
            if (elapsed < config.minLoadingTime)
                await Task.Delay((int)((config.minLoadingTime - elapsed) * 1000));

            // Only end loading screen if we started it
            if (config.showLoadingScreen) {
                GameEvents.RequestLoadingEnd();
            }
        }
    }
}