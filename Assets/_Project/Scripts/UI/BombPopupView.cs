using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.UI
{
    public sealed class BombPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject ui_panel_bomb;
        [SerializeField] private RectTransform animContent;
        [SerializeField] private Button giveUpButton;
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button reviveAdButton;
        [SerializeField] private TMP_Text ui_text_revive_cost_value;

        public event Action GiveUpClicked;
        public event Action ReviveClicked;
        public event Action ReviveAdClicked;

        private void OnEnable()
        {
            if (giveUpButton != null) giveUpButton.onClick.AddListener(RaiseGiveUp);
            if (reviveButton != null) reviveButton.onClick.AddListener(RaiseRevive);
            if (reviveAdButton != null) reviveAdButton.onClick.AddListener(RaiseReviveAd);
        }

        private void OnDisable()
        {
            if (giveUpButton != null) giveUpButton.onClick.RemoveListener(RaiseGiveUp);
            if (reviveButton != null) reviveButton.onClick.RemoveListener(RaiseRevive);
            if (reviveAdButton != null) reviveAdButton.onClick.RemoveListener(RaiseReviveAd);
        }

        private void RaiseGiveUp() { Hide(); GiveUpClicked?.Invoke(); }
        private void RaiseRevive() { ReviveClicked?.Invoke(); }
        private void RaiseReviveAd() { ReviveAdClicked?.Invoke(); }

        public void Show(long reviveCost, bool canAffordGold)
        {
            if (ui_panel_bomb != null) ui_panel_bomb.SetActive(true);
            if (ui_text_revive_cost_value != null) ui_text_revive_cost_value.SetText("{0}", reviveCost);
            if (reviveButton != null) reviveButton.interactable = canAffordGold;

            if (animContent != null)
                animContent.DOShakeAnchorPos(0.5f, 18f, 18, 90f);
        }

        public void Hide()
        {
            if (ui_panel_bomb != null) ui_panel_bomb.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (giveUpButton == null)
            {
                var m = GetComponentInChildren<GiveUpButtonMarker>(true);
                if (m != null) giveUpButton = m.GetComponent<Button>();
            }
            if (reviveButton == null)
            {
                var m = GetComponentInChildren<ReviveButtonMarker>(true);
                if (m != null) reviveButton = m.GetComponent<Button>();
            }
            if (reviveAdButton == null)
            {
                var m = GetComponentInChildren<ReviveAdButtonMarker>(true);
                if (m != null) reviveAdButton = m.GetComponent<Button>();
            }
        }
#endif
    }
}
