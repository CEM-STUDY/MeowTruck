using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace MeowTruck.Environments
{
	public class MeetingPoint : NetworkBehaviour
	{
		[SerializeField] private NetworkVariable<Vector2> boxCenter = new(Vector2.zero);
		[SerializeField] private NetworkVariable<Vector2> boxSize = new(Vector2.zero);
		[SerializeField] private float maxTime = 5f;

		[Header("Progress Bar")]
		[SerializeField] private SpriteRenderer meetingProgress;

		private NetworkVariable<float> elapsedTime = new();

		[SerializeField, ReadOnly]
		private bool isMeeting = false;

		public Action<MeetingPoint> OnMeetingCompletedAction { get; set; } = null;

		private Collider2D[] hits = new Collider2D[Constants.MAX_PLAYERS];
		private ContactFilter2D filter = new ContactFilter2D();

		private bool isOnce = false;

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

		// Host-Only
		public void SetMeetingPoint(Vector2 size, bool isOnce)
		{
			if (!IsHost)
			{
				Debug.LogWarning("[MeetingPoint] - it is Host-Only!");
				return;
			}

			this.isOnce = isOnce;
			boxSize.Value = size;
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

		private int playerCount = 0;
		private bool isLocalPlayerDetected = false;

		private void Update()
		{
			meetingProgress.size = new Vector2(Mathf.Clamp01(elapsedTime.Value / maxTime), 1f);
			playerCount = Physics2D.OverlapBox(transform.position.ToVec2() + boxCenter.Value, boxSize.Value * 0.5f, 0f, filter, hits);

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

			isMeeting = (playerCount == Unity.Netcode.NetworkManager.Singleton.ConnectedClients.Count);
			if (isMeeting)
			{
				elapsedTime.Value += Time.deltaTime;
				if (elapsedTime.Value > maxTime)
				{
					OnMeetingCompletedAction?.Invoke(this);
					if(isOnce) OnMeetingCompletedAction = null;
				}
			}
			else
			{
				if (elapsedTime.Value > 0f) elapsedTime.Value = 0;
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.matrix = Matrix4x4.TRS(transform.position.ToVec2() + boxCenter.Value, Quaternion.identity, Vector3.one);
			Gizmos.DrawWireCube(Vector3.zero, boxSize.Value);
			Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
			Gizmos.DrawCube(Vector3.zero, boxSize.Value);
		}
	}
}