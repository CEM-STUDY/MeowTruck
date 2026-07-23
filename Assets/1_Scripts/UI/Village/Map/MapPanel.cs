using MeowTruck.Manager;
using MeowTruck.Environments;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MeowTruck.UI
{
    public class MapPanel : MonoBehaviour, IMenuHoverHandler
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject cities;
        [SerializeField] private GameObject fields;
        [SerializeField] private GameObject favoriteMenuPrefab;
        [SerializeField] private ConfirmPopup confirmPopup;

        [SerializeField] private MapPanelNetworkSync networkSync;

        private Button[] cityButtons;
        private Button[] fieldButtons;
        private FavoriteMenuHover[] cityHovers;
        private string pendingSceneName;
        private bool isTravelMode;

        private void Awake()
        {
            AddEvents();

            confirmPopup.gameObject.SetActive(false);
        }

        public void SetTravelMode(bool enabled)
        {
            isTravelMode = enabled;
            pendingSceneName = null;
            confirmPopup.gameObject.SetActive(false);
        }

        #region interface implement
        public void RequestFavoriteMenuHover(int cityIndex, bool active)
        {
            if (networkSync == null)
                return;

            networkSync.RequestFavoriteMenuHover(cityIndex, active);
        }

        public bool GetTravelMode() => isTravelMode;
        #endregion

        private void OnMenuHoverChanged(int cityIndex, bool active)
        {
            if (cityHovers == null || cityIndex < 0 || cityIndex >= cityHovers.Length)
                return;

            cityHovers[cityIndex].SetMenuActive(active);
        }

        private void OpenConfirmPopup(bool isCity, int idx)
        {
            if (!isTravelMode || NetworkManager.Singleton == null || !NetworkManager.Singleton.IsHost) return;

            string text = isCity ?
                "Are you sure enter the city?" :
                "Are you sure enter the field?";

            pendingSceneName = isCity ?
                Constants.SCENE_FOOD_TRUCK :
                Constants.SCENE_FIELD;

            confirmPopup.SetHeaderText(text);
            confirmPopup.gameObject.SetActive(true);
        }

        private void ConfirmSceneChange()
        {
            if (string.IsNullOrEmpty(pendingSceneName))
            {
                return;
            }

            confirmPopup.ClosePopup();
            ClosePanel();

            Managers.Scene.ChangeScene(pendingSceneName);
        }

        private void ClosePanel()
        {
            if (isTravelMode)
            {
                if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsHost) return;

                MeetingPoint.RequestCloseTravelMap();
                return;
            }

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            DeleteEvents();
        }

        #region Add & Delete Events
        private void AddEvents()
        {
            closeButton.onClick.AddListener(ClosePanel);

            if (cities)
            {
                cityButtons = cities.GetComponentsInChildren<Button>(true);
                cityHovers = new FavoriteMenuHover[cityButtons.Length];

                for (int i = 0; i < cityButtons.Length; i++)
                {
                    FavoriteMenuHover hover = cityButtons[i].GetComponent<FavoriteMenuHover>();
                    int index = i;

                    if (!hover)
                    {
                        hover = cityButtons[i].gameObject.AddComponent<FavoriteMenuHover>();
                    }

                    hover.Initialize(this, favoriteMenuPrefab, index);
                    cityHovers[i] = hover;
                    cityButtons[i].onClick.AddListener(() => OpenConfirmPopup(true, index));
                }
            }

            if (fields)
            {
                fieldButtons = fields.GetComponentsInChildren<Button>(true);

                for (int i = 0; i < fieldButtons.Length; i++)
                {
                    int index = i;
                    fieldButtons[i].onClick.AddListener(() => OpenConfirmPopup(false, index));
                }
            }

            confirmPopup.SetEvents(ConfirmSceneChange);

            if (networkSync != null)
                networkSync.MenuHoverChanged += OnMenuHoverChanged;
        }

        private void DeleteEvents()
        {
            closeButton.onClick.RemoveListener(ClosePanel);

            for (int i = 0; i < cityButtons.Length; i++)
            {
                if (cityButtons[i] != null)
                    cityButtons[i].onClick.RemoveAllListeners();
            }


            for (int i = 0; i < fieldButtons.Length; i++)
            {
                fieldButtons[i].onClick.RemoveAllListeners();
            }

            if (networkSync != null)
                networkSync.MenuHoverChanged -= OnMenuHoverChanged;
        }
        #endregion
    }

    public class FavoriteMenuHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private IMenuHoverHandler hoverHandler;
        private GameObject favoriteMenuPrefab;
        private GameObject favoriteMenu;
        private int cityIndex;

        public void Initialize(IMenuHoverHandler handler, GameObject prefab, int index)
        {
            hoverHandler = handler;
            favoriteMenuPrefab = prefab;
            cityIndex = index;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!hoverHandler.GetTravelMode()) return;

            hoverHandler?.RequestFavoriteMenuHover(cityIndex, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!hoverHandler.GetTravelMode()) return;

            hoverHandler?.RequestFavoriteMenuHover(cityIndex, false);
        }

        public void SetMenuActive(bool active)
        {
            if (!favoriteMenuPrefab)
                return;

            if (!favoriteMenu)
            {
                favoriteMenu = Instantiate(favoriteMenuPrefab, transform);
                favoriteMenu.transform.localPosition = Vector2.up * 130;
            }

            favoriteMenu.SetActive(active);
        }
    }
}

