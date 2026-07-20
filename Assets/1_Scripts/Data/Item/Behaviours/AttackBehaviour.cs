using MeowTruck.Controllers;
using UnityEngine;

namespace MeowTruck.Data
{
	[CreateAssetMenu(fileName = "Attack Behaviour", menuName = "Behaviour/Attack")]
	public class AttackBehaviour : ItemUseBehaviour
	{
		public override bool Use(PlayerController controller, ItemData data)
		{
			controller.Attack(data);
			return true;
		}
	}
}