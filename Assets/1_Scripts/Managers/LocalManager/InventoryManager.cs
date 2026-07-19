using MeowTruck.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeowTruck.Manager
{
	public class ItemStack
	{
		public ItemData Item;
		public int Count;
	}

	public class InventoryManager
	{
		private List<ItemStack> items { get; } = new();

		public Action<int, ItemStack> OnSlotChanged{ get; set; }
		public Action<List<ItemStack>> OnInventoryUpdated { get; set; }

		public void Init(int count)
		{
			for (int i = 0; i < count; i++)
			{
				items.Add(new ItemStack
				{
					Item = null,
					Count = 0
				});
			};
		}

		public bool IsAbleToAdd(int itemId, int count)
		{
			if (!TryGetSlotId(itemId, out int slotId))
			{
				if(!TryGetEmptySlotId(out slotId))
				{
					return false;
				}

				return true;
			}

			return items[slotId].Item.MaxStack >= items[slotId].Count + count;
		}
		public void AddItem(int itemId, int count = 1)
		{
			if (!TryGetSlotId(itemId, out int slotId))
			{
				if (!TryGetEmptySlotId(out slotId))
				{
					return;
				}

				items[slotId] = new ItemStack
				{
					Item = Managers.Resource.GetItemData(itemId),
					Count = count
				};
			}
			else
			{
				items[slotId].Count += count;
			}

			Debug.Log("ADD ITEM1");
			OnSlotChanged.Invoke(slotId, items[slotId]);
		}

		// count가 -1이면 전부 삭제
		public void RemoveItem(int slotId, int count = -1)
		{
			if (slotId >= items.Count)
			{
				Debug.LogWarning("[InventoryManager] - wrong slotID");
				return;
			}
			OnSlotChanged.Invoke(slotId, items[slotId]);
		}
		public void ChangeSlot(int slotId1, int slotId2)
		{
			if (slotId1 >= items.Count || slotId2 >= items.Count)
			{
				Debug.LogWarning("[InventoryManager] - wrong slotID");
				return;
			}

			OnSlotChanged.Invoke(slotId1, items[slotId1]);
			OnSlotChanged.Invoke(slotId2, items[slotId2]);
		}											   

		private bool TryGetSlotId(int itemId, out int slotId)
		{
			slotId = -1;

			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].Item != null && items[i].Item.ItemID == itemId)
				{
					slotId = i;
					return true;
				}
			}

			return false;
		}
		private bool TryGetEmptySlotId(out int slotId)
		{
			slotId = -1;

			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].Item == null)
				{
					slotId = i;
					return true;
				}
			}

			return false;
		}

		public void UpdateInventory()
		{
			OnInventoryUpdated.Invoke(items);
		}
	}
}
