using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.GameState {
    public class GameStateManager : SingletonPersistent<GameStateManager> {
        private IGameState _currentStateLogic;
        private GameStateType _currentStateType;
        private Dictionary<GameStateType, IGameState> _stateInstances;
        private bool _isBootstrapping = true;

        protected override void OnSingletonAwake() {
            _stateInstances = new Dictionary<GameStateType, IGameState> {
                { GameStateType.MainMenu, new MainMenuState() },
                { GameStateType.Town, new TownState() },
                { GameStateType.Expedition, new ExpeditionState() },
                { GameStateType.Combat, new CombatState() },
                { GameStateType.Victory, new VictoryState() },
                { GameStateType.GameOver, new GameOverState() }
            };
        }

        private async void Start() {
            // Wait for a few frames to ensure all bootstrap processes are completed
            await Task.Delay(100);
            
            // Now safely transition to main menu without showing loading
            await ChangeStateInternal(GameStateType.MainMenu, showLoading: false);
            _isBootstrapping = false;
        }

        public event Action<GameStateType> OnGameStateChanged;

        public async Task ChangeState(GameStateType newState, bool showLoading = true) {
            Debug.Log($"[GameStateManager] ChangeState requested: {newState}, showLoading: {showLoading}, isBootstrapping: {_isBootstrapping}");
            
            // If we're still bootstrapping, queue the state change
            if (_isBootstrapping) {
                Debug.LogWarning("[GameStateManager] Attempted state change during bootstrap, ignoring");
                return;
            }
            
            await ChangeStateInternal(newState, showLoading);
        }

        private async Task ChangeStateInternal(GameStateType newState, bool showLoading = true) {
            if (newState == _currentStateType) return;

            if (_currentStateLogic != null) {
                Debug.Log($"[GameStateManager] Exiting current state: {_currentStateType}");
                await _currentStateLogic.Exit();
            }

            _currentStateType = newState;
            _currentStateLogic = _stateInstances[newState];

            // Only show loading screen if requested AND not during bootstrapping
            if (showLoading && !_isBootstrapping) {
                Debug.Log($"[GameStateManager] REQUESTING LOADING SCREEN for state {newState}");
                GameEvents.RequestLoadingStart($"Loading {newState}...");
            } else {
                Debug.Log($"[GameStateManager] NOT requesting loading screen (showLoading: {showLoading}, isBootstrapping: {_isBootstrapping})");
            }

            Debug.Log($"[GameStateManager] Entering state: {newState}");
            await _currentStateLogic.Enter();

            // Only end loading if we started it in this method
            if (showLoading && !_isBootstrapping) {
                Debug.Log($"[GameStateManager] ENDING LOADING SCREEN for state {newState}");
                GameEvents.RequestLoadingEnd();
            }

            OnGameStateChanged?.Invoke(newState);
            Debug.Log($"[GameStateManager] Successfully entered {newState} (Bootstrap: {_isBootstrapping})");
        }

        // Keep original method for backwards compatibility
        public async Task ChangeState(GameStateType newState) {
            await ChangeState(newState, showLoading: true);
        }

        public GameStateType GetCurrentState() {
            return _currentStateType;
        }
    }

    public enum GameStateType {
        MainMenu,
        Town,
        Expedition,
        Combat,
        Victory,
        GameOver
    }
}