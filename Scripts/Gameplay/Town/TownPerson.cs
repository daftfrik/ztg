using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Town {
    [RequireComponent(typeof(NavMeshAgent))]
    public class TownPerson : MonoBehaviour {
        [Header("NPC Setup")]
        [SerializeField] private string npcName = "Villager";
        [SerializeField] private TownMap townMap;
        
        [Header("Location Settings")]
        [SerializeField] private BuildingType homeBuilding = BuildingType.Home;
        [SerializeField] private BuildingType workBuilding = BuildingType.Training;
        [SerializeField] private BuildingType eatBuilding = BuildingType.Tavern;
        
        [Header("Settings")]
        [SerializeField] private float movementSpeed = 3.5f;
        [SerializeField] private bool showDebugLogs = true;
        
        private NavMeshAgent _agent;
        private DailyEventType _currentActivity = DailyEventType.FreeTime;
        private Coroutine _currentBehavior;
        
        private void Start() {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = movementSpeed;
            MoveToBuilding(homeBuilding);
        }
        
        private void OnEnable() {
            TownManager.OnDailyEvent += HandleDailyEvent;
        }
        
        private void OnDisable() {
            TownManager.OnDailyEvent -= HandleDailyEvent;
        }
        
        private void HandleDailyEvent(DailyEventType eventType, float currentTime) {
            if (_currentBehavior != null) StopCoroutine(_currentBehavior);
            _currentActivity = eventType;
            
            if (showDebugLogs) Debug.Log($"{npcName}: {eventType}");

            _currentBehavior = eventType switch {
                DailyEventType.WakeUp => StartCoroutine(GoSocialize()),
                DailyEventType.Sleep => StartCoroutine(GoHome()),
                DailyEventType.Work => StartCoroutine(GoToWork()),
                DailyEventType.Breakfast 
                    or DailyEventType.Lunch 
                    or DailyEventType.Dinner => StartCoroutine(GoEat()),
                DailyEventType.Socialize => StartCoroutine(GoSocialize()),
                DailyEventType.FreeTime => StartCoroutine(Wander()),
                _ => _currentBehavior
            };
        }
        
        private IEnumerator GoHome() {
            yield return StartCoroutine(MoveToBuildingCoroutine(homeBuilding));
        }
        
        private IEnumerator GoToWork() {
            yield return StartCoroutine(MoveToBuildingCoroutine(workBuilding));
        }
        
        private IEnumerator GoEat() {
            yield return StartCoroutine(MoveToBuildingCoroutine(eatBuilding));
        }
        
        private IEnumerator GoSocialize() {
            var socialSpots = townMap.AllSocialSpots;
            if (socialSpots.Count > 0) {
                string randomSocialLocation = socialSpots[Random.Range(0, socialSpots.Count)].Name;
                yield return StartCoroutine(MoveToSocialSpotCoroutine(randomSocialLocation));
            } else {
                yield return StartCoroutine(MoveToBuildingCoroutine(homeBuilding));
            }
        }
        
        private IEnumerator Wander() {
            var socialSpots = townMap.AllSocialSpots;
            if (socialSpots.Count > 0) {
                string randomLocation = socialSpots[Random.Range(0, socialSpots.Count)].Name;
                yield return StartCoroutine(MoveToSocialSpotCoroutine(randomLocation));
            } else {
                yield return StartCoroutine(MoveToBuildingCoroutine(homeBuilding));
            }
        }
        
        private IEnumerator MoveToBuildingCoroutine(BuildingType buildingType) {
            MoveToBuilding(buildingType);
            
            // Wait until we reach the destination
            while (_agent.pathPending || _agent.remainingDistance > 0.5f) yield return null;
            
            // Wait a bit at the destination
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }

        private IEnumerator MoveToSocialSpotCoroutine(string locationName) {
            MoveToSocialSpot(locationName);
            
            // Wait until we reach the destination
            while (_agent.pathPending || _agent.remainingDistance > 0.5f) yield return null;
            
            // Wait a bit at the destination
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
        
        private void MoveToBuilding(BuildingType buildingType) {
            if (townMap == null) {
                if (showDebugLogs) Debug.LogWarning($"{npcName}: TownMap is not assigned!");
                return;
            }
            
            Location target = townMap.FindBuilding(buildingType);

            if (target == null) {
                if (showDebugLogs) Debug.LogWarning($"{npcName}: Building '{buildingType}' not found in TownMap!");
                return;
            }
            
            Vector3 destination = new Vector3(target.Position.x, transform.position.y, target.Position.y);
            _agent.SetDestination(destination);
            
            if (showDebugLogs) Debug.Log($"{npcName}: Moving to {buildingType} at {destination}");
        }

        private void MoveToSocialSpot(string locationName) {
            if (townMap == null) {
                if (showDebugLogs) Debug.LogWarning($"{npcName}: TownMap is not assigned!");
                return;
            }
            
            Location targetLocation = townMap.FindSocialSpot(locationName);

            if (targetLocation == null) {
                if (showDebugLogs) Debug.LogWarning($"{npcName}: Social spot '{locationName}' not found in TownMap!");
                return;
            }
            
            Vector3 destination = new Vector3(targetLocation.Position.x, transform.position.y, targetLocation.Position.y);
            _agent.SetDestination(destination);
            
            if (showDebugLogs) Debug.Log($"{npcName}: Moving to social spot {locationName} at {destination}");
        }
        
        private void OnGUI() {
            if (!showDebugLogs) return;

            if (Camera.main == null) return;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
            if (screenPos.z > 0) {
                GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y, 100, 40), 
                    $"{npcName}\n{_currentActivity}");
            }
        }
    }
}