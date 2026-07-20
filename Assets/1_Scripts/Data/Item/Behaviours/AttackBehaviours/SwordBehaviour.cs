using MeowTruck.Controllers;
using MeowTruck.Entities;
using System.Linq;
using UnityEngine;

namespace MeowTruck.Data
{
	[CreateAssetMenu(fileName = "Sword Behaviour", menuName = "Behaviour/Attack/Sword")]
	public class SwordBehaviour : AttackBehaviour
	{
		[SerializeField] private float damage;
		[SerializeField] private float distance;
		[SerializeField] private Vector2 boxSize;

		public float Distance => distance;
		public Vector2 BoxSize => boxSize;

		public override bool Attack(PlayerController controller, Vector2 dir, LayerMask targetLayer)
		{
			if (dir == Vector2.zero)
				return false;

			dir.Normalize();

			Vector2 center = (Vector2)controller.transform.position + dir * distance;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

			Collider2D[] hits = Physics2D.OverlapBoxAll(
				center,
				boxSize,
				angle,
				targetLayer);

			bool hit = false;

			foreach (Collider2D col in hits)
			{
				if (col.TryGetComponent<IDamageable>(out var damageable))
				{
					damageable.TakeDamage(damage);
					hit = true;
				}
			}

			return hit;
		}

	}
}
