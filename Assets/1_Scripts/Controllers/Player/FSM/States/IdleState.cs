using UnityEngine;

namespace MeowTruck.Controllers
{
	public class IdleState : StateBase
	{
		public IdleState(PlayerController controller, PlayerStateMachine stateMachine)
			: base(controller, stateMachine) { }

		public override void Enter()
		{
			base.Enter();
		}

		public override void Exit()
		{
			base.Exit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			if (CheckIsAbleToMove()) return;
			// if (CheckIsAbleToAttack()) return;
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
