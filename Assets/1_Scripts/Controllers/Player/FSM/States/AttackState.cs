using MeowTruck.Manager;
using UnityEngine;

namespace MeowTruck.Controllers
{
	public class AttackState : StateBase
	{
		public AttackState(PlayerController controller, PlayerStateMachine stateMachine)
			: base(controller, stateMachine) { }

		private float elapsedTime = 0f;
		private bool isCharged = false;

		public override void Enter()
		{
			base.Enter();

			elapsedTime = 0f;
			isCharged = false;
		}

		public override void Exit()
		{
			base.Exit();

			controller.SetAnimatorParam("WeaponIndex", -1);
			controller.SetAnimatorParam(AnimParamType.Attack, false);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			if (Managers.Input.Control.Player.Use.IsPressed() &&
				controller.ComboIndex == 0)
			{
				elapsedTime += Time.deltaTime;
				if (elapsedTime > 0.5f)
				{
					// Player.Charge
					isCharged = true;
				}
			}

			if (Managers.Input.Control.Player.Use.WasReleasedThisFrame())
			{
				if (isCharged) controller.ChargeAttack();
				else controller.QueueCombo();
			}
		}

		public override void OnLateUpdate()
		{
			base.OnLateUpdate();
		}

		public override void OnPhysicsUpdate()
		{
			base.OnPhysicsUpdate();
		}
	}
}
