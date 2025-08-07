using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Core.GameState {
    public class CombatState : IGameState {
        public async Task Enter() {
            if (SceneManager.GetActiveScene().name != "Combat") {
                GameEvents.RequestLoadingStart("Entering Combat...");
                await SceneLoader.LoadSceneAsync("Combat");
                GameEvents.RequestLoadingEnd();
            }

            GameEvents.RequestUIShow("Combat");
            GameEvents.RequestMusic("CombatTheme");
            await Task.CompletedTask;
        }

        public async Task Exit() {
            GameEvents.RequestUIHide("Combat");
            await Task.CompletedTask;
        }
    }
}