using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.Core;

namespace Vertigo.UI
{
    public sealed class GameplayView : MonoBehaviour
    {
        [SerializeField] private Button collectButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private CanvasGroup interactionGroup;

        public event Action CollectClicked;
        public event Action MenuClicked;

        private void OnEnable()
        {
            if (collectButton != null) collectButton.onClick.AddListener(RaiseCollect);
            if (menuButton != null) menuButton.onClick.AddListener(RaiseMenu);
        }

        private void OnDisable()
        {
            if (collectButton != null) collectButton.onClick.RemoveListener(RaiseCollect);
            if (menuButton != null) menuButton.onClick.RemoveListener(RaiseMenu);
        }

        private void RaiseCollect() => CollectClicked?.Invoke();
        private void RaiseMenu() => MenuClicked?.Invoke();

        public void SetCollectVisible(bool visible)
        {
            if (collectButton != null) collectButton.gameObject.SetActive(visible);
        }

        public void SetInputEnabled(bool enabled)
        {
            if (interactionGroup != null)
            {
                interactionGroup.interactable = enabled;
                interactionGroup.blocksRaycasts = enabled;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (collectButton == null)
            {
                var marker = GetComponentInChildren<CollectButtonMarker>(true);
                if (marker != null) collectButton = marker.GetComponent<Button>();
            }
            if (menuButton == null)
            {
                var marker = GetComponentInChildren<MenuButtonMarker>(true);
                if (marker != null) menuButton = marker.GetComponent<Button>();
            }
        }
#endif
    }
}