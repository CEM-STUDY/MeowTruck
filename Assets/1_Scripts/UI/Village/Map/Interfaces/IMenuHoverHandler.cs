namespace MeowTruck.UI
{
    public interface IMenuHoverHandler
    {
        void RequestFavoriteMenuHover(int cityIndex, bool active);
        bool GetTravelMode();
    }
}
