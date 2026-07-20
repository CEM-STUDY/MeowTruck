using MeowTruck.Interactables;
using MeowTruck.Items;
using MeowTruck.Manager;
using UnityEngine;

namespace MeowTruck.Controllers
{
	/// <summary>
	/// 
	/// Player의 상호작용 관련 스크립트
	/// 
	/// </summary>
	public partial class PlayerController
	{

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

		public void TryPickUpItem()
		{
			if (!Managers.Input.Control.Player.Interact.WasPressedThisFrame()) return;

			Collider2D[] hits = Physics2D.OverlapCircleAll(
				transform.position + itemDetectOffset.ToVec3(),
				itemDetectRange,
				itemLayer);

			if (hits.Length == 0)
				return;

			DropItem nearestItem = null;
			float nearestDistance = float.MaxValue;

			foreach (Collider2D hit in hits)
			{
				DropItem item = hit.GetComponent<DropItem>();
				if (item == null)
					continue;

				float sqrDistance = (item.transform.position - transform.position).sqrMagnitude;

				if (sqrDistance < nearestDistance)
				{
					nearestDistance = sqrDistance;
					nearestItem = item;
				}
			}

			if (nearestItem == null)
				return;

			nearestItem.TryGetItem();
			Managers.Input.Control.Player.Interact.Reset();
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

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position + itemDetectOffset.ToVec3(), itemDetectRange);

			Gizmos.color = prev;
		}
	}
}
