using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Manager
{
	public class GameManagerEx : MonoBehaviour
	{
		private bool isConnected;
		private bool inGame;
		private bool isHost;
		private ulong myClientId;

		public ulong MyClientId { get => myClientId; set { myClientId = value;} }

		[Header("PlayerSpawn")]
		[SerializeField] private GameObject playerPrefab;

		private static GameManagerEx instance;
		public static GameManagerEx Instance => instance;
		
		
		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(this);
			}
		}

		[ContextMenu("PLAYER SPAWN TEST")]
		public void SpawnPlayerObject()
		{
			if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
			{
				if (NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject() != null) return;

				var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
				player.GetComponent<NetworkObject>().SpawnAsPlayerObject(myClientId);
			}
		}
		public void HostCreated()
		{
			isHost = true;
			isConnected = true;
		}

		public void ConnectedAsClient()
		{
			isHost = false;
			isConnected = true;
		}

		public void Disconnected()
		{
			isHost = false;
			isConnected = false;
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}