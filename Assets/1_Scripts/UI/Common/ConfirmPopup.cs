using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MeowTruck.UI
{
    public class ConfirmPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button closeButton;

        private Action onConfirm;
        private Action onCancel;

        private void Awake()
        {
            if(confirmButton) confirmButton.onClick.AddListener(OnClickConfirm);
            if(cancelButton) cancelButton.onClick.AddListener(OnClickCancel);
            if(closeButton) closeButton.onClick.AddListener(ClosePopup);
        }

        private void OnClickConfirm()
        {
            onConfirm?.Invoke();
        }

        private void OnClickCancel()
        {
            if(onCancel != null)
            {
                onCancel?.Invoke();
            }

            ClosePopup();
        }


        public void SetHeaderText(string text)
        {
            if(headerText != null)
            {
                headerText.text = text;
            }
        }

        public void SetEvents(Action confirm, Action cancel = null)
        {
            if(confirm != null)
            {
                onConfirm = confirm;
            }
            if(cancel != null)
            {
                onCancel = cancel;
            }
        }

        public void ClosePopup()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if(confirmButton) confirmButton.onClick.RemoveListener(OnClickConfirm);
            if(cancelButton) cancelButton.onClick.RemoveListener(OnClickCancel);
            if(closeButton) closeButton.onClick.RemoveListener(ClosePopup);
        }
    }
}

