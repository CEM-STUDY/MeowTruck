using Unity.Netcode;
using UnityEngine;

namespace MeowTruck.Environments
{
    public class NetworkDaySpawner : MonoBehaviour
    {
        [SerializeField] private NetworkObject networkDayPrefab;

        private NetworkManager networkManager;

        private void Start()
        {
            networkManager = NetworkManager.Singleton;

            if (networkManager == null)
            {
                Debug.LogError("[NetworkDaySpawner] NetworkManager was not found.", this);
                return;
            }

            if (networkManager.IsServer)
                SpawnNetworkDay();
        }

        private void SpawnNetworkDay()
        {
            if (!networkManager.IsServer || NetworkDay.Instance != null)
                return;

            if (networkDayPrefab == null)
            {
                Debug.LogError("[NetworkDaySpawner] NetworkDay prefab is not assigned.", this);
                return;
            }

            if (networkDayPrefab.GetComponent<NetworkDay>() == null)
            {
                Debug.LogError("[NetworkDaySpawner] The assigned prefab needs a NetworkDay component.", this);
                return;
            }

            NetworkObject instance = Instantiate(networkDayPrefab);

            // This NetworkObject survives NGO scene changes on the server and clients.
            instance.Spawn(destroyWithScene: false);
        }
    }
}