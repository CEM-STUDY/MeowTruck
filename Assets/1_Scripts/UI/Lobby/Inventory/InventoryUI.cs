using MeowTruck.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MeowTruck.UI
{
	public class InventoryUI : MonoBehaviour
    {
        private List<ItemSlot> slots = new();

		private void Awake()
		{
			slots = GetComponentsInChildren<ItemSlot>().ToList();

			for (int i = 0; i < slots.Count; i++)
			{
				int t = i;
				slots[i].GetComponent<Button>().onClick.AddListener(() =>
				{
					Managers.Inventory.SelectSlot(t);
				});
				slots[i].GetComponent<ItemSlot>().Init(t);
			}
		}

		private void Start()
		{
			Managers.Inventory.OnSlotSelected += OnSlotSelected;
			Managers.Inventory.OnSlotChanged += OnSlotChanged;
			Managers.Inventory.OnInventoryUpdated += OnInventoryUpdated;

			Managers.Inventory.UpdateInventory();
		}

		private void OnInventoryUpdated(List<ItemStack> items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				OnSlotChanged(i, items[i]);
			}
		}

		private void OnSlotChanged(int slotId, ItemStack stack)
		{
			if (slots.Count <= slotId) return;
			slots[slotId].SetSprite(stack.Item != null ? stack.Item.ItemSprite : null);
			slots[slotId].SetCountText(stack.Count);
		}

		private void OnSlotSelected(int prev, int cur)
		{
			if (prev >= slots.Count || cur >= slots.Count)
			{
				Debug.LogWarning("[InventoryUI] - wrong slotId");
				return;
			}
			if (prev == cur) return;

			if (prev == -1)
				for (int i = 0; i < slots.Count; i++) slots[i].OnUnselected();
			else
				slots[prev].OnUnselected();

			slots[cur].OnSelected();
		}
	}
}