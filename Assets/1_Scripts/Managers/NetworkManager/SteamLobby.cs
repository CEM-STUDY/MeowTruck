using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;

namespace MeowTruck.Manager
{
	/// <summary>
	/// 
	/// - Steam Lobby 관리 및 관련 API 제공
	/// - Steam 관련 Callback 관리
	/// 
	/// </summary>
	public class SteamLobby
	{
		/** Callbacks **/
		public Action<Lobby, SteamId> OnGameLobbyJoinRequested { get; set; } = null;
		public Action<Lobby, uint, ushort, SteamId> OnLobbyGameCreated { get; set; } = null;
		public Action<Friend, Lobby> OnLobbyInvite { get; set; } = null;
		public Action<Lobby, Friend> OnLobbyMemberLeave { get; set; } = null;
		public Action<Lobby, Friend> OnLobbyMemberJoined { get; set; } = null;
		public Action<Lobby> OnLobbyEntered { get; set; } = null;
		public Action<Result, Lobby> OnLobbyCreated { get; set; } = null;

		public void OnEnable()
		{
			SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
			SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
			SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
			SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
			SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
			SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
			SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;
		}
		public void OnDisable()
		{
			SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
			SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
			SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
			SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
			SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
			SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
			SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;
		}

		public async void FindLobbiesWithCallback(Action<Lobby[]> callback)
		{
			try
			{
				var query = SteamMatchmaking.LobbyList
					.WithKeyValue(Constants.KEY_GAMENAME, Constants.KEY_GAMENAME);

				var lobbies = await query.RequestAsync();
				if (lobbies == null) return;

				callback?.Invoke(lobbies);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
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

		private async void SteamFriends_OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
		{
			Debug.Log("SteamFriends_OnGameLobbyJoinRequested");
			RoomEnter joinedLobby = await lobby.Join();
			if (joinedLobby != RoomEnter.Success)
			{
				Debug.Log("Failed to create lobby");
			}
			else
			{
				Debug.Log("Joined Lobby");
				OnGameLobbyJoinRequested?.Invoke(lobby, steamId);
			}
		}

		private void SteamMatchmaking_OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId)
		{
			Debug.Log("SteamMatchmaking_OnLobbyGameCreated");
			OnLobbyGameCreated?.Invoke(lobby, ip, port, steamId);
		}

		private void SteamMatchmaking_OnLobbyInvite(Friend steamId, Lobby lobby)
		{
			Debug.Log($"SteamMatchmaking_OnLobbyInvite {steamId.Name}");
			OnLobbyInvite?.Invoke(steamId, lobby);
		}

		private void SteamMatchmaking_OnLobbyMemberLeave(Lobby lobby, Friend steamId)
		{
			Debug.Log("SteamMatchmaking_OnLobbyMemberLeave");
			OnLobbyMemberLeave?.Invoke(lobby, steamId);
		}

		private void SteamMatchmaking_OnLobbyMemberJoined(Lobby lobby, Friend steamId)
		{
			Debug.Log("SteamMatchmaking_OnLobbyMemberJoined");
			OnLobbyMemberJoined?.Invoke(lobby, steamId);
		}

		private void SteamMatchmaking_OnLobbyEntered(Lobby lobby)
		{
			Debug.Log("SteamMatchmaking_OnLobbyEntered");
			OnLobbyEntered?.Invoke(lobby);
		}

		private void SteamMatchmaking_OnLobbyCreated(Result result, Lobby lobby)
		{
			if (result != Result.OK)
			{
				Debug.Log("lobby was not created");
				return;
			}

			OnLobbyCreated?.Invoke(result, lobby);
			Debug.Log($"SteamMatchmaking_OnLobbyCreated");
		}
	}
}
