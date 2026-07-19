namespace MeowTruck.Controllers
{
	public class AttackState : StateBase
	{
		public AttackState(PlayerController controller, PlayerStateMachine stateMachine)
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
