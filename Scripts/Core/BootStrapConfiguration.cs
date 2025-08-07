using UnityEngine;

namespace Core {
    [CreateAssetMenu(fileName = "BootstrapConfig", menuName = "Core/Bootstrap Configuration")]
    public class BootstrapConfiguration : ScriptableObject {
        [Header("Scene Management")] public string initialSceneName = "MainMenu";
        public float sceneTransitionDelay = 0.1f;

        [Header("Manager Prefabs")] public GameObject[] managerPrefabs;

        [Header("Loading")] public bool showLoadingScreen = true;
        public float minLoadingTime = 1f;

        [Header("Debug")] public bool enableDebugLogs = true;
    }
}