using MeowTruck.Controllers;
using MeowTruck.Environments;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Manager
{
	public enum DungeonRoomType
	{
		Normal,
		Boss,
	}

	/// <summary>
	/// 
	/// Room들을 관리합니다.
	/// 
	/// - 해당하는 테마의 방을 로드/언로드
	/// - 어떤 방을 생성할지 결정 및 스폰
	/// 
	/// </summary>
	public class DungeonManager : NetworkBehaviour
	{
		private int dungeonIndex = 0;

		private GameObject[] dungeonRooms;
		private GameObject bossRoom;

		private DungeonRoom currentRoom;

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			dungeonRooms = Managers.Resource.LoadAll<GameObject>(Constants.PATH_DUNGEON);
			// bossRoom = Managers.Resource.Load<GameObject>(Constants.PATH_DUNGEON_BOSS + "TestBossRoom.prefab");

			if (!IsHost) return;

			currentRoom = Instantiate(dungeonRooms[0], Vector3.zero, Quaternion.identity)
				.GetComponent<DungeonRoom>();
			currentRoom.GetComponent<NetworkObject>().Spawn();
			
			currentRoom.SetNextRoom((endPoint) => {
				OnSetNextRoom(dungeonIndex, endPoint);
			});

			foreach (var client in NetworkManager.ConnectedClients)
			{
				client.Value.PlayerObject.GetComponent<PlayerController>().SetPosition(currentRoom.StartPoint.transform.position);
			}

			// DUNGEON READY
			Debug.Log("DUNGEON READY");
		}


		public override void OnNetworkDespawn()
		{
			base.OnNetworkDespawn();
		}

		private void OnSetNextRoom(int dungeonIndex, MeetingPoint endPoint)
		{
			GameObject nextRoomPrefab = GetRoom(dungeonIndex);
			Vector3 spawnPos = nextRoomPrefab.GetComponent<DungeonRoom>().GetSuitableSpawnPosition(endPoint.transform.position);
			GameObject nextRoom = Instantiate(nextRoomPrefab, spawnPos, Quaternion.identity);
			nextRoom.GetComponent<NetworkObject>().Spawn();

			currentRoom.FadeOutClientRPC();
			currentRoom = nextRoom.GetComponent<DungeonRoom>();

			int t = ++dungeonIndex;
			currentRoom.SetNextRoom((endPoint) => {
				OnSetNextRoom(t, endPoint);
			});
		}

		// dungeonIndex에 맞는 Room 반환
		private GameObject GetRoom(int dungeonIndex)
		{
			return dungeonRooms[1]; // HACK
		}
	}
}
