using Cysharp.Threading.Tasks;
using MeowTruck.Data;
using MeowTruck.Manager;
using System;
using System.Collections.Generic;
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

		private List<int> animIds = new();
		private PlayerController instance = null;
		private PlayerStateMachine stateMachine = null;

		/** Components **/
		private Rigidbody2D rigidBody;
		private Animator animator;

		private bool isDashing = false;
		public bool IsDashing => isDashing;

		private Vector2 prevDir = Vector2.right;

		private NetworkVariable<int> selectedItemId = new(-1, writePerm: NetworkVariableWritePermission.Owner);

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
			animator = GetComponent<Animator>();

			animIds.Add(Animator.StringToHash(Constants.ANIM_PARAM_ATTACK));
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
			
			selectedItemId.OnValueChanged += ((_, itemId) =>
			{
				OnItemSelected(itemId);
			});
			OnItemSelected(selectedItemId.Value);
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
		public async UniTask Dash(Vector2 dir)
		{
			if (isDashing) return;
			isDashing = true;

			rigidBody.linearVelocity = dir.normalized * dashSpeed;
			await UniTask.Delay(TimeSpan.FromSeconds(dashDuration));

			isDashing = false;
		}
		public void Attack(ItemData itemData)
		{
			// Called from ItemUseBehaviour
			// TODO - 방향 고려 필요
			stateMachine.ChangeState(stateMachine.Attack);
		}

		public void UseCurrentItem()
		{
			Managers.Inventory.UseCurrentSelectedItem(this);
		}
		private void RecordPrevDir()
		{
			Vector2 dir = Managers.Input.Control.Player.Move.ReadValue<Vector2>();
			if (dir.sqrMagnitude < 0.001f) return;

			prevDir = dir;
		}

		public void SetAnimatorParam(AnimParamType type) => animator.SetTrigger(animIds[(int)type]);
		public void SetAnimatorParam(AnimParamType type, bool isOn) => animator.SetBool(animIds[(int)type], isOn);
	}
}