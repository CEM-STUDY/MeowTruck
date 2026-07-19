using UnityEngine;
using UnityEngine.UI;

namespace MeowTruck.UI
{
    public class VillageUI : MonoBehaviour
    {
        [SerializeField] private Button mapButton;
        [SerializeField] private MapPanel mapPanel;

        private void Awake()
        {
            mapButton.onClick.AddListener(OpenMapPanel);

            mapPanel.gameObject.SetActive(false);
        }

        private void OpenMapPanel()
        {
            mapPanel.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            mapButton.onClick.RemoveListener(OpenMapPanel);
        }
    }
}

