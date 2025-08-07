using System.Threading.Tasks;

namespace Core.GameState {
    public class VictoryState : IGameState {
        public async Task Enter() {
            GameEvents.RequestUIShow("Victory");
            GameEvents.RequestMusic("VictoryTheme");
            await Task.CompletedTask;
        }

        public async Task Exit() {
            GameEvents.RequestUIHide("Victory");
            await Task.CompletedTask;
        }
    }
}