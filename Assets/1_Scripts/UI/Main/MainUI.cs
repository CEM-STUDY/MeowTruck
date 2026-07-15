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
			clientButton.onClick.AddListener(() =>
			{
				GameNetworkManager.instance.FindLobbiesWithCallback((lobbies) => GameNetworkManager.instance.JoinLobby(lobbies[0]));
			});
		}

		private async UniTaskVoid StartLobby()
		{
			await GameNetworkManager.instance.StartHost(Constants.MAX_PLAYERS); 
			Managers.Scene.ChangeScene(Constants.SCENE_LOBBY);
		}

		private void OnDisable()
		{

		}
	}
}