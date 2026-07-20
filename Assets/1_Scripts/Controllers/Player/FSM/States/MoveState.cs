using MeowTruck.Manager;
using UnityEngine;

namespace MeowTruck.Controllers
{
	public class MoveState : StateBase
	{
		public MoveState(PlayerController controller, PlayerStateMachine stateMachine)
			: base(controller, stateMachine) { }

		public override void Enter()
		{
			base.Enter();
		}

		public override void Exit()
		{
			base.Exit();
			controller.ResetVelocity();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			if (CheckIsAbleToIdle()) return;
			if (CheckIsAbleToUse()) return;
			if (CheckIsAbleToDash()) return;

			Vector2 dir = Managers.Input.Control.Player.Move.ReadValue<Vector2>();
			controller.MovePosition(dir);
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
