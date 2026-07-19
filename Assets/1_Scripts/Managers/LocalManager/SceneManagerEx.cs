using UnityEngine;
using UnityEngine.SceneManagement;

namespace MeowTruck.Manager
{
	public class SceneManagerEx
	{
		public string NextScene { get; private set; }

		public void ChangeScene(string sceneName)
		{
			NextScene = sceneName;

			if (Unity.Netcode.NetworkManager.Singleton != null && Unity.Netcode.NetworkManager.Singleton.IsListening)
			{
				if (!Unity.Netcode.NetworkManager.Singleton.IsHost)
				{
					Debug.LogWarning("[SceneManager] - Change NetworkScene from client");
					return;
				}

				Unity.Netcode.NetworkManager.Singleton.SceneManager.LoadScene(
					Constants.SCENE_TRANSITION,
					LoadSceneMode.Single);
			}
			else
			{
				SceneManager.LoadSceneAsync(
					Constants.SCENE_TRANSITION,
					LoadSceneMode.Single);
			}
		}
	}
}