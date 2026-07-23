using Cysharp.Threading.Tasks;
using MeowTruck.Manager;
using System;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Controllers
{
	/// <summary>
	/// 
	/// Player의 기본적인 동작 관련 스크립트
	/// 
	/// </summary>
	public partial class PlayerController : NetworkBehaviour
    {
		public static PlayerController LocalPlayer { get; private set; }

		[Header("Movement Params")]
		[SerializeField] private float moveSpeed;
		[SerializeField] private float dashSpeed;
		[SerializeField] private float dashDuration;
		[Space(20)]

		[Header("Bindings")]
		[SerializeField] private SpriteRenderer itemSprite;

		[Header("Interacts")]
		[SerializeField] private LayerMask interactLayer;
		[SerializeField] private float interactDistance;

		[SerializeField] private LayerMask itemLayer;
		[SerializeField] private Vector2 itemDetectOffset;
		[SerializeField] private float itemDetectRange;
		[Space(20)]


		private PlayerController instance = null;
		private PlayerStateMachine stateMachine = null;

		/** Components **/
		private new Rigidbody2D rigidBody;

		private bool isDashing = false;
		public bool IsDashing => isDashing;

		private Vector2 prevDir = Vector2.right;

		private NetworkVariable<int> selectedItemId = new(-1, writePerm: NetworkVariableWritePermission.Owner);
		public int SelectedItemId => selectedItemId.Value;

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
			if (!IsOwner) return;

			stateMachine = new(this);

			Managers.Inventory.OnSlotSelected += ((_, cur) => { OnSlotSelected(cur); });
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			if (IsOwner)
				LocalPlayer = this;
			
			selectedItemId.OnValueChanged += ((_, itemId) =>
			{
				OnItemSelected(itemId);
			});
			OnItemSelected(selectedItemId.Value);
		}

		public override void OnNetworkDespawn()
		{
			if (LocalPlayer == this)
				LocalPlayer = null;

			base.OnNetworkDespawn();
		}

		private void Update()
		{
			if (!IsOwner) return;

			RecordPrevDir();
			TryPickUpItem();
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

		public void SetHeldItemTint(Color color)
		{
			if (itemSprite == null) return;
			itemSprite.color = color;
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
