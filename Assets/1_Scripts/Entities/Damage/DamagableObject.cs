using MeowTruck.Data;
using MeowTruck.Misc;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Entities
{
	public abstract class DamageableObject : NetworkBehaviour, IDamageable
	{
		[SerializeField] protected EntityData entityData = null;

		protected NetworkVariable<float> currentHp = new(float.MaxValue);
		protected SpriteRenderer spriteRenderer;

		protected virtual void Awake()
		{
			if (entityData == null)
			{
				Debug.LogWarning("[DamagableObject] - EntityData is NULL");
				return;
			}
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			if (!IsHost) return;
			currentHp.Value = entityData.MaxHp;
		}

		public void TakeDamage(float damage)
		{
			Debug.Log("TAKE DAMAGE : " + damage);
			TakeDamageServerRPC(damage);
		}

		[Rpc(SendTo.Server)]
		public void TakeDamageServerRPC(float damage)
		{
			currentHp.Value -= damage;

			if (currentHp.Value <= 0)
				Die();
		}

		protected virtual void Die()
		{
			DropItems();
			GetComponent<NetworkObject>().Despawn(destroy: true);
		}

		protected void DropItems()
		{
			// TODO - DropTable 참조
			ItemSpawner.Instance.SpawnItem(transform.position, 1, 1);
		}

	}
}
