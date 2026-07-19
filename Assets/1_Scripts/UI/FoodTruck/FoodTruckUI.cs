using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FoodTruckUI : MonoBehaviour
{
    [SerializeField] private Button startButton;


    private void Awake()
    {
        Debug.Log("Ready for Food Truck");
    }

    private void OnEnable()
    {
        bool isHost = NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost;

        startButton.gameObject.SetActive(isHost);

        if (isHost)
        {
            startButton.interactable = NetworkDay.Instance != null &&
                !NetworkDay.Instance.IsRunning.Value;
            startButton.onClick.AddListener(StartFoodTruck);
        }
    }

    private void StartFoodTruck()
    {
        if (NetworkDay.Instance == null)
        {
            Debug.LogError("[FoodTruckUI] NetworkDay has not been spawned.");
            return;
        }

        if (!NetworkDay.Instance.StartFoodTruckPeriod())
            return;

        startButton.gameObject.SetActive(false);
        Debug.Log("[FoodTruckUI] Food truck time started.");
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(StartFoodTruck);
    }
}
