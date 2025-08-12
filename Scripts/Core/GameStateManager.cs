using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Core {
    public class GameStateManager : SingletonPersistent<GameStateManager> {
        private GameStateType _currentState;
        
        public event Action<GameStateType> OnGameStateChanged;
        
        protected override void OnSingletonAwake() {
            // Subscribe to scene load requests
            GameEvents.OnSceneLoadRequested += LoadScene;
        }

        private void OnDestroy() {
            GameEvents.OnSceneLoadRequested -= LoadScene;
        }

        private async Task ChangeState(GameStateType newState) {
            if (newState == _currentState) return;

            Debug.Log($"[GameStateManager] Changing state from {_currentState} to {newState}");
            
            string sceneName = GetSceneNameForState(newState);
            if (!string.IsNullOrEmpty(sceneName)) {
                GameEvents.RequestLoadingStart($"Loading {newState}...");
                await SceneLoader.LoadSceneAsync(sceneName);
                GameEvents.RequestLoadingEnd();
            }

            _currentState = newState;
            OnGameStateChanged?.Invoke(newState);
            
            Debug.Log($"[GameStateManager] Successfully entered {newState}");
        }

        private void LoadScene(string sceneName) {
            var state = GetStateForSceneName(sceneName);
            if (state != GameStateType.None) {
                _ = ChangeState(state); // Fire and forget
            }
        }

        private string GetSceneNameForState(GameStateType state) {
            return state switch {
                GameStateType.MainMenu => "MainMenu",
                GameStateType.Town => "Town",
                GameStateType.Expedition => "Expedition",
                GameStateType.Combat => "Combat",
                _ => null
            };
        }

        private GameStateType GetStateForSceneName(string sceneName) {
            return sceneName switch {
                "MainMenu" => GameStateType.MainMenu,
                "Town" => GameStateType.Town,
                "Expedition" => GameStateType.Expedition,
                "Combat" => GameStateType.Combat,
                _ => GameStateType.None
            };
        }

        public GameStateType GetCurrentState() => _currentState;

        // Simple state change methods
        public void GoToMainMenu() => _ = ChangeState(GameStateType.MainMenu);
        public void GoToTown() => _ = ChangeState(GameStateType.Town);
        public void GoToExpedition() => _ = ChangeState(GameStateType.Expedition);
        public void GoToCombat() => _ = ChangeState(GameStateType.Combat);
    }

    public enum GameStateType {
        None,
        MainMenu,
        Town,
        Expedition,
        Combat,
        Victory,
        GameOver
    }
}