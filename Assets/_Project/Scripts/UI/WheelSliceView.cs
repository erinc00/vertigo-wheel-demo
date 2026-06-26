using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.UI
{
    [RequireComponent(typeof(Image))]
    public sealed class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text amountText;

        public void SetIcon(Sprite icon)
        {
            EnsureRef();
            iconImage.sprite = icon;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            iconImage.enabled = icon != null;
        }

        public void SetAmountText(string text)
        {
            if (amountText == null) return;
            bool hasText = !string.IsNullOrEmpty(text);
            amountText.gameObject.SetActive(hasText);
            if (hasText) amountText.text = text;
        }

        private void EnsureRef()
        {
            if (iconImage == null) iconImage = GetComponent<Image>();
        }

#if UNITY_EDITOR
        private void OnValidate() => EnsureRef();
#endif
    }
}