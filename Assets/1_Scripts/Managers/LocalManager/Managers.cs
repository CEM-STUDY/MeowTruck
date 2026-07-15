using UnityEngine;

namespace MeowTruck.Manager
{
	public class Managers : MonoBehaviour
	{
		private static Managers instance = null;
		public static Managers Instance { get { return instance; } }

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}


		private static SceneManagerEx scene = new SceneManagerEx();

		public static SceneManagerEx Scene => scene;

	}
}