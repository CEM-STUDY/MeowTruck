using MeowTruck.Controllers;
using UnityEngine;

namespace MeowTruck.Data
{
	/// <summary>
	/// 
	/// 아이템의 행동을 정의하는 ScriptableObject
	/// State를 가지지 않는 순수한 로직 스크립트
	/// 
	/// </summary>
	public abstract class ItemUseBehaviour : ScriptableObject
	{
		public abstract bool Use(PlayerController controller, ItemData data);
	}
}