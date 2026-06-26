using UnityEditor;
using UnityEngine;
using Vertigo.Core;

namespace Vertigo.EditorTools
{
    [CustomEditor(typeof(WheelConfig))]
    public sealed class WheelConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var cfg = (WheelConfig)target;
            int bombs = 0;
            for (int i = 0; i < cfg.Slices.Count; i++)
                if (cfg.Slices[i] != null && cfg.Slices[i].IsBomb) bombs++;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Özet", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Dilim sayısı: {cfg.SliceCount}");
            EditorGUILayout.LabelField($"Bomba sayısı: {bombs}");

            bool ok = cfg.WheelType == ZoneType.Normal ? bombs == 1 : bombs == 0;
            if (!ok)
            {
                EditorGUILayout.HelpBox(
                    cfg.WheelType == ZoneType.Normal
                        ? "Normal çarkta tam 1 bomba olmalı."
                        : $"{cfg.WheelType} çarkta bomba OLAMAZ.",
                    MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("Çark kurulumu geçerli.", MessageType.Info);
            }
        }
    }
}
