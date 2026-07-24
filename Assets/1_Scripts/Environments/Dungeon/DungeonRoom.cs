using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Environments
{
	/// <summary>
	/// 
	/// 1. PrevRoom 의 endPoint에 모임 -> PrevRoom 으로 못돌아가게 문 닫기
	/// 2. CurrentRoom 스폰 -> 모든 클라에서 전부 스폰되면 PrevRoom은 FadeOut, CurrentRoom 은 FadeIn
	/// 
	/// </summary>
	public class DungeonRoom : NetworkBehaviour
	{
		[SerializeField] private Transform startPoint;
		[SerializeField] private Transform endPoint;
		[SerializeField] private GameObject meetingPointPrefab;

		[SerializeField] private List<Transform> enemySpawnPoints = new();
		[SerializeField] private List<Transform> naturalSpawnPoints = new();

		private MeetingPoint meetingPoint = null;
		private HashSet<ulong> clientIds = new();

		private bool isCleanUp = false;

		public bool IsReady => clientIds.Count == NetworkManager.ConnectedClients.Count;
		public Transform StartPoint => startPoint;

		private void Awake()
		{
			clientIds.Clear();
			meetingPoint = Instantiate(meetingPointPrefab, endPoint.position, Quaternion.identity)
				.GetComponent<MeetingPoint>();
			meetingPoint.NetworkObject.Spawn();
			meetingPoint.SetMeetingPoint(new Vector2(4.5f, 2.0f), true);
		}

		public void SpawnAll()
		{
			NetworkObject.Spawn();
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			OnNetworkInitServerRPC(NetworkManager.LocalClientId);
		}

		public override void OnNetworkDespawn()
		{
			base.OnNetworkDespawn();
		}

		public Vector2 GetSuitableSpawnPosition(Vector2 prevRoomEndPoint)
		{
			Vector2 offset = (Vector2)startPoint.position - (Vector2)transform.position;

			return prevRoomEndPoint - offset;
		}


		[Rpc(SendTo.Server)]
		public void OnNetworkInitServerRPC(ulong clientId)
		{
			clientIds.Add(clientId);
		}

		[Rpc(SendTo.Server)]
		public void OnNetworkCleanUpServerRPC(ulong clientId)
		{
			isCleanUp = true;
			if (clientIds.Contains(clientId)) { clientIds.Remove(clientId); }
		}

		private void Update()
		{
			if (IsReady)
			{
				DungeonFadeIn();
			}

			Debug.Log("NAME :" + this.gameObject.name);
			Debug.Log("ISHOST :" + IsHost);
			Debug.Log("ISCLEAN : " + isCleanUp);
			Debug.Log("CLIENTID : " + clientIds.Count);

			if (IsHost && isCleanUp && clientIds.Count == 0)
			{
				meetingPoint.NetworkObject.Despawn(destroy: true);
				NetworkObject.Despawn(destroy: true);
			}
		}

		public void SetNextRoom(Action<MeetingPoint> action)
		{
			meetingPoint.OnMeetingCompletedAction += action;
		}

		private void DungeonFadeIn()
		{
			// TODO - 점점 선명하게
			// 
		}

		[Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
		public void FadeOutClientRPC()
		{
			OnNetworkCleanUpServerRPC(NetworkManager.LocalClientId);
		}
	}
}