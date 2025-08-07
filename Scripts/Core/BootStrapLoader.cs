using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Core {
    public class BootstrapLoader : MonoBehaviour {
        [Header("Bootstrap Configuration")]
        [SerializeField] private string initialSceneName = "MainMenu";
        [SerializeField] private GameObject[] managerPrefabs;
        [SerializeField] private bool showLoadingScreen = true;
        [SerializeField] private float minLoadingTime = 1f;

        private async void Start() {
            Debug.Log("[Bootstrap] Starting game initialization...");

            if (showLoadingScreen) {
                GameEvents.RequestLoadingStart("Initializing Game...");
            }

            try {
                // Initialize persistent managers
                InitializeManagers();
                
                // Wait minimum loading time
                await Task.Delay((int)(minLoadingTime * 1000));

                // Load initial scene
                await LoadInitialScene();

                Debug.Log("[Bootstrap] Game initialization complete!");
            } catch (Exception e) {
                Debug.LogError($"Bootstrap failed: {e.Message}");
            } finally {
                if (showLoadingScreen) {
                    GameEvents.RequestLoadingEnd();
                }
            }
        }

        private void InitializeManagers() {
            if (managerPrefabs == null || managerPrefabs.Length == 0) {
                Debug.LogWarning("[Bootstrap] No manager prefabs configured.");
                return;
            }

            foreach (var prefab in managerPrefabs) {
                if (prefab == null) continue;

                // Check if manager already exists
                var componentType = prefab.GetComponent<MonoBehaviour>()?.GetType();
                if (componentType != null && FindAnyObjectByType(componentType) == null) {
                    Instantiate(prefab);
                    Debug.Log($"[Bootstrap] Initialized {prefab.name}");
                }
            }
        }

        private async Task LoadInitialScene() {
            if (string.IsNullOrEmpty(initialSceneName)) {
                Debug.LogWarning("[Bootstrap] No initial scene configured!");
                return;
            }

            // Only load if not already in the scene
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != initialSceneName) {
                await SceneLoader.LoadSceneAsync(initialSceneName);
            }
        }
    }
}