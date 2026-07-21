using MeowTruck.Entities;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck
{
	public class Arrow : NetworkBehaviour
	{
		private LayerMask targetMask;
		private Vector2 dir;
		private float distance;
		private float damage;
		private float speed;

		private float traveledDistance = 0f;

		private new Rigidbody2D rigidbody = null;

		public void Init(Vector2 dir, float distance, float speed, 
			LayerMask targetMask,
			float damage)
		{
			traveledDistance = 0f;

			this.distance = distance;
			this.dir = dir;
			this.targetMask = targetMask;
			this.damage = damage;
			this.speed = speed;

			rigidbody = GetComponent<Rigidbody2D>();
		}


		private void FixedUpdate()
		{
			float moveDistance = speed * Time.fixedDeltaTime;

			rigidbody.MovePosition(rigidbody.position + dir * moveDistance);
			traveledDistance += moveDistance;

			if (traveledDistance >= distance)
			{
				NetworkObject.Despawn(destroy: true);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!IsHost) return;

            int otherLayer = other.gameObject.layer;
			if ((targetMask.value & (1 << otherLayer)) == 0) return;
			if (!other.TryGetComponent(out IDamageable damagable)) return;

			damagable.TakeDamage(damage);
			NetworkObject.Despawn(destroy: true);
		}
	}
}
