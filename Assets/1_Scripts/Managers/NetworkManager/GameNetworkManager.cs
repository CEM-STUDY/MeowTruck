using Cysharp.Threading.Tasks;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Manager
{
	/// <summary>
	/// SteamworksSDK와 로비 연결을 다룹니다.
	/// </summary>
	public class GameNetworkManager : MonoBehaviour
	{
		public static GameNetworkManager instance { get; private set; } = null;

		private FacepunchTransport transport = null;

		public Lobby? currentLobby { get; private set; } = null;

		public ulong hostId;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				Destroy(gameObject);
				return;
			}
		}

		private void Start()
		{
			transport = GetComponent<FacepunchTransport>();

			SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
			SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
			SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
			SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
			SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
			SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
			SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;

		}

		private void OnDestroy()
		{
			SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
			SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
			SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
			SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
			SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
			SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
			SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

			if (NetworkManager.Singleton == null)
			{
				return;
			}
			NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
			NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
			NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;

		}

		private void OnApplicationQuit()
		{
			Disconnected();
		}

		//when you accept the invite or Join on a friend
		private async void SteamFriends_OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
		{
			RoomEnter joinedLobby = await lobby.Join();
			if (joinedLobby != RoomEnter.Success)
			{
				Debug.Log("Failed to create lobby");
			}
			else
			{
				currentLobby = lobby;
				GameManagerEx.Instance.ConnectedAsClient();
				Debug.Log("Joined Lobby");
			}
		}

		private void SteamMatchmaking_OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId)
		{
			Debug.Log("Lobby was created");
		}

		//friend send you an steam invite
		private void SteamMatchmaking_OnLobbyInvite(Friend steamId, Lobby lobby)
		{
			Debug.Log($"Invite from {steamId.Name}");
		}

		private void SteamMatchmaking_OnLobbyMemberLeave(Lobby lobby, Friend steamId)
		{
			Debug.Log("member leave"); 
			// GameManagerEx.instance.SendMessageToChat($"{_steamId.Name} has left", _steamId.Id, true);
			// NetworkTransmission.instance.RemoveMeFromDictionaryServerRPC(_steamId.Id);
		}

		private void SteamMatchmaking_OnLobbyMemberJoined(Lobby lobby, Friend steamId)
		{
			Debug.Log("member join");
		}

		private void SteamMatchmaking_OnLobbyEntered(Lobby lobby)
		{
			if (NetworkManager.Singleton.IsHost)
			{
				return;
			}
			StartClient(currentLobby.Value.Owner.Id);

		}

		private void SteamMatchmaking_OnLobbyCreated(Result result, Lobby lobby)
		{
			if (result != Result.OK)
			{
				Debug.Log("lobby was not created");
				return;
			}
			lobby.SetPublic();
			lobby.SetJoinable(true);
			lobby.SetGameServer(lobby.Owner.Id);
			Debug.Log($"lobby created");
		}

		public async UniTask StartHost(int maxMembers)
		{
			NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
			NetworkManager.Singleton.StartHost();
			GameManagerEx.Instance.MyClientId = NetworkManager.Singleton.LocalClientId;
			currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);
		}

		public void StartClient(SteamId sId)
		{
			NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
			NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
			transport.targetSteamId = sId;
			GameManagerEx.Instance.MyClientId = NetworkManager.Singleton.LocalClientId;
			if (NetworkManager.Singleton.StartClient())
			{
				Debug.Log("Client has started");
			}
		}

		public void Disconnected()
		{
			currentLobby?.Leave();
			if (NetworkManager.Singleton == null)
			{
				return;
			}
			if (NetworkManager.Singleton.IsHost)
			{
				NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
			}
			else
			{
				NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
			}
			NetworkManager.Singleton.Shutdown(true);
			GameManagerEx.Instance.Disconnected();
			Debug.Log("disconnected");
		}

		private void Singleton_OnClientDisconnectCallback(ulong cliendId)
		{
			NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
			if (cliendId == 0)
			{
				Disconnected();
			}
		}

		private void Singleton_OnClientConnectedCallback(ulong cliendId)
		{
			GameManagerEx.Instance.MyClientId = cliendId;
			Debug.Log($"Client has connected : AnotherFakeSteamName");
		}

		private void Singleton_OnServerStarted()
		{
			Debug.Log("Host started");
			GameManagerEx.Instance.HostCreated();
		}
	}
}