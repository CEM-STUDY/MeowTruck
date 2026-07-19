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
        bool isHost = NetworkManager.Singleton.IsHost;

        startButton.gameObject.SetActive(isHost);

        if (isHost)
            startButton.onClick.AddListener(StartFoodTruck);
    }

    private void StartFoodTruck()
    {
        Debug.Log("Start Food Truck!");
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(StartFoodTruck);
    }
}
