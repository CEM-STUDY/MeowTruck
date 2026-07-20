using MeowTruck.Data;
using MeowTruck.Interactables;
using MeowTruck.Items;
using MeowTruck.Manager;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace MeowTruck.Controllers
{
	/// <summary>
	/// 
	/// Player의 디버그 관련 스크립트
	/// 무기 관련 로직이 길어질 것 같아 분리하였습니다.
	/// 
	/// </summary>
	public partial class PlayerController
	{
		private void OnDrawGizmos()
		{
			Color prev = Gizmos.color;
			Gizmos.color = Color.red;
			DrawInteractGizmos();

			Gizmos.color = Color.blue;
			DrawWeaponGizmos();

			Gizmos.color = prev;
		}

		private void DrawInteractGizmos()
		{
			Vector3 start = transform.position;
			Vector3 end = start + (Vector3)(currentDir.normalized * interactDistance);

			Gizmos.DrawLine(start, end);
			Gizmos.DrawSphere(end, 0.08f);

			Gizmos.DrawWireSphere(transform.position + itemDetectOffset.ToVec3(), itemDetectRange);
		}

		private void DrawWeaponGizmos()
		{
			var itemData = Managers.Resource.GetItemData(selectedItemId.Value);
			if (itemData == null || itemData.UseBehaviour == null) return;

			switch (itemData.UseBehaviour)
			{
				case SwordBehaviour sword:
					{
						Vector2 center = transform.position.ToVec2() + currentDir * sword.Distance;
						float angle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;

						Gizmos.matrix = Matrix4x4.TRS(
							center,
							Quaternion.Euler(0, 0, angle),
							Vector3.one);

						Gizmos.DrawWireCube(Vector3.zero, sword.BoxSize);
					}
					break;
			}

		}
	}
}
