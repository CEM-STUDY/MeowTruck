using UnityEngine;

namespace MeowTruck.Systems.Placement
{
	public class PlacedItem : MonoBehaviour
	{
		public int ItemId { get; private set; }
		public Vector2Int OriginCell { get; private set; }
		public Vector2Int[] Footprint { get; private set; }

		public void Init(int itemId, Vector2Int originCell, Vector2Int[] footprint)
		{
			ItemId = itemId;
			OriginCell = originCell;
			Footprint = footprint;
		}
	}
}
