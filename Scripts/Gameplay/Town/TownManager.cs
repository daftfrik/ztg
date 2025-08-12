using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Town {
    public enum DailyEventType {
        WakeUp,
        Sleep,
        Work,
        Breakfast,
        Lunch,
        Dinner,
        Socialize,
        FreeTime
    }

    public class TownManager : MonoBehaviour {
        [Header("Time Settings")] [SerializeField]
        private float dayDurationInMinutes = 24f;

        [SerializeField] private float startHour = 6f;

        [Header("Event Schedule")] [SerializeField]
        private List<DailyEvent> dailyEvents = new();

        [Header("Debug")] [SerializeField] private bool showDebugLogs = true;
        [SerializeField] private bool pauseTime;

        // Events
        [field: NonSerialized] public static event Action<DailyEventType, float> OnDailyEvent;
        [field: NonSerialized] public static event Action<float> OnTimeChanged;
        [field: NonSerialized] public static event Action OnNewDay;

        // State
        private float CurrentHour { get; set; }
        private int CurrentDay { get; set; }
        private string CurrentTimeString => FormatTime(CurrentHour);
        private DailyEventType CurrentEventType { get; set; } = DailyEventType.FreeTime;
        private string CurrentEventName => CurrentEventType.ToString();

        private float _timeSpeed;
        private DailyEventType _lastEventType = DailyEventType.FreeTime;

        [Serializable]
        public class DailyEvent {
            public DailyEventType eventType;
            [Range(0, 24)] public float startHour;
            [Range(0.1f, 12f)] public float durationHours = 1f;

            [TextArea(2, 4)] public string description;

            public float EndHour => (startHour + durationHours) % 24f;
            public string EventName => eventType.ToString();
        }

        private void Awake() {
            if (FindObjectsByType<TownManager>(FindObjectsSortMode.None).Length > 1) {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            InitializeDefaultSchedule();
        }

        private void Start() {
            CurrentHour = startHour;
            CurrentDay = 1;
            _timeSpeed = 24f / (dayDurationInMinutes * 60f); // hours per real second

            if (showDebugLogs)
                Debug.Log(
                    $"TownManager started. Day duration: {dayDurationInMinutes} min. Time speed: {_timeSpeed:F4} h/s");
        }

        private void Update() {
            if (pauseTime) return;

            UpdateTime();
            CheckForEvents();
        }

        private void UpdateTime() {
            CurrentHour += _timeSpeed * Time.deltaTime;

            if (CurrentHour >= 24f) {
                CurrentHour -= 24f;
                CurrentDay++;
                _lastEventType = DailyEventType.FreeTime;
                OnNewDay?.Invoke();

                if (showDebugLogs) Debug.Log($"New day started: Day {CurrentDay}");
            }

            OnTimeChanged?.Invoke(CurrentHour);
        }

        private void CheckForEvents() {
            var currentEvent = dailyEvents.FirstOrDefault(e =>
                IsTimeInRange(CurrentHour, e.startHour, e.EndHour));

            var activeType = currentEvent?.eventType ?? DailyEventType.FreeTime;

            if (activeType == _lastEventType) return;

            CurrentEventType = activeType;
            _lastEventType = activeType;
            OnDailyEvent?.Invoke(activeType, CurrentHour);

            if (showDebugLogs) Debug.Log($"[{CurrentTimeString}] Event: {CurrentEventName}");
        }

        private bool IsTimeInRange(float currentTime, float startTime, float endTime) {
            return startTime <= endTime
                ? currentTime >= startTime && currentTime < endTime
                : currentTime >= startTime || currentTime < endTime;
        }

        private string FormatTime(float hour) {
            int h = Mathf.FloorToInt(hour);
            int m = Mathf.FloorToInt((hour - h) * 60);
            string period = h < 12 ? "AM" : "PM";

            int displayHour = h % 12;
            if (displayHour == 0) displayHour = 12;

            return $"{displayHour:00}:{m:00} {period}";
        }

        private void InitializeDefaultSchedule() {
            if (dailyEvents.Count > 0) return;

            dailyEvents = new List<DailyEvent> {
                new() {
                    eventType = DailyEventType.WakeUp, startHour = 6f,
                    durationHours = 0.5f, description = "NPCs wake up and start their day"
                },
                new() {
                    eventType = DailyEventType.Breakfast, startHour = 6.5f,
                    durationHours = 1f, description = "Morning meal time"
                },
                new() {
                    eventType = DailyEventType.Work, startHour = 8f,
                    durationHours = 4f, description = "Morning work shift"
                },
                new() {
                    eventType = DailyEventType.Lunch, startHour = 12f,
                    durationHours = 1f, description = "Lunch break"
                },
                new() {
                    eventType = DailyEventType.Work, startHour = 13f,
                    durationHours = 4f, description = "Afternoon work shift"
                },
                new() {
                    eventType = DailyEventType.Dinner, startHour = 18f,
                    durationHours = 1.5f, description = "Evening meal time"
                },
                new() {
                    eventType = DailyEventType.Socialize, startHour = 19.5f,
                    durationHours = 2.5f, description = "Leisure and social time"
                },
                new() {
                    eventType = DailyEventType.Sleep, startHour = 22f,
                    durationHours = 8f, description = "Sleep until morning"
                }
            };

            dailyEvents.Sort((a, b) => a.startHour.CompareTo(b.startHour));
        }

        public void SetTime(float hour) {
            CurrentHour = Mathf.Clamp(hour, 0f, 24f);
            _lastEventType = DailyEventType.FreeTime;
        }

        public void AddEvent(DailyEventType type, float start, float duration, string description = "") {
            if (dailyEvents.Any(e => e.eventType == type)) {
                Debug.LogWarning($"Event type {type} already exists.");
                return;
            }

            dailyEvents.Add(new DailyEvent {
                eventType = type,
                startHour = start,
                durationHours = duration,
                description = description
            });

            dailyEvents.Sort((a, b) => a.startHour.CompareTo(b.startHour));
        }

        public void RemoveEvent(DailyEventType type) =>
            dailyEvents.RemoveAll(e => e.eventType == type);

        public DailyEvent GetCurrentEvent() =>
            dailyEvents.FirstOrDefault(e => IsTimeInRange(CurrentHour, e.startHour, e.EndHour));

        public IReadOnlyList<DailyEvent> GetTodaysSchedule() => dailyEvents;

        public void PauseTime() => pauseTime = true;
        public void ResumeTime() => pauseTime = false;
        public void TogglePause() => pauseTime = !pauseTime;

        private void OnGUI() {
            if (!showDebugLogs) return;

            const int width = 220;
            const int height = 130;

            GUI.Box(new Rect(10, 10, width, height), "Town Manager");
            GUI.Label(new Rect(20, 30, width, 20), $"Time: {CurrentTimeString}");
            GUI.Label(new Rect(20, 50, width, 20), $"Day: {CurrentDay}");
            GUI.Label(new Rect(20, 70, width, 20), $"Event: {CurrentEventName}");

            if (GUI.Button(new Rect(20, 100, 80, 20), pauseTime ? "Resume" : "Pause"))
                TogglePause();
        }
    }
}