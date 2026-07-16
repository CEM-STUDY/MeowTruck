using MeowTruck.Manager;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Misc
{
	public class PlayerSpawner : MonoBehaviour
	{
		[Header("PlayerSpawn")]
		[SerializeField] private GameObject playerPrefab;

		private void Start()
		{
			SpawnPlayerObject();
		}

		public void SpawnPlayerObject()
		{
			if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
			{
				if (NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject() != null) return;

				var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
				player.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
			}
		}
	}
}