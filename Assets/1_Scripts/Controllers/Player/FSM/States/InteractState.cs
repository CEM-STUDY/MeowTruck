using MeowTruck.Manager;

namespace MeowTruck.Controllers
{
	public class InteractState : StateBase
	{
		public InteractState(PlayerController controller, PlayerStateMachine stateMachine)
			: base(controller, stateMachine) { }

		public override void Enter()
		{
			base.Enter();
		}

		public override void Exit()
		{
			base.Exit();
			controller.UnsetCurrentInteractable();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			if (Managers.Input.Control.Player.Interact.IsPressed())
			{
				controller.KeepInteracts();
				return;
			}

			stateMachine.ChangeState(stateMachine.Idle);
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
