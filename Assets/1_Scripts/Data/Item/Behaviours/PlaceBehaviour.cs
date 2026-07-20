using MeowTruck.Controllers;
using UnityEngine;

namespace MeowTruck.Data
{
	[CreateAssetMenu(fileName = "Place Behaviour", menuName = "Behaviour/Place")]
	public class PlaceBehaviour : ItemUseBehaviour
    {
		public override bool Use(PlayerController controller, ItemData data)
		{
			throw new global::System.NotImplementedException();
		}
	}
}