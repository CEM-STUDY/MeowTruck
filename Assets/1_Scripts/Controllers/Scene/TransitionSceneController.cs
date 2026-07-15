using MeowTruck.Manager;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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

			if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
				if (!NetworkManager.Singleton.IsServer)
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