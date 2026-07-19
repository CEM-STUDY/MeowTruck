using MeowTruck.Controllers;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Interactables
{
	public class InteractableBase : NetworkBehaviour
	{
		public NetworkVariable<ulong> OwnerID { get; private set; } = new(ulong.MaxValue);

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			OwnerID.OnValueChanged += OnOwnerChanged;
		}

		public override void OnNetworkDespawn()
		{
			base.OnNetworkDespawn();
			OwnerID.OnValueChanged -= OnOwnerChanged;
		}

		/// <summary>
		/// Interactable 의 소유권을 갖기 위해 시도
		/// </summary>
		[Rpc(SendTo.Server)]
		public void TryGetOwnershipServerRPC(ulong localID)
		{
			if (OwnerID.Value != ulong.MaxValue) return;  // 이미 소유권이 있는 상태

			OwnerID.Value = localID;
		}

		[Rpc(SendTo.Server)]
		public void RemoveOwnershipServerRPC()
		{
			OwnerID.Value = ulong.MaxValue;
        }

		private void OnOwnerChanged(ulong prev, ulong next)
		{
			if (next != NetworkManager.LocalClientId) return;

			NetworkManager.SpawnManager.GetLocalPlayerObject()
				.GetComponent<PlayerController>().SetCurrentInteractable(this);
		}

		private void Update()
		{
			// HACK - 테스트용입니다
			GetComponent<SpriteRenderer>().color = (OwnerID.Value == NetworkManager.LocalClientId) 
				? Color.blue : Color.green;
		}
	}
}
