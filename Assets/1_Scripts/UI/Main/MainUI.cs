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
				GameNetworkManager.Lobby.FindLobbiesWithCallback((lobbies) => { GameNetworkManager.Lobby.JoinLobby(lobbies[0]); });
			});
		}

		private async UniTaskVoid StartLobby()
		{
			GameNetworkManager.Instance.StartHost(Constants.MAX_PLAYERS); 
			Managers.Scene.ChangeScene(Constants.SCENE_LOBBY);
		}

		private void OnDisable()
		{

		}
	}
}