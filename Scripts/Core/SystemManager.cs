using UnityEngine;
using UnityEngine.Events;

namespace Core {
    [CreateAssetMenu(fileName = "SystemManager", menuName = "Core/System Manager")]
    public class SystemManager : ScriptableObject {
        [Header("Events")] [SerializeField] private UnityEvent onSystemsInitialized;
        [SerializeField] private UnityEvent onBootstrapComplete;

        // Public accessors for the events
        public UnityEvent OnSystemsInitialized => onSystemsInitialized;
        public UnityEvent OnBootstrapComplete => onBootstrapComplete;

        public void NotifySystemsInitialized() {
            onSystemsInitialized?.Invoke();
            Debug.Log("All systems initialized successfully!");
        }

        public void NotifyBootstrapComplete() {
            onBootstrapComplete?.Invoke();
            Debug.Log("Bootstrap sequence completed!");
        }
    }
}