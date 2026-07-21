using MeowTruck.Controllers;
using MeowTruck.Entities;
using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Data
{
	[CreateAssetMenu(fileName = "Bow Behaviour", menuName = "Behaviour/Attack/Bow")]
	public class BowBehaviour : AttackBehaviour
	{
		[SerializeField] private float damage;
		[SerializeField] private float distance;
		[SerializeField] private float arrowSpeed;
		[SerializeField] private GameObject arrowPrefab;

		public float Distance => distance;

		public override void Attack(PlayerController controller, Vector2 dir, LayerMask targetLayer)
		{
			var arrowObj = Instantiate(arrowPrefab, controller.transform.position, dir.ToQuaternion());
			arrowObj.GetComponent<NetworkObject>().Spawn();
			arrowObj.GetComponent<Arrow>().Init(dir, distance, arrowSpeed, targetLayer, damage);

			return;
		}
	}
}
