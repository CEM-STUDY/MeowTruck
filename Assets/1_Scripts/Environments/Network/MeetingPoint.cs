using MeowTruck.Manager;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MeowTruck.Environments
{
	public class MeetingPoint : NetworkBehaviour
	{
		public static event Action TravelMapOpened;
		public static event Action TravelMapClosed;

		[SerializeField] private Vector2 boxCenter;
		[SerializeField] private Vector2 boxSize;
		[SerializeField] private float maxTime = 5f;

		[Header("Progress Bar")]
		[SerializeField] private SpriteRenderer meetingProgress;

		private NetworkVariable<float> elapsedTime = new();

		[SerializeField, ReadOnly]
		private bool isMeeting = false;


		private Collider2D[] hits = new Collider2D[Constants.MAX_PLAYERS];
		private ContactFilter2D filter = new ContactFilter2D();
		private readonly HashSet<ulong> detectedClients = new();
		private bool meetingCompleted;

		private void Awake()
		{
			filter.SetLayerMask(Constants.LAYER_PLAYER);
			filter.useLayerMask = true;
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
		}

		public override void OnNetworkDespawn()
		{
			base.OnNetworkDespawn();
		}

		[ClientRpc]
		public void StartMeetClientRPC()
		{
			if (IsHost)
				elapsedTime.Value = 0f;

			gameObject.SetActive(true);
		}

		[ClientRpc]
		public void EndMeetClientRPC()
		{
			gameObject.SetActive(false);
		}

		public static void RequestCloseTravelMap()
		{
			MeetingPoint meetingPoint = FindFirstObjectByType<MeetingPoint>();
			if (meetingPoint != null && meetingPoint.IsHost)
			{
				meetingPoint.meetingCompleted = false;
				meetingPoint.elapsedTime.Value = 0f;
				meetingPoint.CloseTravelMapClientRpc();
			}
		}

		[ClientRpc]
		private void OpenTravelMapClientRpc()
		{
			TravelMapOpened?.Invoke();
		}

		[ClientRpc]
		private void CloseTravelMapClientRpc()
		{
			TravelMapClosed?.Invoke();
		}

		private int playerCount = 0;
		private bool isLocalPlayerDetected = false;

		private void Update()
		{
			meetingProgress.size = new Vector2(Mathf.Clamp01(elapsedTime.Value / maxTime), 1f);
			playerCount = Physics2D.OverlapBox(transform.position.ToVec2() + boxCenter, boxSize * 0.5f, 0f, filter, hits);
			
			isLocalPlayerDetected = false;
			for (int i = 0; i < playerCount; i++)
			{
				var networkBehaviour = hits[i].GetComponent<NetworkBehaviour>();
				if (networkBehaviour != null && networkBehaviour.IsLocalPlayer)
				{
					isLocalPlayerDetected = true;
					break;
				}
			}

			if (!IsHost) return;

			detectedClients.Clear();
			for (int i = 0; i < playerCount; i++)
			{
				NetworkObject networkObject = hits[i].GetComponentInParent<NetworkObject>();
				if (networkObject != null && networkObject.IsPlayerObject)
					detectedClients.Add(networkObject.OwnerClientId);
			}

			isMeeting = detectedClients.Count == NetworkManager.Singleton.ConnectedClients.Count;
			if (isMeeting)
			{
				if (meetingCompleted) return;

				elapsedTime.Value = Mathf.Min(elapsedTime.Value + Time.deltaTime, maxTime);
				if (elapsedTime.Value >= maxTime)
				{
					// TODO - 게이지 다 차면 할 일 작성
					meetingCompleted = true;
					if (SceneManager.GetActiveScene().name == Constants.SCENE_VILLAGE)
						OpenTravelMapClientRpc();
					else
						Managers.Scene.ChangeScene(Constants.SCENE_VILLAGE);
				}
			}
			else
			{
				if (!meetingCompleted && elapsedTime.Value > 0f) elapsedTime.Value = 0f;
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.matrix = Matrix4x4.TRS(transform.position.ToVec2() + boxCenter, Quaternion.identity, Vector3.one);
			Gizmos.DrawWireCube(Vector3.zero, boxSize);
			Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
			Gizmos.DrawCube(Vector3.zero, boxSize);
		}
	}
}
