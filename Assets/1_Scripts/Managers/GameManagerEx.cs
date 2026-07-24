using UnityEngine;

namespace MeowTruck.Manager
{
	public class GameManagerEx : MonoBehaviour
	{
		public static GameManagerEx Instance { get; private set; } = null;
		
		
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(this);
			}
		}

		[ContextMenu("ToVillage")]
		public void ToVillage()
		{
			Managers.Scene.ChangeScene(Constants.SCENE_VILLAGE);	
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}