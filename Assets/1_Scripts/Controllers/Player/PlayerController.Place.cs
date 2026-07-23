using MeowTruck.Data;
using MeowTruck.Environments;
using MeowTruck.Manager;
using MeowTruck.Systems.Placement;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Controllers
{
	public partial class PlayerController
	{
		public void RequestPlaceItem(int itemId, Vector2Int originCell)
		{
			if (!IsOwner)
				return;

			RequestPlaceItemServerRpc(itemId, originCell.x, originCell.y);
		}

		[Rpc(SendTo.Server)]
		private void RequestPlaceItemServerRpc(int itemId, int cellX, int cellY)
		{
			if (NetworkDay.Instance != null && NetworkDay.Instance.IsRunning.Value)
				return;

			if (PlacementGrid.Instance == null)
				return;

			ItemData itemData = Managers.Resource.GetItemData(itemId);
			if (itemData == null || !itemData.IsPlaceable)
				return;

			Vector2Int originCell = new(cellX, cellY);
			if (!PlacementGrid.Instance.CanPlace(originCell, itemData.Placement))
				return;

			GameObject placedObject = Instantiate(
				itemData.Placement.PlacedPrefab,
				PlacementGrid.Instance.CellToWorldCenter(originCell),
				Quaternion.identity);

			if (!placedObject.TryGetComponent(out NetworkObject networkObject))
			{
				Debug.LogError("[FoodTruckPlacement] Placed prefab must have a NetworkObject.");
				Destroy(placedObject);
				return;
			}

			PlacedItem placedItem = placedObject.GetComponent<PlacedItem>();
			if (placedItem == null)
				placedItem = placedObject.AddComponent<PlacedItem>();

			placedItem.Init(itemId, originCell, itemData.Placement.Footprint);
			PlacementGrid.Instance.Register(originCell, itemData.Placement, placedItem);

			networkObject.Spawn();
			RegisterPlacedItemClientRpc(itemId, cellX, cellY, networkObject);
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void RegisterPlacedItemClientRpc(int itemId, int cellX, int cellY, NetworkObjectReference placedObjectReference)
		{
			if (PlacementGrid.Instance == null)
				return;

			if (!placedObjectReference.TryGet(out NetworkObject networkObject))
				return;

			ItemData itemData = Managers.Resource.GetItemData(itemId);
			if (itemData == null || !itemData.IsPlaceable)
				return;

			PlacedItem placedItem = networkObject.GetComponent<PlacedItem>();
			if (placedItem == null)
				placedItem = networkObject.gameObject.AddComponent<PlacedItem>();

			Vector2Int originCell = new(cellX, cellY);
			placedItem.Init(itemId, originCell, itemData.Placement.Footprint);
			PlacementGrid.Instance.Register(originCell, itemData.Placement, placedItem);
		}
	}
}
