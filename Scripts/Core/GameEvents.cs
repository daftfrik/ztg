using System;
using UnityEngine;

namespace Core {
    public static class GameEvents {
        // UI Events
        public static event Action<string> OnUIShowRequested;
        public static event Action<string> OnUIHideRequested;
        public static event Action OnAllUIHideRequested;
        
        // Music Events
        public static event Action<string> OnMusicRequested;
        public static event Action OnMusicStopRequested;
        
        // Scene Events
        public static event Action<string> OnSceneLoadRequested;
        
        // Loading Events
        public static event Action<string> OnLoadingStartRequested;
        public static event Action OnLoadingEndRequested;
        public static event Action<float> OnLoadingProgressRequested;

        // Counter for tracking requests
        private static readonly float GameStartTime;

        static GameEvents() {
            GameStartTime = Time.time;
        }

        // UI Request Methods
        public static void RequestUIShow(string uiType) {
            Debug.Log($"[GameEvents] UI Show requested: {uiType}");
            OnUIShowRequested?.Invoke(uiType);
        }
        
        public static void RequestUIHide(string uiType) {
            Debug.Log($"[GameEvents] UI Hide requested: {uiType}");
            OnUIHideRequested?.Invoke(uiType);
        }
        
        public static void RequestAllUIHide() {
            Debug.Log("[GameEvents] Hide All UI requested");
            OnAllUIHideRequested?.Invoke();
        }

        // Music Request Methods
        public static void RequestMusic(string trackName) {
            Debug.Log($"[GameEvents] Music requested: {trackName}");
            OnMusicRequested?.Invoke(trackName);
        }
        
        public static void RequestMusicStop() {
            Debug.Log("[GameEvents] Music stop requested");
            OnMusicStopRequested?.Invoke();
        }

        // Scene Request Methods
        public static void RequestSceneLoad(string sceneName) {
            Debug.Log($"[GameEvents] Scene load requested: {sceneName}");
            OnSceneLoadRequested?.Invoke(sceneName);
        }

        // Loading Request Methods
        public static void RequestLoadingStart(string message) {
            OnLoadingStartRequested?.Invoke(message);
        }
        
        public static void RequestLoadingEnd() {
            OnLoadingEndRequested?.Invoke();
        }
        
        public static void RequestLoadingProgress(float progress) {
            // Don't spam progress logs, only log significant changes
            if (Mathf.Approximately(progress, 1f) || progress == 0f || progress % 0.25f < 0.01f) {
                Debug.Log($"[GameEvents] Loading progress: {progress:F2}");
            }
            OnLoadingProgressRequested?.Invoke(progress);
        }
    }
}