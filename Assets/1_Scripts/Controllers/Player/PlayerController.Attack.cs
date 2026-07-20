using MeowTruck.Data;
using MeowTruck.Manager;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Controllers
{
	/// <summary>
	/// 
	/// Player의 공격 로직 관련 스크립트
	/// 
	/// </summary>
	public partial class PlayerController
	{
		private bool isEnableToCombo = false;
		private bool isComboQueued = false;
		private int comboIndex = 0;

		public int ComboIndex => comboIndex;
		public bool IsEnableToCombo => isEnableToCombo;

		public void Attack()
		{
			// Called from ItemUseBehaviour
			stateMachine.ChangeState(stateMachine.Attack);
		}

		public void ChargeAttack()
		{
			stateMachine.ChangeState(stateMachine.Idle);
		}

		[Rpc(SendTo.Server)]
		public void AttackServerRPC(Vector2 direction)
		{
			ItemData itemData = Managers.Resource.GetItemData(selectedItemId.Value);
			if (itemData.UseBehaviour is AttackBehaviour attackBehaviour)
			{
				attackBehaviour.Attack(this, direction, attackTargetLayer);
			}
		}

		/** Animation Events**/
		public void OnAttackHit()
		{
			if (!IsOwner) return;

			isEnableToCombo = true;
			AttackServerRPC(currentDir);
		}

		public void OnAttackEnd()
		{
			if (!IsOwner) return;

			isEnableToCombo = false;
			if (ConsumeCombo())
			{
				animator.SetInteger(Constants.ANIM_PARAM_COMBO, comboIndex);
				animator.SetBool(Constants.ANIM_PARAM_ATTACK, true);

				NextCombo();
			}
			else
			{
				ResetCombo();
				stateMachine.ChangeState(stateMachine.Idle);
			}
		}

		public void QueueCombo()
		{
			if (comboIndex == 0)
			{
				animator.SetBool(Constants.ANIM_PARAM_ATTACK, true);
				NextCombo();
				return;
			}

			if (!isEnableToCombo) return;
			isComboQueued = true;
		}

		public bool ConsumeCombo()
		{
			bool value = isComboQueued;
			isComboQueued = false;

			AttackBehaviour attackBehaviour = Managers.Resource.GetItemData(selectedItemId.Value)?.UseBehaviour as AttackBehaviour;
			if (attackBehaviour == null) return false;
			bool isComboMax = (comboIndex == attackBehaviour.MaxCombo);

			return value && !isComboMax;
		}

		public void ResetCombo()
		{
			comboIndex = 0;
			animator.SetInteger(Constants.ANIM_PARAM_COMBO, comboIndex);
			isComboQueued = false;
		}

		public void NextCombo()
		{
			comboIndex++;
		}
	}
}
