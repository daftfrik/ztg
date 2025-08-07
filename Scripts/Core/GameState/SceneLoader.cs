using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.GameState {
    public static class SceneLoader {
        public static async Task LoadSceneAsync(string sceneName) {
            // Don't load if we're already in the target scene
            if (SceneManager.GetActiveScene().name == sceneName) {
                Debug.Log($"[SceneLoader] Already in scene {sceneName}, skipping load");
                return;
            }
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            // Report progress through events, but be more careful about it
            float lastProgress = 0f;
            while (asyncLoad is not { isDone: true }) {
                if (asyncLoad != null && !Mathf.Approximately(asyncLoad.progress, lastProgress)) {
                    lastProgress = asyncLoad.progress;
                    GameEvents.RequestLoadingProgress(asyncLoad.progress);
                }
                await Task.Yield();
            }
            
            GameEvents.RequestLoadingProgress(1f);
            
            await Task.Delay(100); // Small delay for smooth transition instead
        }
    }
}