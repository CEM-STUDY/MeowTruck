using UnityEngine;

namespace MeowTruck.Data
{
	[CreateAssetMenu(fileName = "Item Data", menuName = "SO/Item Data")]
	public class ItemData : ScriptableObject
	{
		[SerializeField] private int itemID;
		[SerializeField] private int maxStack;
		[SerializeField] private Sprite itemSprite;
		[SerializeField] private PlacementDefinition placement;

		public int ItemID => itemID;
		public int MaxStack => maxStack;
		public Sprite ItemSprite => itemSprite;
		public PlacementDefinition Placement => placement;
		public bool IsPlaceable => placement != null;
	}
}
