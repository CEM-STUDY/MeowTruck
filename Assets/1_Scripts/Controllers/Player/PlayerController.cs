using Cysharp.Threading.Tasks;
using MeowTruck.Manager;
using System;
using Unity.Netcode;
using UnityEditor.Analytics;
using UnityEngine;

namespace MeowTruck.Controllers
{
	public partial class PlayerController : NetworkBehaviour
    {
		[Header("Movement Params")]
		[SerializeField] private float moveSpeed;
		[SerializeField] private float dashSpeed;
		[SerializeField] private float dashDuration;

		private PlayerController instance = null;
		private PlayerStateMachine stateMachine = null;

		/** Components **/
		private new Rigidbody2D rigidBody;

		private bool isDashing = false;
		public bool IsDashing => isDashing;

		private Vector2 prevDir = Vector2.right;

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

			rigidBody = GetComponent<Rigidbody2D>();
		}

		private void Start()
		{
			stateMachine = new(this);
		}

		private void Update()
		{
			if (!IsOwner) return;

			RecordPrevDir();
			TryInteract();

			stateMachine.CurState.OnUpdate();
		}


		private void LateUpdate()
		{
			if (!IsOwner) return;

			stateMachine.CurState.OnLateUpdate();
		}

		private void FixedUpdate()
		{
			if (!IsOwner) return;

			stateMachine.CurState.OnPhysicsUpdate();
		}

		public void MovePosition(Vector2 dir)
		{
			rigidBody.linearVelocity = dir * moveSpeed;
		}
		public async UniTask Dash(Vector2 dir)
		{
			if (isDashing) return;
			isDashing = true;

			rigidBody.linearVelocity = dir.normalized * dashSpeed;
			await UniTask.Delay(TimeSpan.FromSeconds(dashDuration));

			isDashing = false;
		}

		private void RecordPrevDir()
		{
			Vector2 dir = Managers.Input.Control.Player.Move.ReadValue<Vector2>();
			if (dir.sqrMagnitude < 0.001f) return;

			prevDir = dir;
		}
	}
}