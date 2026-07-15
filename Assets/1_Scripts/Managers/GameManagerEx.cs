using UnityEngine;

namespace MeowTruck.Manager
{
	public class GameManagerEx : MonoBehaviour
	{
		public bool connected;
		public bool inGame;
		public bool isHost;
		public ulong myClientId;

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
			connected = true;
		}

		public void ConnectedAsClient()
		{
			isHost = false;
			connected = true;
		}

		public void Disconnected()
		{
			isHost = false;
			connected = false;
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}