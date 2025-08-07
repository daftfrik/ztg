using Core.GameState;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class TownUI : MonoBehaviour {
        public Button startExpeditionButton;

        private void Start() {
            startExpeditionButton.onClick.AddListener(OnStartExpeditionClicked);
        }

        private void OnStartExpeditionClicked() {
            _ = GameStateManager.Instance.ChangeState(GameStateType.Expedition);
        }
    }
}