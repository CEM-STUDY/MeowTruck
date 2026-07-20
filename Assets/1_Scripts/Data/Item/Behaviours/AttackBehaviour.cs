using MeowTruck.Controllers;
using UnityEngine;

namespace MeowTruck.Data
{
	public abstract class AttackBehaviour : ItemUseBehaviour
	{
		public override bool Use(PlayerController controller, ItemData data)
		{
			controller.Attack();
			return true;
		}

		public abstract bool Attack(PlayerController controller, Vector2 dir, LayerMask targetLayer);
	}
}