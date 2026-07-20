using UnityEngine;

namespace MeowTruck.Data
{
	[CreateAssetMenu(fileName = "Item Data", menuName = "SO/Item Data")]
	public class ItemData : ScriptableObject
	{
		[SerializeField] private int itemID;
		[SerializeField] private bool isConsumable;
		[SerializeField] private int maxStack;
		[SerializeField] private Sprite itemSprite;
		[SerializeField] private ItemUseBehaviour useBehaviour;

		public int ItemID => itemID;
		public bool IsConsumable => isConsumable;
		public int MaxStack => maxStack;
		public Sprite ItemSprite => itemSprite;
		public ItemUseBehaviour UseBehaviour => useBehaviour;
	}
}