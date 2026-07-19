using MeowTruck.Interactables;
using MeowTruck.Manager;
using Mono.Cecil.Cil;
using UnityEngine;

namespace MeowTruck.Controllers
{
	public partial class PlayerController
	{
		[Header("Interacts")]
		[SerializeField] private LayerMask interactLayer; 
		[SerializeField] private float interactDistance;

		public InteractableBase CurrentInteractable { get; private set; } = null;

		public void SetCurrentInteractable(InteractableBase interactables)
		{
			if (stateMachine.CurState != stateMachine.Idle && stateMachine.CurState != stateMachine.Move) return;

			CurrentInteractable = interactables;
			stateMachine.ChangeState(stateMachine.Interact);
		}
		public void UnsetCurrentInteractable()
		{
			CurrentInteractable.RemoveOwnershipServerRPC();
			CurrentInteractable = null;
		}

		public void KeepInteracts()
		{
			// CurrentInteractable 게이지 증가
		}

		public void TryInteract()
		{
			if (!Managers.Input.Control.Player.Interact.WasPressedThisFrame()) return;

			RaycastHit2D hit = Physics2D.Raycast(
				transform.position,
				prevDir,
				interactDistance,
				interactLayer);

			if (hit.collider == null) return;

			hit.collider.GetComponent<InteractableBase>()?.TryGetOwnershipServerRPC(OwnerClientId);
		}

		private void OnDrawGizmos()
		{
			Color prev = Gizmos.color;
			Gizmos.color = Color.red;

			Vector3 start = transform.position;
			Vector3 end = start + (Vector3)(prevDir.normalized * interactDistance);

			Gizmos.DrawLine(start, end);
			Gizmos.DrawSphere(end, 0.08f);

			Gizmos.color = prev;
		}
	}
}
