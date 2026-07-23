using UnityEngine;

namespace MeowTruck.Data
{
	[CreateAssetMenu(fileName = "Placement Definition", menuName = "SO/Placement Definition")]
	public class PlacementDefinition : ScriptableObject
	{
		[SerializeField] private GameObject placedPrefab;
		[SerializeField] private Vector2Int[] footprint = { Vector2Int.zero };

		public GameObject PlacedPrefab => placedPrefab;
		public Vector2Int[] Footprint => footprint;
	}
}
