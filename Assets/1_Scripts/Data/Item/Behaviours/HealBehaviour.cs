using MeowTruck.Controllers;
using UnityEngine;

namespace MeowTruck.Data
{
	[CreateAssetMenu(fileName = "Heal Behaviour", menuName = "Behaviour/Heal")]
	public class HealBehaviour : ItemUseBehaviour
	{
		public override bool Use(PlayerController controller, ItemData data)
		{
			Debug.Log("HEAL!");
			return true;
		}
	}
}