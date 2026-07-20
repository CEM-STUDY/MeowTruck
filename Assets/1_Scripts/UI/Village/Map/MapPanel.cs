using MeowTruck.Manager;
using MeowTruck.Environments;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MeowTruck.UI
{
    public class MapPanel : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject cities;
        [SerializeField] private GameObject fields;
        [SerializeField] private GameObject favoriteMenuPrefab;
        [SerializeField] private ConfirmPopup confirmPopup;

        private Button[] cityButtons;
        private Button[] fieldButtons;
        private string pendingSceneName;
        private bool isTravelMode;

        public void SetTravelMode(bool enabled)
        {
            isTravelMode = enabled;
            pendingSceneName = null;
            confirmPopup.gameObject.SetActive(false);
        }

        private void Awake()
        {
            AddEvents();

            confirmPopup.gameObject.SetActive(false);
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

                for (int i = 0; i < cityButtons.Length; i++)
                {
                    FavoriteMenuHover hover = cityButtons[i].GetComponent<FavoriteMenuHover>();
                    int index = i;

                    if (!hover)
                    {
                        hover = cityButtons[i].gameObject.AddComponent<FavoriteMenuHover>();
                    }

                    hover.Initialize(favoriteMenuPrefab);
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
        }

        private void DeleteEvents()
        {
            closeButton.onClick.RemoveListener(ClosePanel);
        }
        #endregion
    }

    public class FavoriteMenuHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private GameObject favoriteMenuPrefab;
        private GameObject favoriteMenu;

        public void Initialize(GameObject prefab)
        {
            favoriteMenuPrefab = prefab;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!favoriteMenuPrefab)
            {
                return;
            }

            if (!favoriteMenu)
            {
                favoriteMenu = Instantiate(favoriteMenuPrefab, transform);
                favoriteMenu.transform.localPosition = Vector2.up * 130;
            }

            favoriteMenu.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (favoriteMenu)
            {
                favoriteMenu.SetActive(false);
            }
        }
    }
}

