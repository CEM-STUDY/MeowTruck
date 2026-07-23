using MeowTruck.Controllers;
using MeowTruck.Data;
using MeowTruck.Environments;
using MeowTruck.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MeowTruck.Systems.Placement
{
	[RequireComponent(typeof(PlacementGrid))]
	public class FoodTruckPlacementSystem : MonoBehaviour
	{
		private static readonly Vector2Int[] DirectionOffsets =
		{
			Vector2Int.up,
			Vector2Int.down,
			Vector2Int.left,
			Vector2Int.right,
			new Vector2Int(1, 1),
			new Vector2Int(1, -1),
			new Vector2Int(-1, 1),
			new Vector2Int(-1, -1)
		};

		[SerializeField] private Color hoverValidColor = new(0.2f, 1f, 0.35f, 0.5f);
		[SerializeField] private Color hoverInvalidColor = new(1f, 0.15f, 0.1f, 0.55f);
		[SerializeField] private Color heldInvalidColor = new(1f, 0.25f, 0.25f, 1f);
		[SerializeField] private int highlightSortingOrder = 50;

		private SpriteRenderer[] highlights;
		private Sprite highlightSprite;
		private PlacementGrid placementGrid;
		private PlayerController localPlayer;
		private Camera cachedCamera;
		private Vector2Int lastPlayerCell;
		private Vector2Int lastMouseCell;
		private Vector2Int activeOriginCell;
		private int lastSelectedItemId = int.MinValue;
		private bool hasLastPlayerCell;
		private bool hasLastMouseCell;
		private bool hasActiveOriginCell;

		private void Awake()
		{
			placementGrid = GetComponent<PlacementGrid>();
			cachedCamera = Camera.main;
			CreateHighlights();
			HideHighlights();
		}

		private void Update()
		{
			if (!CanPreviewPlacement())
			{
				ClearPreview();
				return;
			}

			if (!TryGetHeldPlaceable(out ItemData itemData))
			{
				ClearPreview();
				return;
			}

			Vector2Int playerCell = placementGrid.WorldToCell(localPlayer.transform.position);
			Vector2Int mouseCell = GetMouseCell();

			if (ShouldRefresh(playerCell, mouseCell, itemData.ItemID))
				RefreshHighlights(mouseCell, itemData.Placement);

			if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
				TryPlace(itemData);
		}

		private bool CanPreviewPlacement()
		{
			if (NetworkDay.Instance != null && NetworkDay.Instance.IsRunning.Value)
				return false;

			if (localPlayer == null)
				localPlayer = PlayerController.LocalPlayer;

			if (cachedCamera == null)
				cachedCamera = Camera.main;

			return localPlayer != null && cachedCamera != null;
		}

		private bool TryGetHeldPlaceable(out ItemData itemData)
		{
			itemData = null;
			int itemId = localPlayer.SelectedItemId;

			if (itemId < 0)
				return false;

			itemData = Managers.Resource.GetItemData(itemId);
			return itemData != null && itemData.IsPlaceable;
		}

		private Vector2Int GetMouseCell()
		{
			Vector2 screenPosition = Mouse.current != null
				? Mouse.current.position.ReadValue()
				: Vector2.zero;

			Vector3 worldPosition = cachedCamera.ScreenToWorldPoint(screenPosition);
			worldPosition.z = 0f;
			return placementGrid.WorldToCell(worldPosition);
		}

		private bool ShouldRefresh(Vector2Int playerCell, Vector2Int mouseCell, int selectedItemId)
		{
			bool changed = !hasLastPlayerCell || !hasLastMouseCell ||
				playerCell != lastPlayerCell ||
				mouseCell != lastMouseCell ||
				selectedItemId != lastSelectedItemId;

			lastPlayerCell = playerCell;
			lastMouseCell = mouseCell;
			lastSelectedItemId = selectedItemId;
			hasLastPlayerCell = true;
			hasLastMouseCell = true;

			return changed;
		}

		private void RefreshHighlights(Vector2Int mouseCell, PlacementDefinition placement)
		{
			if (!TryGetPreviewOrigin(lastPlayerCell, mouseCell, placement, out Vector2Int originCell))
			{
				HideHighlights();
				localPlayer.SetHeldItemTint(Color.white);
				hasActiveOriginCell = false;
				return;
			}

			Vector2Int[] footprint = placement.Footprint;
			EnsureHighlightCount(footprint.Length);

			bool canPlace = placementGrid.CanPlace(originCell, placement);
			for (int i = 0; i < highlights.Length; i++)
			{
				if (i >= footprint.Length)
				{
					highlights[i].gameObject.SetActive(false);
					continue;
				}

				Vector2Int cell = originCell + footprint[i];

				highlights[i].transform.position = placementGrid.CellToWorldCenter(cell);
				highlights[i].color = canPlace ? hoverValidColor : hoverInvalidColor;
				highlights[i].gameObject.SetActive(true);
			}

			activeOriginCell = originCell;
			hasActiveOriginCell = true;
			localPlayer.SetHeldItemTint(canPlace ? Color.white : heldInvalidColor);
		}

		private void TryPlace(ItemData itemData)
		{
			if (!hasActiveOriginCell)
				return;

			if (!placementGrid.CanPlace(activeOriginCell, itemData.Placement))
				return;

			localPlayer.RequestPlaceItem(itemData.ItemID, activeOriginCell);
		}

		private bool TryGetPreviewOrigin(Vector2Int playerCell, Vector2Int mouseCell, PlacementDefinition placement, out Vector2Int originCell)
		{
			originCell = default;
			Vector2Int mouseOffset = mouseCell - playerCell;
			if (mouseOffset == Vector2Int.zero || placement == null || placement.Footprint == null || placement.Footprint.Length == 0)
				return false;

			Vector2Int direction = GetClosestDirection(mouseOffset);
			originCell = GetOriginForDirection(playerCell, direction, placement.Footprint);
			return true;
		}

		private static Vector2Int GetClosestDirection(Vector2Int offset)
		{
			Vector2 direction = new Vector2(offset.x, offset.y).normalized;
			Vector2Int closestDirection = DirectionOffsets[0];
			float closestDot = float.MinValue;

			foreach (Vector2Int candidate in DirectionOffsets)
			{
				float dot = Vector2.Dot(direction, new Vector2(candidate.x, candidate.y).normalized);
				if (dot > closestDot)
				{
					closestDot = dot;
					closestDirection = candidate;
				}
			}

			return closestDirection;
		}

		private static Vector2Int GetOriginForDirection(Vector2Int playerCell, Vector2Int direction, Vector2Int[] footprint)
		{
			GetFootprintBounds(footprint, out int minX, out int maxX, out int minY, out int maxY);

			int originX;
			if (direction.x > 0)
				originX = playerCell.x + 1 - minX;
			else if (direction.x < 0)
				originX = playerCell.x - 1 - maxX;
			else
				originX = playerCell.x - Mathf.FloorToInt((minX + maxX) * 0.5f);

			int originY;
			if (direction.y > 0)
				originY = playerCell.y + 1 - minY;
			else if (direction.y < 0)
				originY = playerCell.y - 1 - maxY;
			else
				originY = playerCell.y - Mathf.FloorToInt((minY + maxY) * 0.5f);

			return new Vector2Int(originX, originY);
		}

		private static void GetFootprintBounds(Vector2Int[] footprint, out int minX, out int maxX, out int minY, out int maxY)
		{
			minX = maxX = footprint[0].x;
			minY = maxY = footprint[0].y;

			for (int i = 1; i < footprint.Length; i++)
			{
				Vector2Int cell = footprint[i];
				minX = Mathf.Min(minX, cell.x);
				maxX = Mathf.Max(maxX, cell.x);
				minY = Mathf.Min(minY, cell.y);
				maxY = Mathf.Max(maxY, cell.y);
			}
		}

		private void ClearPreview()
		{
			HideHighlights();
			localPlayer?.SetHeldItemTint(Color.white);
			hasLastPlayerCell = false;
			hasLastMouseCell = false;
			hasActiveOriginCell = false;
			lastSelectedItemId = int.MinValue;
		}

		private void CreateHighlights()
		{
			highlights = global::System.Array.Empty<SpriteRenderer>();
			highlightSprite = CreateHighlightSprite();
		}

		private void EnsureHighlightCount(int count)
		{
			if (highlights.Length >= count)
				return;

			int oldCount = highlights.Length;
			global::System.Array.Resize(ref highlights, count);

			for (int i = oldCount; i < highlights.Length; i++)
			{
				GameObject highlight = new($"Placement Highlight {i}");
				highlight.transform.SetParent(transform);
				highlight.transform.localScale = placementGrid.CellSize;

				SpriteRenderer renderer = highlight.AddComponent<SpriteRenderer>();
				renderer.sprite = highlightSprite;
				renderer.sortingOrder = highlightSortingOrder;
				highlights[i] = renderer;
			}
		}

		private static Sprite CreateHighlightSprite()
		{
			Texture2D texture = Texture2D.whiteTexture;
			return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
		}

		private void HideHighlights()
		{
			if (highlights == null)
				return;

			foreach (SpriteRenderer highlight in highlights)
				highlight.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			if (highlightSprite != null)
				Destroy(highlightSprite);
		}
	}
}
