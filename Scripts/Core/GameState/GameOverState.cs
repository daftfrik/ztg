using System.Threading.Tasks;

namespace Core.GameState {
    public class GameOverState : IGameState {
        public async Task Enter() {
            GameEvents.RequestUIShow("GameOver");
            GameEvents.RequestMusic("GameOverTheme");
            await Task.CompletedTask;
        }

        public async Task Exit() {
            GameEvents.RequestUIHide("GameOver");
            await Task.CompletedTask;
        }
    }
}