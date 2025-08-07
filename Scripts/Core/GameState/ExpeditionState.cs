using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Core.GameState {
    public class ExpeditionState : IGameState {
        public async Task Enter() {
            if (SceneManager.GetActiveScene().name != "Expedition") {
                GameEvents.RequestLoadingStart("Preparing Expedition...");
                await SceneLoader.LoadSceneAsync("Expedition");
                GameEvents.RequestLoadingEnd();
            }

            GameEvents.RequestUIShow("Expedition");
            GameEvents.RequestMusic("ExpeditionTheme");
            await Task.CompletedTask;
        }

        public async Task Exit() {
            GameEvents.RequestUIHide("Expedition");
            await Task.CompletedTask;
        }
    }
}