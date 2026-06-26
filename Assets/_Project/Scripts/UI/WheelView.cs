using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.Core;

namespace Vertigo.UI
{
    public sealed class WheelView : MonoBehaviour
    {
        [SerializeField] private RectTransform rotatingWheel;
        [SerializeField] private Button spinButton;

        [SerializeField] private Image baseImage;
        [SerializeField] private Image indicatorImage;
        [SerializeField] private WheelSliceView slicePrefab;
        [SerializeField] private GameConfig gameConfig;

        [SerializeField, Min(10f)] private float sliceRadius = 150f;

        private readonly System.Collections.Generic.List<WheelSliceView> _slices = new();
        private Tween _spinTween;
        private int _sliceCount;

        public event Action SpinClicked;

        private void OnEnable()
        {
            if (spinButton != null) spinButton.onClick.AddListener(RaiseSpin);
        }

        private void OnDisable()
        {
            if (spinButton != null) spinButton.onClick.RemoveListener(RaiseSpin);
            _spinTween?.Kill();
        }

        private void RaiseSpin() => SpinClicked?.Invoke();

        public void SetSpinInteractable(bool value)
        {
            if (spinButton != null) spinButton.interactable = value;
        }

        public void Build(WheelConfig config, int zone, RewardScaler scaler, RewardData[] resolvedRewards)
        {
            if (config == null) return;

            if (baseImage != null)
            {
                baseImage.sprite = config.WheelBaseSprite;
                baseImage.preserveAspect = true;
                baseImage.raycastTarget = false;
            }
            if (indicatorImage != null)
            {
                indicatorImage.sprite = config.IndicatorSprite;
                indicatorImage.preserveAspect = true;
                indicatorImage.raycastTarget = false;
            }

            _sliceCount = config.SliceCount;
            EnsureSlicePool(_sliceCount);

            float sliceAngle = 360f / Mathf.Max(1, _sliceCount);
            for (int i = 0; i < _slices.Count; i++)
            {
                bool active = i < _sliceCount;
                _slices[i].gameObject.SetActive(active);
                if (!active) continue;

                float theta = i * sliceAngle * Mathf.Deg2Rad;
                var rt = (RectTransform)_slices[i].transform;
                rt.anchoredPosition = new Vector2(Mathf.Sin(theta), Mathf.Cos(theta)) * sliceRadius;
                rt.localRotation = Quaternion.identity;

                RewardData effective = (resolvedRewards != null && i < resolvedRewards.Length)
                    ? resolvedRewards[i]
                    : config.Slices[i].Reward;

                _slices[i].SetIcon(config.Slices[i].ResolveIcon(effective));
                _slices[i].SetAmountText(ResolveLabel(config.Slices[i], effective, zone, scaler));
            }

            if (rotatingWheel != null) rotatingWheel.localRotation = Quaternion.identity;
        }

        private static string ResolveLabel(WheelSliceData slice, RewardData effectiveReward, int zone, RewardScaler scaler)
        {
            if (slice.IsBomb) return string.Empty;
            if (!string.IsNullOrEmpty(slice.DisplayLabel)) return slice.DisplayLabel;
            if (effectiveReward == null || scaler == null) return string.Empty;

            RewardInstance instance = scaler.Create(effectiveReward, zone);
            return $"x{instance.Amount}";
        }

        public void PlaySpin(int sliceIndex, Action onComplete)
        {
            _spinTween?.Kill();

            if (rotatingWheel == null || gameConfig == null)
            {
                onComplete?.Invoke();
                return;
            }

            float sliceAngle = 360f / Mathf.Max(1, _sliceCount);
            float jitter = UnityEngine.Random.Range(-sliceAngle * 0.35f, sliceAngle * 0.35f);
            float currentZ = rotatingWheel.localEulerAngles.z;
            float desired = sliceIndex * sliceAngle + jitter;
            float delta = Mathf.Repeat(currentZ - desired, 360f);
            float by = -(gameConfig.SpinExtraTurns * 360f + delta);

            _spinTween = rotatingWheel
                .DOLocalRotate(new Vector3(0f, 0f, by), gameConfig.SpinDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutQuart)
                .OnComplete(() => onComplete?.Invoke());
        }

        private void EnsureSlicePool(int count)
        {
            if (slicePrefab == null || rotatingWheel == null) return;
            while (_slices.Count < count)
            {
                var s = Instantiate(slicePrefab, rotatingWheel);
                _slices.Add(s);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rotatingWheel == null)
            {
                var marker = GetComponentInChildren<RotatingWheelMarker>(true);
                if (marker != null) rotatingWheel = marker.GetComponent<RectTransform>();
            }
            if (spinButton == null)
            {
                var marker = GetComponentInChildren<SpinButtonMarker>(true);
                if (marker != null) spinButton = marker.GetComponent<Button>();
            }
        }
#endif
    }
}