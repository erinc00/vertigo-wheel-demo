using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.Core;

namespace Vertigo.UI
{
    public sealed class RewardPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject ui_panel_reward;
        [SerializeField] private Image ui_image_reward_value;
        [SerializeField] private TMP_Text ui_text_reward_value;
        [SerializeField] private RectTransform animContent;
        [SerializeField] private Image ui_image_reward_flash;

        private Tween _flashTween;

        public void Show(RewardInstance reward, float seconds, Action onClosed)
        {
            if (ui_panel_reward != null) ui_panel_reward.SetActive(true);
            if (ui_image_reward_value != null)
            {
                ui_image_reward_value.sprite = reward.Icon;
                ui_image_reward_value.preserveAspect = true;
                ui_image_reward_value.enabled = reward.Icon != null;
            }
            if (ui_text_reward_value != null)
                ui_text_reward_value.text = $"+{reward.Amount} {reward.DisplayName}";

            if (animContent != null)
            {
                animContent.localScale = Vector3.zero;
                animContent.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
            }

            PlayFlash();

            StopAllCoroutines();
            StartCoroutine(CloseAfter(seconds, onClosed));
        }

        private void PlayFlash()
        {
            if (ui_image_reward_flash == null) return;

            _flashTween?.Kill();
            var c = ui_image_reward_flash.color;
            ui_image_reward_flash.color = new Color(c.r, c.g, c.b, 1f);
            ui_image_reward_flash.rectTransform.localRotation = Quaternion.identity;

            _flashTween = DOTween.Sequence()
                .Join(ui_image_reward_flash.DOFade(0f, 0.6f).SetEase(Ease.OutQuad))
                .Join(ui_image_reward_flash.rectTransform.DORotate(new Vector3(0f, 0f, 40f), 0.6f, RotateMode.LocalAxisAdd));
        }

        private void OnDisable() => _flashTween?.Kill();

        private IEnumerator CloseAfter(float seconds, Action onClosed)
        {
            yield return new WaitForSeconds(seconds);
            if (ui_panel_reward != null) ui_panel_reward.SetActive(false);
            onClosed?.Invoke();
        }
    }
}