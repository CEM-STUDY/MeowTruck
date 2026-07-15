using Cysharp.Threading.Tasks;
using MeowTruck.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace MeowTruck.UI
{
	public class MainUI : MonoBehaviour
	{
		[SerializeField] private Button hostButton;
		[SerializeField] private Button clientButton;

		private async void OnEnable()
		{
			hostButton.onClick.AddListener(() => { StartLobby().Forget(); });
			clientButton.onClick.AddListener(async () =>
			{
				Debug.Log("CLICK");
				GameNetworkManager.Instance.FindLobbiesWithCallback((lobbies) => GameNetworkManager.Instance.JoinLobby(lobbies[0]));
			});
		}

		private async UniTaskVoid StartLobby()
		{
			GameNetworkManager.Instance.StartHost(); 
			Managers.Scene.ChangeScene(Constants.SCENE_LOBBY);
		}

		private void OnDisable()
		{

		}
	}
}