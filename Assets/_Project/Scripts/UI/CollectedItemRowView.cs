using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.UI
{
    public sealed class CollectedItemRowView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text amountText;

        public void Set(Sprite sprite, string text)
        {
            if (icon != null)
            {
                icon.sprite = sprite;
                icon.enabled = sprite != null;
                icon.preserveAspect = true;
                icon.raycastTarget = false;
            }
            if (amountText != null) amountText.text = text;
        }
    }
}