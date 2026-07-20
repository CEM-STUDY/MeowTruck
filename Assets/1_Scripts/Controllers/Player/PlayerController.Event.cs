using MeowTruck.Data;
using MeowTruck.Manager;
using UnityEngine;

namespace MeowTruck.Controllers
{
	/// <summary>
	/// 
	/// Player의 이벤트 관련 스크립트
	/// 
	/// </summary>
	public partial class PlayerController
	{
		private void OnSlotSelected(int slotID)
		{
			if (!Managers.Inventory.TryGetItemData(slotID, out ItemData itemData))
			{
				selectedItemId.Value = -1;
				return;
			}

			selectedItemId.Value = itemData.ItemID;
		}

		private void OnItemSelected(int itemID)
		{
			if (itemID == -1)
			{
				itemSprite.sprite = null;
				return;
			}

			itemSprite.sprite = Managers.Resource.GetItemData(itemID).ItemSprite;
		}

		/** Animation Events**/
		public void OnAttackFrame()
		{

		}
	}
}
