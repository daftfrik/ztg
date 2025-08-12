using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Core {
    public class LoadingScreenManager : SingletonPersistent<LoadingScreenManager> {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private float fadeTime = 0.3f;
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private UnityEngine.UI.Slider progressBar;

        private CanvasGroup _canvasGroup;
        private bool _isShowing;
        private bool _isTransitioning;

        protected override void OnSingletonAwake() {
            _canvasGroup = loadingScreen?.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = loadingScreen?.AddComponent<CanvasGroup>();

            // Subscribe to events
            GameEvents.OnLoadingStartRequested += ShowWithMessage;
            GameEvents.OnLoadingEndRequested += HideAsync;
            GameEvents.OnLoadingProgressRequested += UpdateProgress;

            // Initially hidden
            if (loadingScreen == null) return;
            loadingScreen.SetActive(false);
            if (_canvasGroup != null) _canvasGroup.alpha = 0f;
        }

        private void OnDestroy() {
            // Unsubscribe from events
            GameEvents.OnLoadingStartRequested -= ShowWithMessage;
            GameEvents.OnLoadingEndRequested -= HideAsync;
            GameEvents.OnLoadingProgressRequested -= UpdateProgress;
        }

        private async Task ShowAsync() {
            if (_isShowing || _isTransitioning) {
                Debug.Log("[LoadingScreenManager] Already showing or transitioning, ignoring show request");
                return;
            }

            _isTransitioning = true;
            _isShowing = true;

            // Show INSTANTLY - no fade in animation
            loadingScreen?.SetActive(true);
            if (_canvasGroup != null) {
                _canvasGroup.alpha = 1f; // Instant show
            }

            _isTransitioning = false;
            //Debug.Log("[LoadingScreenManager] Loading screen shown INSTANTLY");
            await Task.CompletedTask; // Complete immediately
        }

        private async void ShowWithMessage(string message) {
            if (loadingText != null) {
                loadingText.text = message;
            }
            await ShowAsync();
        }

        private async void HideAsync() {
            await HideAsyncTask();
        }

        private async Task HideAsyncTask() {
            if (!_isShowing || _isTransitioning) {
                Debug.Log("[LoadingScreenManager] Not showing or already transitioning, ignoring hide request");
                return;
            }

            _isTransitioning = true;

            // Fade out smoothly (keep this animation)
            if (_canvasGroup != null)
                await FadeOut();
                
            loadingScreen?.SetActive(false);
            _isShowing = false;
            _isTransitioning = false;
        }

        private void UpdateProgress(float progress) {
            if (progressBar != null) {
                progressBar.value = progress;
            }
            if (loadingText != null && loadingText.text.Contains("Loading")) {
                loadingText.text = $"Loading... {progress:P0}";
            }
        }

        private async Task FadeOut() {
            await Fade(1f, 0f);
        }

        private async Task Fade(float from, float to) {
            float elapsed = 0f;
            while (elapsed < fadeTime) {
                elapsed += Time.deltaTime;
                if (_canvasGroup != null) _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeTime);
                await Task.Yield();
            }
            if (_canvasGroup != null) _canvasGroup.alpha = to;
        }
    }
}