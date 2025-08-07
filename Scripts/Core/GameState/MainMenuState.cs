using System.Threading.Tasks;

namespace Core.GameState {
    public class MainMenuState : IGameState {
        public async Task Enter() {
            GameEvents.RequestUIShow("MainMenu");
            GameEvents.RequestMusic("MainMenuTheme");
            await Task.CompletedTask;
        }

        public async Task Exit() {
            GameEvents.RequestUIHide("MainMenu");
            await Task.CompletedTask;
        }
    }
}