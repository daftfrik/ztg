using UnityEngine;

namespace Core {
    public class SceneController : MonoBehaviour {
        [Header("Scene Settings")]
        [SerializeField] private GameStateType sceneType;
        [SerializeField] private string musicTrack;
        
        [Header("UI Panels")]
        [SerializeField] private UIPanel[] uiPanels;
        
        [Header("Settings")]
        [SerializeField] private string defaultPanel = "MainMenu";
        [SerializeField] private float showDelay = 0.1f;

        [System.Serializable]
        public class UIPanel {
            public string name;
            public GameObject panel;
        }

        private System.Collections.Generic.Dictionary<string, GameObject> _panelDict;

        private void Start() {
            InitializePanels();
            UpdateGameState();
            PlayMusic();
            Invoke(nameof(ShowDefaultPanel), showDelay);
        }

        private void InitializePanels() {
            _panelDict = new System.Collections.Generic.Dictionary<string, GameObject>();
            
            foreach (var uiPanel in uiPanels) {
                if (uiPanel.panel != null && !string.IsNullOrEmpty(uiPanel.name)) {
                    _panelDict[uiPanel.name] = uiPanel.panel;
                    uiPanel.panel.SetActive(false); // Hide initially
                }
            }
        }

        private void UpdateGameState() {
            // Update the game state manager
            if (GameStateManager.Instance != null) {
                var currentState = GameStateManager.Instance.GetCurrentState();
                if (currentState != sceneType) {
                    // Update internal state without loading scene again
                    typeof(GameStateManager).GetField("_currentState", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.SetValue(GameStateManager.Instance, sceneType);
                }
            }
        }

        private void PlayMusic() {
            // Play music if specified
            if (!string.IsNullOrEmpty(musicTrack)) {
                GameEvents.RequestMusic(musicTrack);
            }
        }

        private void ShowDefaultPanel() {
            ShowPanel(defaultPanel);
        }

        // Panel management methods
        private void ShowPanel(string panelName) {
            if (!_panelDict.ContainsKey(panelName)) {
                Debug.LogWarning($"[SceneController] Panel '{panelName}' not found!");
                return;
            }

            // Hide all panels first
            foreach (var panel in _panelDict.Values) {
                panel?.SetActive(false);
            }

            // Show the requested panel
            _panelDict[panelName].SetActive(true);
            Debug.Log($"[SceneController] Showing panel: {panelName}");
        }

        // UI Panel Methods (for buttons)
        public void ShowMainMenu() => ShowPanel("MainMenu");
        public void ShowSettings() => ShowPanel("Settings");
        public void ShowCredits() => ShowPanel("Credits");
        public void GoBack() => ShowPanel(defaultPanel);

        // Public methods for manual control
        public void ShowAllUI() {
            foreach (var panel in _panelDict.Values) {
                panel?.SetActive(true);
            }
        }

        public void HideAllUI() {
            foreach (var panel in _panelDict.Values) {
                panel?.SetActive(false);
            }
        }

        // Convenience methods for buttons
        public void GoToMainMenu() => GameStateManager.Instance?.GoToMainMenu();
        public void GoToTown() => GameStateManager.Instance?.GoToTown();
        public void GoToExpedition() => GameStateManager.Instance?.GoToExpedition();
        public void GoToCombat() => GameStateManager.Instance?.GoToCombat();
        public void QuitGame() => Application.Quit();
    }
}