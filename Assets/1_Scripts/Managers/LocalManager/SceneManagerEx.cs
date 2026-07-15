using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace MeowTruck.Manager
{
	public class SceneManagerEx
	{
		public string NextScene { get; private set; }

		public void ChangeScene(string sceneName)
		{
			NextScene = sceneName;

			if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
			{
				if (!NetworkManager.Singleton.IsHost)
				{
					Debug.LogWarning("[SceneManager] - Change NetworkScene from client");
					return;
				}

				NetworkManager.Singleton.SceneManager.LoadScene(
					Constants.SCENE_TRANSITION,
					LoadSceneMode.Single);
			}
			else
			{
				SceneManager.LoadScene(
					Constants.SCENE_TRANSITION,
					LoadSceneMode.Single);
			}
		}
	}
}