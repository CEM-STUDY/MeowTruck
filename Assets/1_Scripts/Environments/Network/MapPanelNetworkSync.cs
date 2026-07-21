using System;
using Unity.Netcode;

namespace MeowTruck.Environments
{
    public class MapPanelNetworkSync : NetworkBehaviour
    {
        public event Action<int, bool> MenuHoverChanged;

        public void RequestFavoriteMenuHover(int cityIndex, bool active)
        {
            if (!IsHost) return;

            SetMenuHoverClientRpc(cityIndex, active);
        }

        [ClientRpc]
        private void SetMenuHoverClientRpc(int cityIndex, bool active)
        {
            MenuHoverChanged?.Invoke(cityIndex, active);
        }
    }
}
