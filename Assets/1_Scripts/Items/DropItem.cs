using MeowTruck.Data;
using MeowTruck.Manager;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Items
{
	public class DropItem : NetworkBehaviour
	{
		private ItemData itemData;
		private NetworkVariable<int> itemId = new(-1);
		private NetworkVariable<int> itemCount = new(0);

		private bool isOwned = true;

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			itemId.OnValueChanged += ((prev, cur) =>
			{
				ApplyItem(cur);
			});

			ApplyItem(itemId.Value);
		}

		public void InitDropItem(int id, int count)
		{
			itemId.Value = id;
			itemCount.Value = count;
			isOwned = false;
		}

		private void ApplyItem(int cur)
		{
			if (cur < 0) return;
			itemData = Managers.Resource.GetItemData(cur);
			GetComponent<SpriteRenderer>().sprite = itemData.ItemSprite;
		}

		// 지금 습득 가능한지 확인 후에 RPC 호출
		public void TryGetItem()
		{
			if (itemCount.Value == 0) return;   // Init 안됨

			if (Managers.Inventory.IsAbleToAdd(itemData.ItemID, itemCount.Value, out int count))
				TryGetItemServerRPC(NetworkManager.LocalClientId);
		}

		[Rpc(SendTo.Server)]
		private void TryGetItemServerRPC(ulong clientID)
		{
			if (isOwned) return;
			isOwned = true;

			GetItemClientRPC(itemData.ItemID, itemCount.Value,
				new RpcParams
				{
					Send = new RpcSendParams
					{
						Target = RpcTarget.Single(clientID, RpcTargetUse.Temp)
					}
				});
		}

		[Rpc(SendTo.SpecifiedInParams)]
		private void GetItemClientRPC(int id, int count, RpcParams rpcParmas = default)
		{
			Managers.Inventory.AddItem(id, count, out int getCount);
			SendResultServerRPC(getCount);
		}

		[Rpc(SendTo.Server)]
		private void SendResultServerRPC(int count)
		{
			itemCount.Value -= count;
			isOwned = false;
			if (itemCount.Value == 0)
				GetComponent<NetworkObject>().Despawn(true);
		}
	}
}
