using MeowTruck.Items;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Misc
{
	// HACK - Test용 스크립트입니다.
	public class ItemSpawner : NetworkBehaviour
	{
		[SerializeField] private GameObject itemPrefab;

		private void Update()
		{
			if (IsHost && Input.GetKeyDown(KeyCode.R))
			{
				var obj = Instantiate(itemPrefab);
				obj.GetComponent<NetworkObject>().Spawn();
				obj.GetComponent<DropItem>().InitDropItem(Random.Range(1, 4), Random.Range(2, 10));
			}
		}
	}
}
