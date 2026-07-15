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