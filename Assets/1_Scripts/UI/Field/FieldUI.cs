using MeowTruck.Manager;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MeowTruck.UI
{
    public class FieldUI : MonoBehaviour
    {
        [SerializeField] private Slider hpBar;
        [SerializeField] private Button exitButton;
        [SerializeField] private ConfirmPopup confirmPopup;

        private void Awake()
        {
            bool isHost = NetworkManager.Singleton.IsHost;

            if (NetworkManager.Singleton.IsHost)
            {
                exitButton.onClick.AddListener(OpenConfirmPopup);
            }

            exitButton.gameObject.SetActive(isHost);
        }

        private void OpenConfirmPopup()
        {
            confirmPopup.SetHeaderText("Are you sure exit the field?");
            confirmPopup.SetEvents(ExitField);
            confirmPopup.gameObject.SetActive(true);
        }
        
        private void ExitField()
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                Constants.SCENE_VILLAGE,
                LoadSceneMode.Single);
        }

        private void OnDestroy()
        {
            exitButton.onClick.RemoveListener(ExitField);
        }
    }
}

