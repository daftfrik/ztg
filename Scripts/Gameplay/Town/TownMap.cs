using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Town {
    public enum BuildingType {
        Home,
        Tavern,
        Armory,
        Training,
        Lab,
        Ingest,
        Social
    }

    [CreateAssetMenu(fileName = "Town Map", menuName = "Town/Town Map")]
    public class TownMap : ScriptableObject {
        [SerializeField] private List<Location> buildings = new();
        [SerializeField] private List<Location> socialSpots = new();

        public Location FindBuilding(BuildingType buildingType) {
            return buildings.FirstOrDefault(b => b.BuildingType == buildingType);
        }

        public Location FindSocialSpot(string spotName) {
            return socialSpots.FirstOrDefault(s => s.CustomName == spotName);
        }

        public List<Location> GetBuildingsOfType(BuildingType buildingType) {
            return buildings.Where(b => b.BuildingType == buildingType).ToList();
        }

        public IReadOnlyList<Location> AllBuildings => buildings;
        public IReadOnlyList<Location> AllSocialSpots => socialSpots;
    }

    [System.Serializable]
    public class Location {
        [SerializeField] private BuildingType buildingType;
        [SerializeField] private Vector2 position;

        [Tooltip("Optional custom name override. If empty, will use BuildingType name.")] [SerializeField]
        private string customName;

        public BuildingType BuildingType => buildingType;
        public Vector2 Position => position;
        public string CustomName => customName;
        public string Name => string.IsNullOrEmpty(customName) ? buildingType.ToString() : customName;
    }
}