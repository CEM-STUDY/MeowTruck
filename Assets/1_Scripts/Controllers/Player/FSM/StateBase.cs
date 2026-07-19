using MeowTruck.Manager;
using UnityEngine;

namespace MeowTruck.Controllers
{
	public class StateBase
	{
		protected PlayerController controller;      // Needed to control player (ex. move)
		protected PlayerStateMachine stateMachine;

		public StateBase(PlayerController controller, PlayerStateMachine stateMachine)
		{
			this.controller = controller;
			this.stateMachine = stateMachine;
		}

		public virtual void Enter() { }
		public virtual void Exit() { }
		public virtual void OnUpdate() { }
		public virtual void OnLateUpdate() { }
		public virtual void OnPhysicsUpdate() { }

		protected bool CheckIsAbleToIdle()
		{
			Vector2 move = Managers.Input.Control.Player.Move.ReadValue<Vector2>();
			if (move.sqrMagnitude > 0.01f) return false;

			stateMachine.ChangeState(stateMachine.Idle);
			return true;
		}
		protected bool CheckIsAbleToMove()
		{
			Vector2 move = Managers.Input.Control.Player.Move.ReadValue<Vector2>();
			if (move.sqrMagnitude < 0.01f) return false;

			stateMachine.ChangeState(stateMachine.Move);
			return true;
		}
		protected bool CheckIsAbleToAttack()
		{
			if (!Managers.Input.Control.Player.Attack.WasPressedThisFrame()) return false;

			stateMachine.ChangeState(stateMachine.Attack);
			return true;
		}
		protected bool CheckIsAbleToDash()
		{
			if (!Managers.Input.Control.Player.Dash.WasPressedThisFrame()) return false;

			stateMachine.ChangeState(stateMachine.Dash);
			return true;
		}
	}
}
