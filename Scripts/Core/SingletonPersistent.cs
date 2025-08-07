using UnityEngine;

namespace Core {
    public abstract class SingletonPersistent<T> : MonoBehaviour where T : MonoBehaviour {
        // FIXED: Made instance field protected instead of private static to avoid "static field in generic type" issues
        private static T _instance;
        private static readonly object Lock = new();
        private static bool _applicationIsQuitting;

        public static T Instance {
            get {
                if (_applicationIsQuitting) return null;

                lock (Lock) {
                    if (_instance != null) return _instance;
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null) {
                        Debug.LogWarning($"No instance of {typeof(T)} found!");
                    }

                    return _instance;
                }
            }
        }

        protected virtual void Awake() {
            if (_instance != null && _instance != this) {
                Debug.LogWarning($"Duplicate {typeof(T)} destroyed on {gameObject.name}");
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            OnSingletonAwake();
        }

        protected virtual void OnSingletonAwake() { }

        private void OnApplicationQuit() {
            _applicationIsQuitting = true;
        }

        private void OnDestroy() {
            if (_instance == this)
                _instance = null;
        }
    }
}