using UnityEngine;

namespace MeowTruck.Data
{
	[CreateAssetMenu(fileName = "Item Data", menuName = "SO/Item Data")]
	public class ItemData : ScriptableObject
	{
		[SerializeField] private int itemID;
		[SerializeField] private int maxStack;
		[SerializeField] private Sprite itemSprite;

		public int ItemID => itemID;
		public int MaxStack => maxStack;
		public Sprite ItemSprite => itemSprite;
	}
}