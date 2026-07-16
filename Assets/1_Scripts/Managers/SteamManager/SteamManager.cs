using Cysharp.Threading.Tasks;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Manager
{
	/// <summary>
	/// 
	/// - Steam과 관련된 매니저들을 관리
	/// - NetworkManager 관련 API 제공 및 콜백 관리
	/// 
	/// </summary>
	public class SteamManager : MonoBehaviour
	{
		public static SteamManager Instance { get; private set; } = null;

		public static SteamCloud Cloud { get; private set; } = new SteamCloud();
		public static SteamAchievement Achieve { get; private set; } = new SteamAchievement();
		public static SteamLobby Lobby { get; private set; } = new SteamLobby();

		public Lobby? CurrentLobby { get; private set; } = null;
		public ulong HostId { get; private set; }

		private FacepunchTransport transport = null;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
            }

			transport = GetComponent<FacepunchTransport>();
			transport.Initialize();
		}

		private void OnEnable()
		{
			Lobby.OnEnable();

			Lobby.OnGameLobbyJoinRequested += ((lobby, steamId) => { CurrentLobby = lobby; });
			Lobby.OnLobbyEntered += ((lobby) => { if (!NetworkManager.Singleton.IsHost) StartClient(lobby.Owner.Id); });
			Lobby.OnLobbyCreated += ((result, lobby) =>
			{
				lobby.SetPublic();
				lobby.SetJoinable(true);
				lobby.SetData(Constants.KEY_GAMENAME, Constants.KEY_GAMENAME);
			});
		}

		private void OnDisable()
		{
			Lobby.OnDisable();

			if (NetworkManager.Singleton == null)
			{
				return;
			}
			NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
			NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
			NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
		}

		private void Update()
		{
			if (SteamClient.IsValid)
				SteamClient.RunCallbacks();
		}

		private void OnApplicationQuit()
		{
			SteamClient.Shutdown();
			Disconnected();
		}

		public async UniTask StartHost(int maxMembers)
		{
			NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
			NetworkManager.Singleton.StartHost();
			CurrentLobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);
		}

		public void StartClient(SteamId sId)
		{
			NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
			NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
			transport.targetSteamId = sId;
			if (NetworkManager.Singleton.StartClient())
			{
				Debug.Log("Client has started");
			}
		}

		public void Disconnected()
		{
			CurrentLobby?.Leave();
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
			Debug.Log("Disconnected");
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
			Debug.Log($"Singleton_OnClientConnectedCallback");
		}

		private void Singleton_OnServerStarted()
		{
			Debug.Log("Singleton_OnServerStarted");
		}
	}
}
