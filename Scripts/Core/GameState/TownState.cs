using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.GameState {
    public class TownState : IGameState {
        public async Task Enter() {
            if (SceneManager.GetActiveScene().name != "Town") {
                GameEvents.RequestLoadingStart("Loading Town...");
                await SceneLoader.LoadSceneAsync("Town");
                GameEvents.RequestLoadingEnd();
            }

            GameEvents.RequestUIShow("Town");
            GameEvents.RequestMusic("TownTheme");
            
            Debug.Log("Entered Town State");
            await Task.CompletedTask;
        }

        public async Task Exit() {
            GameEvents.RequestUIHide("Town");
            await Task.CompletedTask;
        }
    }
}