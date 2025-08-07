using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core {
    public static class SceneLoader {
        public static async Task LoadSceneAsync(string sceneName) {
            if (SceneManager.GetActiveScene().name == sceneName) {
                Debug.Log($"[SceneLoader] Already in scene {sceneName}, skipping load");
                return;
            }
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            float lastProgress = 0f;
            while (asyncLoad is not { isDone: true }) {
                if (asyncLoad != null && !Mathf.Approximately(asyncLoad.progress, lastProgress)) {
                    lastProgress = asyncLoad.progress;
                    GameEvents.RequestLoadingProgress(asyncLoad.progress);
                }
                await Task.Yield();
            }
            
            GameEvents.RequestLoadingProgress(1f);
            
            await Task.Delay(100);
        }
    }
}