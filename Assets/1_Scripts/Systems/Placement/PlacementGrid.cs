using System.Collections.Generic;
using MeowTruck.Data;
using UnityEngine;

namespace MeowTruck.Systems.Placement
{
	public class PlacementGrid : MonoBehaviour
	{
		public static PlacementGrid Instance { get; private set; }

		[SerializeField] private Vector2 cellSize = Vector2.one;
		[SerializeField] private Vector2 origin;

		private readonly Dictionary<Vector2Int, PlacedItem> occupiedCells = new();

		public Vector2 CellSize => cellSize;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				return;
			}

			if (Instance != this)
				Destroy(gameObject);
		}

		private void OnDestroy()
		{
			if (Instance == this)
				Instance = null;
		}

		public Vector2Int WorldToCell(Vector3 worldPosition)
		{
			Vector2 local = (Vector2)worldPosition - origin;
			return new Vector2Int(
				Mathf.FloorToInt(local.x / cellSize.x),
				Mathf.FloorToInt(local.y / cellSize.y));
		}

		public Vector3 CellToWorldCenter(Vector2Int cell)
		{
			return new Vector3(
				origin.x + (cell.x + 0.5f) * cellSize.x,
				origin.y + (cell.y + 0.5f) * cellSize.y,
				0f);
		}

		public bool IsOccupied(Vector2Int cell)
		{
			return occupiedCells.ContainsKey(cell);
		}

		public bool CanPlace(Vector2Int originCell, PlacementDefinition placement)
		{
			if (placement == null || placement.PlacedPrefab == null)
				return false;

			foreach (Vector2Int offset in placement.Footprint)
			{
				if (occupiedCells.ContainsKey(originCell + offset))
					return false;
			}

			return true;
		}

		public void Register(Vector2Int originCell, PlacementDefinition placement, PlacedItem placedItem)
		{
			foreach (Vector2Int offset in placement.Footprint)
				occupiedCells[originCell + offset] = placedItem;
		}

		public void Unregister(PlacedItem placedItem)
		{
			if (placedItem == null || placedItem.Footprint == null)
				return;

			foreach (Vector2Int offset in placedItem.Footprint)
				occupiedCells.Remove(placedItem.OriginCell + offset);
		}
	}
}
