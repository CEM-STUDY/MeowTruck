using System.Collections.Generic;
using System;
using Unity.Netcode;

namespace MeowTruck.Manager
{
	/// <summary>
	/// MonoBehaviour 오브젝트에서 전역적인 네트워크 통신 접근 필요시 사용합니다.
	/// 
	/// ex. 채팅, vfx, ...
	/// </summary>
	public class NetworkTransmission : NetworkBehaviour
	{
		private static NetworkTransmission instance;
		public static NetworkTransmission Instance => instance;

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
			}
		}

		/// <summary>
		/// Synchronize clients event
		/// 
		/// ex) Wait until all clients' cameras finish moving, then trigger the next event.
		/// </summary>

		private int currentEventId = 0;
		private Action onAllFinishedEvent = null;
		private HashSet<ulong> finishedClients = new();

		public Action<GameEventType, int> OnGameEvent { get; set; }

		public void StartEventSync(Action finishEvent, GameEventType evtType)
		{
			if (!NetworkManager.IsHost) return;

			onAllFinishedEvent = finishEvent;
			finishedClients.Clear();

			StartEventSyncClientRPC(evtType, ++currentEventId);
			OnGameEvent?.Invoke(evtType, currentEventId);   // OnHost
		}

		[Rpc(SendTo.NotServer, InvokePermission = RpcInvokePermission.Server)]
		public void StartEventSyncClientRPC(GameEventType evtType, int eventId)
		{
			currentEventId = eventId;
			OnGameEvent?.Invoke(evtType, eventId);  // OnClient
		}

		[Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
		public void ReportEventFinishServerRPC(int eventId, ulong clientId)
		{
			if (currentEventId != eventId) return;

			if (finishedClients.Add(clientId))
			{
				if (finishedClients.Count == NetworkManager.ConnectedClientsList.Count)
				{
					onAllFinishedEvent?.Invoke();
				}
			}
		}
	}
}