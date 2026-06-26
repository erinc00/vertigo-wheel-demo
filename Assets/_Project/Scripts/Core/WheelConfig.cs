using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.Core
{
    [CreateAssetMenu(menuName = "Vertigo/Wheel Config", fileName = "wheel_")]
    public sealed class WheelConfig : ScriptableObject
    {
        [SerializeField] private ZoneType wheelType = ZoneType.Normal;
        [Tooltip("Çark tabanı: bronze (Normal) / silver (Safe) / golden (Super)")]
        [SerializeField] private Sprite wheelBaseSprite;
        [Tooltip("Üstteki sabit gösterge sprite'ı (zone tipine göre değişir).")]
        [SerializeField] private Sprite indicatorSprite;
        [SerializeField] private List<WheelSliceData> slices = new List<WheelSliceData>();

        public ZoneType WheelType => wheelType;
        public Sprite WheelBaseSprite => wheelBaseSprite;
        public Sprite IndicatorSprite => indicatorSprite;
        public IReadOnlyList<WheelSliceData> Slices => slices;
        public int SliceCount => slices.Count;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (slices == null || slices.Count < 2)
            {
                Debug.LogWarning($"[{name}] En az 2 dilim olmalı.", this);
                return;
            }

            int bombCount = 0;
            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i] == null) continue;
                if (slices[i].IsBomb) bombCount++;
                if (!slices[i].IsBomb && slices[i].Reward == null && slices[i].Pool == null)
                    Debug.LogWarning($"[{name}] Dilim {i}: bomba değil ama Reward ve Pool boş.", this);
            }

            if (wheelType == ZoneType.Normal && bombCount != 1)
                Debug.LogError($"[{name}] Normal çarkta tam olarak 1 bomba olmalı (şu an {bombCount}).", this);

            if (wheelType != ZoneType.Normal && bombCount > 0)
                Debug.LogError($"[{name}] {wheelType} çarkta bomba OLAMAZ (şu an {bombCount}).", this);

            if (wheelBaseSprite == null)
                Debug.LogWarning($"[{name}] Çark tabanı sprite'ı atanmamış.", this);
        }
#endif
    }
}
