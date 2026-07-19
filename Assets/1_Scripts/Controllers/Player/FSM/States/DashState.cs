using Cysharp.Threading.Tasks;
using MeowTruck.Manager;
using UnityEngine;

namespace MeowTruck.Controllers
{
	public class DashState : StateBase
	{
		public DashState(PlayerController controller, PlayerStateMachine stateMachine)
			: base(controller, stateMachine) { }

		public override void Enter()
		{
			base.Enter();
			controller.Dash(Managers.Input.Control.Player.Move.ReadValue<Vector2>()).Forget();
		}

		public override void Exit()
		{
			base.Exit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			if (!controller.IsDashing) stateMachine.ChangeState(stateMachine.Idle);
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
