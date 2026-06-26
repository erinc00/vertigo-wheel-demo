using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.Core;

namespace Vertigo.UI
{
    public sealed class ZoneStripView : MonoBehaviour
    {
        [SerializeField] private TMP_Text ui_text_zone_value;
        [SerializeField] private TMP_Text ui_text_zone_type_value;
        [SerializeField] private Image ui_image_zone_panel;

        [Header("Zone tipi panelleri")]
        [SerializeField] private Sprite normalPanel;
        [SerializeField] private Sprite safePanel;
        [SerializeField] private Sprite superPanel;

        public void Show(int zone, ZoneType type)
        {
            if (ui_text_zone_value != null) ui_text_zone_value.SetText("ZONE {0}", zone);
            if (ui_text_zone_type_value != null) ui_text_zone_type_value.text = type.ToString().ToUpperInvariant();

            if (ui_image_zone_panel != null)
            {
                ui_image_zone_panel.sprite = type switch
                {
                    ZoneType.Safe => safePanel,
                    ZoneType.Super => superPanel,
                    _ => normalPanel
                };
                ui_image_zone_panel.preserveAspect = true;
            }
        }
    }
}
