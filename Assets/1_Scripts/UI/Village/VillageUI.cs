using UnityEngine;
using UnityEngine.UI;
using MeowTruck.Environments;

namespace MeowTruck.UI
{
    public class VillageUI : MonoBehaviour
    {
        [SerializeField] private Button mapButton;
        [SerializeField] private MapPanel mapPanel;

        private void Awake()
        {
            mapButton.onClick.AddListener(OpenMapPanel);
            MeetingPoint.TravelMapOpened += OpenTravelMapPanel;
            MeetingPoint.TravelMapClosed += CloseTravelMapPanel;

            mapPanel.gameObject.SetActive(false);
        }

        private void OpenMapPanel()
        {
            mapPanel.SetTravelMode(false);
            mapPanel.gameObject.SetActive(true);
        }

        private void OpenTravelMapPanel()
        {
            mapPanel.SetTravelMode(true);
            mapPanel.gameObject.SetActive(true);
        }

        private void CloseTravelMapPanel()
        {
            mapPanel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            mapButton.onClick.RemoveListener(OpenMapPanel);
            MeetingPoint.TravelMapOpened -= OpenTravelMapPanel;
            MeetingPoint.TravelMapClosed -= CloseTravelMapPanel;
        }
    }
}

