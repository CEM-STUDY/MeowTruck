using MeowTruck.Data;
using MeowTruck.Items;
using MeowTruck.Manager;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Misc
{
	// HACK - Test용 스크립트입니다.
	public class ItemSpawner : NetworkBehaviour
	{
		[SerializeField] private GameObject itemPrefab;

		public static ItemSpawner Instance { get; private set; } = null;
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			Managers.Inventory.OnDropItem += SpawnItemServerRPC;
		}

		private void Update()
		{
			if (!IsHost) return;
			
			if(Input.GetKeyDown(KeyCode.R))
			{
				var obj = Instantiate(itemPrefab);
				obj.GetComponent<NetworkObject>().Spawn();
				obj.GetComponent<DropItem>().InitDropItem(Random.Range(1, 4), 60);
			}

			if (Input.GetKeyDown(KeyCode.T))
			{
				SpawnEntity(transform.position, Random.Range(1, 4));
			}
		}

		// HACK - NetworkManager, NetworkTransmission으로 옮기기
		[Rpc(SendTo.Server)]
		public void SpawnItemServerRPC(Vector3 position, int itemId, int count)
		{
			SpawnItem(position, itemId, count);
		}

		public void SpawnItem(Vector3 position, int itemId, int count)
		{
			var obj = Instantiate(itemPrefab, position, Quaternion.identity);
			obj.GetComponent<NetworkObject>().Spawn();
			obj.GetComponent<DropItem>().InitDropItem(itemId, count);
		}

		[Rpc(SendTo.Server)]
		public void SpawnEntityServerRPC(Vector3 position, int entityId)
		{
			SpawnEntity(position, entityId);
		}

		public void SpawnEntity(Vector3 position, int entityId)
		{
			EntityData entityData = Managers.Resource.GetEntityData(entityId);
			var obj = Instantiate(entityData.EntityPrefab, position, Quaternion.identity);
			obj.GetComponent<NetworkObject>().Spawn();
		}
	}
}
