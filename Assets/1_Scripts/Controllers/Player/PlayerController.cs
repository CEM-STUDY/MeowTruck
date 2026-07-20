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
		[Space(20)]

		[SerializeField] private LayerMask itemLayer;
		[SerializeField] private Vector2 itemDetectOffset;
		[SerializeField] private float itemDetectRange;
		[Space(20)]

		[SerializeField] private LayerMask attackTargetLayer;

		private List<int> animIds = new();
		private PlayerController instance = null;
		private PlayerStateMachine stateMachine = null;

		/** Components **/
		private Rigidbody2D rigidBody;
		private Animator animator;

		private bool isDashing = false;
		public bool IsDashing => isDashing;

		private Vector2 currentDir = Vector2.right;

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

			CalculateCurrentDir();
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
		public void ResetVelocity()
		{
			rigidBody.linearVelocity = Vector2.zero;
		}
		public async UniTask Dash(Vector2 dir)
		{
			if (isDashing) return;
			isDashing = true;

			rigidBody.linearVelocity = dir.normalized * dashSpeed;
			await UniTask.Delay(TimeSpan.FromSeconds(dashDuration));

			isDashing = false;
		}

		public void UseCurrentItem()
		{
			Managers.Inventory.UseCurrentSelectedItem(this);
		}
		private void CalculateCurrentDir()
		{
			Vector2 screenPos = Managers.Input.Control.Player.MousePos.ReadValue<Vector2>();
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
			worldPos.z = 0f;

			currentDir = (worldPos - transform.position).ToVec2().normalized;
		}

		public void SetAnimatorParam(AnimParamType type) => animator.SetTrigger(animIds[(int)type]);
		public void SetAnimatorParam(AnimParamType type, bool isOn) => animator.SetBool(animIds[(int)type], isOn);
	}
}