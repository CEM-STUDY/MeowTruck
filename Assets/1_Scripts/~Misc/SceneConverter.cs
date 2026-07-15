using MeowTruck.Manager;
using UnityEngine;

namespace MeowTruck.Misc
{
	// Convert scene after others init.
	[DefaultExecutionOrder(100)]
	public class SceneConverter : MonoBehaviour
	{
		private void Start()
		{
			Managers.Scene.ChangeScene(Constants.SCENE_MAIN);
		}
	}
}