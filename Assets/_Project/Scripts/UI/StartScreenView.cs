using System;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.UI
{
    public sealed class StartScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button startButton;

        public event Action StartClicked;

        private void OnEnable()
        {
            if (startButton != null) startButton.onClick.AddListener(RaiseStart);
        }

        private void OnDisable()
        {
            if (startButton != null) startButton.onClick.RemoveListener(RaiseStart);
        }

        private void RaiseStart() => StartClicked?.Invoke();

        public void Show()
        {
            if (panel != null) panel.SetActive(true);
        }

        public void Hide()
        {
            if (panel != null) panel.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (startButton == null)
            {
                var marker = GetComponentInChildren<StartButtonMarker>(true);
                if (marker != null) startButton = marker.GetComponent<Button>();
            }
        }
#endif
    }
}