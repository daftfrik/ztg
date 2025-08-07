using System.Threading.Tasks;

namespace Core.GameState {
    public interface IGameState {
        Task Enter();
        Task Exit();
    }
}