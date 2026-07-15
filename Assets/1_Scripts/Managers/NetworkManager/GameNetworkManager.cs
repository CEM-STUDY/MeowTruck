using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;
using System;

namespace MeowTruck.Manager
{
	public class GameNetworkManager : MonoBehaviour
	{
		private FacepunchTransport transport = null;

		public Lobby? currentLobby { get; private set; } = null;

		#region Singleton
		public static GameNetworkManager Instance { get => instance; }
		private static GameNetworkManager instance = null;
		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
				return;
			}
		}
		#endregion
		private void Update()
		{
			if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsConnectedClient)
			{
				ulong ping = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.ServerClientId);
				//Debug.Log("PingRtt: " + ping + "ms");
			}
		}

		private void Start()
		{
			transport = GetComponent<FacepunchTransport>();

			SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
			SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
			SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyJoined;
			SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyLeaved;
			SteamMatchmaking.OnLobbyInvite += SteamMatchMaking_OnLobbyInvite;
			SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
			SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;
		}
		private void OnDestroy()
		{
			SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
			SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
			SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyJoined;
			SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyLeaved;
			SteamMatchmaking.OnLobbyInvite -= SteamMatchMaking_OnLobbyInvite;
			SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
			SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

			if (NetworkManager.Singleton == null) return;

			NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
			NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
			NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectedCallback;
		}
		private void OnApplicationQuit()
		{
			Disconnected();
		}

		#region Lobby Callbacks
		private void SteamMatchmaking_OnLobbyCreated(Result result, Lobby lobby)
		{
			if (result != Result.OK)
			{
				Debug.Log("Lobby was not created, result: " + result);
				// TODO - 로비 생성 실패 팝업?
				return;
			}

			// 로비 데이터 초기화
			lobby.SetPublic();
			lobby.SetJoinable(true);
			Debug.Log($"Lobby created : {lobby.Owner.Name}");

			// Host 시작
			StartHost();
		}
		private void SteamMatchmaking_OnLobbyEntered(Lobby lobby)
		{
			Debug.Log("Lobby entered");
			if (lobby.Owner.Id == SteamClient.SteamId) return;

			currentLobby = lobby;
			StartClient(lobby.Owner.Id);
		}
		private void SteamMatchmaking_OnLobbyJoined(Lobby lobby, Friend friend)
		{
			Debug.Log("member join");
		}

		// Only Lobby Owner
		private void SteamMatchmaking_OnLobbyLeaved(Lobby lobby, Friend friend)
		{
			Debug.Log("member leave");
			if (friend.Id == lobby.Owner.Id)
			{
				Debug.Log("HOST LEAVED");
			}
		}
		private void SteamMatchMaking_OnLobbyInvite(Friend friend, Lobby lobby)
		{
			Debug.Log($"Invite from {friend.Name}");
		}
		private void SteamMatchmaking_OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId)
		{
			Debug.Log("LobbyGame created");
		}

		// Accept the invice or join on a friend
		private async void SteamFriends_OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
		{
			RoomEnter joinedLobby = await lobby.Join();

			if (joinedLobby != RoomEnter.Success)
			{
				Debug.Log("Failed to create lobby");
			}
			else
			{
				Debug.Log("Joined Lobby");
			}
		}
		#endregion

		#region FromLobbyToGame Sequences
		public void StartHost()
		{
			Debug.Log("START HOST...");
			NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
			NetworkManager.Singleton.StartHost();
			NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectedCallback;
			GameManagerEx.Instance.MyClientId = NetworkManager.Singleton.LocalClientId;
		}
		public async void CreateLobby()
		{
			Debug.Log("Create lobby...");
			currentLobby = await SteamMatchmaking.CreateLobbyAsync(Constants.MAX_PLAYERS);
			currentLobby.Value.SetData("HELLO", "HIHI");
		}
		public void StartClient(SteamId steamId)
		{
			Debug.Log("Start client...");
			NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
			NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectedCallback;
			transport.targetSteamId = steamId.Value;
			GameManagerEx.Instance.MyClientId = NetworkManager.Singleton.LocalClientId;

			if (NetworkManager.Singleton.StartClient())
			{
				Debug.Log("StartClient...");
			}
		}
		public void StartGameInLobby()
		{
			if (!NetworkManager.Singleton.IsHost) return;

			currentLobby.Value.SetGameServer(currentLobby.Value.Owner.Id);
			Debug.Log("Start Game in lobby...");
			LockLobby();
		}

		// TODO - 나중에 인게임 메뉴 버튼에 할당해야됨
		public async void Disconnected()
		{
			if (currentLobby == null) return;
			currentLobby?.Leave();
			currentLobby = null;

			if (NetworkManager.Singleton == null) return;
			if (NetworkManager.Singleton.IsHost)
			{
				NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
			}
			else
			{
				NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
			}

			GameManagerEx.Instance.Disconnected();
			Debug.Log("Disconnected.");
			NetworkManager.Singleton.Shutdown(true);
			Debug.Log("Shutdown.");
		}
		public async void FindLobbiesWithCallback(Action<Lobby[]> callback)
		{
			var query = SteamMatchmaking.LobbyList
				.WithKeyValue("HELLO", "HIHI")
				.FilterDistanceClose();

			var lobbies = await query.RequestAsync();

			if (lobbies == null) return;

			callback.Invoke(lobbies);
			return;
		}
		public async void JoinLobby(Lobby lobby)
		{
			try
			{
				await lobby.Join();
			}
			catch (Exception e)
			{
				Debug.LogWarning($"Lobby enter failed : {e.Message}");
			}
		}
		public void LockLobby()
		{
			currentLobby.Value.SetJoinable(false);
		}
		public void UnlockLobby()
		{
			currentLobby.Value.SetJoinable(true);
		}
		#endregion

		private void Singleton_OnClientDisconnectedCallback(ulong clientId)
		{
			Debug.Log("Client Disconnected, ClientID: " + clientId);
			if (clientId == NetworkManager.Singleton.LocalClientId)
			{
				Disconnected();
				NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectedCallback;
			}
		}

		public void OpenInviteWindow()
		{
			SteamFriends.OpenGameInviteOverlay(currentLobby.Value.Id);
		}

		// Both Server and Client
		private void Singleton_OnClientConnectedCallback(ulong clientId)
		{
			if (NetworkManager.Singleton.IsHost) return;

			GameManagerEx.Instance.ConnectedAsClient();
			GameManagerEx.Instance.MyClientId = clientId;
		}
		private void Singleton_OnServerStarted()
		{
			Debug.Log("OnServerStarted Callback...");
			GameManagerEx.Instance.HostCreated();
		}
	}

}