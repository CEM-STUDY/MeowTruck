using MeowTruck.Controllers;
using UnityEngine;

namespace MeowTruck.Data
{
	public abstract class AttackBehaviour : ItemUseBehaviour
	{
		[SerializeField] private int maxCombo;
		[SerializeField] private int animWeaponIndex;

		public int MaxCombo => maxCombo;
		public int WeaponIndex => animWeaponIndex;

		public override bool Use(PlayerController controller, ItemData data)
		{
			controller.Attack();
			return true;
		}

		public abstract void Attack(PlayerController controller, Vector2 dir, LayerMask targetLayer);
	}
}