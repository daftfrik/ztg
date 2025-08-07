using UnityEngine;
using System.Collections.Generic;

namespace Core.GameState {
    public class UIController : SingletonPersistent<UIController> {
        [Header("UI Panels")] 
        public GameObject mainMenuUI;
        public GameObject townUI;
        public GameObject expeditionUI;
        public GameObject combatUI;
        public GameObject victoryUI;
        public GameObject gameOverUI;

        private Dictionary<string, GameObject> _uiPanels;

        protected override void OnSingletonAwake() {
            // Initialize UI panel dictionary
            _uiPanels = new Dictionary<string, GameObject> {
                { "MainMenu", mainMenuUI },
                { "Town", townUI },
                { "Expedition", expeditionUI },
                { "Combat", combatUI },
                { "Victory", victoryUI },
                { "GameOver", gameOverUI }
            };

            // Subscribe to events
            GameEvents.OnUIShowRequested += ShowUI;
            GameEvents.OnUIHideRequested += HideUI;
            GameEvents.OnAllUIHideRequested += HideAllUI;
        }

        private void OnDestroy() {
            // Unsubscribe from events
            GameEvents.OnUIShowRequested -= ShowUI;
            GameEvents.OnUIHideRequested -= HideUI;
            GameEvents.OnAllUIHideRequested -= HideAllUI;
        }

        private void ShowUI(string uiType) {
            if (_uiPanels.TryGetValue(uiType, out GameObject panel)) {
                panel?.SetActive(true);
                Debug.Log($"[UIController] Showing {uiType} UI");
            } else {
                Debug.LogWarning($"[UIController] UI panel '{uiType}' not found!");
            }
        }

        private void HideUI(string uiType) {
            if (_uiPanels.TryGetValue(uiType, out GameObject panel)) {
                panel?.SetActive(false);
                Debug.Log($"[UIController] Hiding {uiType} UI");
            } else {
                Debug.LogWarning($"[UIController] UI panel '{uiType}' not found!");
            }
        }

        private void HideAllUI() {
            foreach (var panel in _uiPanels.Values) {
                panel?.SetActive(false);
            }
            Debug.Log("[UIController] All UI panels hidden");
        }

        // Keep legacy methods for backwards compatibility (optional)
        public void ShowMainMenu() => ShowUI("MainMenu");
        public void HideMainMenu() => HideUI("MainMenu");
        public void ShowTownUI() => ShowUI("Town");
        public void HideTownUI() => HideUI("Town");
        public void ShowExpeditionUI() => ShowUI("Expedition");
        public void HideExpeditionUI() => HideUI("Expedition");
        public void ShowCombatUI() => ShowUI("Combat");
        public void HideCombatUI() => HideUI("Combat");
        public void ShowVictoryUI() => ShowUI("Victory");
        public void HideVictoryUI() => HideUI("Victory");
        public void ShowGameOverUI() => ShowUI("GameOver");
        public void HideGameOverUI() => HideUI("GameOver");
    }
}