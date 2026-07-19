using UnityEngine;

namespace MeowTruck.Controllers
{
	public class PlayerStateMachine
	{
		public StateBase CurState { get; private set; }

		public IdleState Idle { get; }
		public MoveState Move { get; }
		public AttackState Attack { get; }
		public InteractState Interact { get; }
		public DashState Dash { get; }

		public PlayerStateMachine(PlayerController controller)
		{
			Idle = new(controller, this);
			Move = new(controller, this);
			Attack = new(controller, this);
			Interact = new(controller, this);
			Dash = new(controller, this);

			CurState = Idle;
			CurState.Enter();
		}

		public void ChangeState(StateBase newState)
		{
			CurState.Exit();
			Debug.Log("CHANGE TO : " + newState.ToString());
			CurState = newState;
			CurState.Enter();
		}
	}

}
