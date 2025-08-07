using UnityEngine;

namespace Core {
    public class MusicManager : SingletonPersistent<MusicManager> {
        [Header("Audio Source")] 
        public AudioSource musicSource;

        [Header("Music Tracks")] 
        public AudioClip mainMenuTheme;
        public AudioClip townTheme;
        public AudioClip expeditionTheme;
        public AudioClip combatTheme;
        public AudioClip victoryTheme;
        public AudioClip gameOverTheme;

        protected override void OnSingletonAwake() {
            // Subscribe to events
            GameEvents.OnMusicRequested += PlayTrack;
            GameEvents.OnMusicStopRequested += StopMusic;
        }

        private void OnDestroy() {
            // Unsubscribe from events
            GameEvents.OnMusicRequested -= PlayTrack;
            GameEvents.OnMusicStopRequested -= StopMusic;
        }

        private void PlayTrack(string trackName) {
            var clip = GetClipByName(trackName);
            if (clip == null || musicSource.clip == clip) return;
            
            musicSource.clip = clip;
            musicSource.Play();
            Debug.Log($"[MusicManager] Playing track: {trackName}");
        }

        private void StopMusic() {
            musicSource.Stop();
            Debug.Log("[MusicManager] Music stopped");
        }

        private AudioClip GetClipByName(string clipName) {
            return clipName switch {
                "MainMenuTheme" => mainMenuTheme,
                "TownTheme" => townTheme,
                "ExpeditionTheme" => expeditionTheme,
                "CombatTheme" => combatTheme,
                "VictoryTheme" => victoryTheme,
                "GameOverTheme" => gameOverTheme,
                _ => null
            };
        }

        // Keep legacy method for backwards compatibility (optional)
        public void PlayTrack(string trackName, bool legacy) => PlayTrack(trackName);
    }
}