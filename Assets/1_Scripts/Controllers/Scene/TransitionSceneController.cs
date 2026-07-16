using MeowTruck.Manager;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace MeowTruck.Controllers
{
	public class TransitionSceneController : MonoBehaviour
    {
		private void Start()
		{
            StartCoroutine(StartTransition());
		}

		private IEnumerator StartTransition()
        {
            // ... Loading Animation

            yield return null;

            string nextScene = Managers.Scene.NextScene;

			if (NetworkManager.Singleton != null && Unity.Netcode.NetworkManager.Singleton.IsListening)
            {
				if (!NetworkManager.Singleton.IsHost)
				{
					yield break;
				}

				NetworkManager.Singleton.SceneManager.LoadScene(
					nextScene,
					LoadSceneMode.Single);
            }
            else
            {
				SceneManager.LoadSceneAsync(
                    nextScene,
					LoadSceneMode.Single);
            }
		}
	}
}