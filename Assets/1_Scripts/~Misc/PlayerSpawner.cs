using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Misc
{
	public class PlayerSpawner : NetworkBehaviour
	{
		[Header("PlayerSpawn")]
		[SerializeField] private GameObject playerPrefab;

		protected override void OnNetworkPostSpawn()
		{
			SpawnPlayerObject();
		}

		public void SpawnPlayerObject()
		{
			if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
			{
				if (NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject() != null) return;

				SpawnPlayerObjectServerRPC(NetworkManager.Singleton.LocalClientId);
			}
		}

		[Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
		public void SpawnPlayerObjectServerRPC(ulong clientId)
		{
			var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
			player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
		}
	}
}