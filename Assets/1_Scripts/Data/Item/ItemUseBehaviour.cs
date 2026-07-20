using MeowTruck.Controllers;
using UnityEngine;

namespace MeowTruck.Data
{
	/// <summary>
	/// 
	/// 아이템의 행동을 정의하는 ScriptableObject
	/// 
	/// </summ>
	public abstract class ItemUseBehaviour : ScriptableObject
	{
		public abstract bool Use(PlayerController controller, ItemData data);
	}
}