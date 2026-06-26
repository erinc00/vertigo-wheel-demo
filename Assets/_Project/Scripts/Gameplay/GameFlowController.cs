using System;
using UnityEngine;
using Vertigo.Core;
using Vertigo.UI;

namespace Vertigo.Gameplay
{
    public sealed class GameFlowController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private ZoneProgressionConfig progression;
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private RewardDatabase rewardDatabase;
        [SerializeField] private int randomSeed = 0;

        [Header("Views")]
        [SerializeField] private StartScreenView startScreenView;
        [SerializeField] private WheelView wheelView;
        [SerializeField] private GameplayView gameplayView;
        [SerializeField] private HudView hudView;
        [SerializeField] private ZoneStripView zoneStripView;
        [SerializeField] private RewardPopupView rewardPopup;
        [SerializeField] private BombPopupView bombPopup;

        private IRandomProvider _random;
        private ZoneRules _zoneRules;
        private IWheelResultSelector _selector;
        private RewardScaler _scaler;
        private RunState _runState;
        private Wallet _wallet;
        private GameStateMachine _stateMachine;

        private ZoneType _currentZoneType;
        private WheelConfig _currentWheel;
        private RewardData[] _resolvedSliceRewards;

        private void Awake()
        {
            Debug.Assert(progression != null, "ZoneProgressionConfig atanmamış.", this);
            Debug.Assert(gameConfig != null, "GameConfig atanmamış.", this);
            Debug.Assert(wheelView != null, "WheelView atanmamış.", this);

            _random = randomSeed == 0 ? new SystemRandomProvider() : new SystemRandomProvider(randomSeed);
            _zoneRules = new ZoneRules(progression.RuleSettings);
            _selector = new WeightedWheelResultSelector(_random);
            _scaler = new RewardScaler(progression);
            _runState = new RunState();
            _wallet = WalletRepository.Load(rewardDatabase, gameConfig.StartGold, gameConfig.StartCash);
            _stateMachine = new GameStateMachine();
        }

        private void OnEnable()
        {
            if (startScreenView != null) startScreenView.StartClicked += OnStartGameClicked;
            if (wheelView != null) wheelView.SpinClicked += OnSpinRequested;
            if (gameplayView != null)
            {
                gameplayView.CollectClicked += OnCollectRequested;
                gameplayView.MenuClicked += OnMenuRequested;
            }
            if (bombPopup != null)
            {
                bombPopup.GiveUpClicked += OnGiveUp;
                bombPopup.ReviveClicked += OnRevive;
                bombPopup.ReviveAdClicked += OnReviveAd;
            }
        }

        private void OnDisable()
        {
            if (startScreenView != null) startScreenView.StartClicked -= OnStartGameClicked;
            if (wheelView != null) wheelView.SpinClicked -= OnSpinRequested;
            if (gameplayView != null)
            {
                gameplayView.CollectClicked -= OnCollectRequested;
                gameplayView.MenuClicked -= OnMenuRequested;
            }
            if (bombPopup != null)
            {
                bombPopup.GiveUpClicked -= OnGiveUp;
                bombPopup.ReviveClicked -= OnRevive;
                bombPopup.ReviveAdClicked -= OnReviveAd;
            }
        }

        private void Start()
        {
            RefreshHud();
            gameplayView?.gameObject.SetActive(false);
            startScreenView?.Show();
        }

        private void OnStartGameClicked()
        {
            startScreenView?.Hide();
            gameplayView?.gameObject.SetActive(true);
            BeginRun();
        }

        private void OnMenuRequested()
        {

            if (_stateMachine.Current != GameState.WaitingForSpin) return;

            _runState.ClearCollected();
            RefreshHud();

            gameplayView?.gameObject.SetActive(false);
            startScreenView?.Show();
        }

        private void BeginRun()
        {
            _runState.Reset();
            RefreshHud();
            PrepareZone();
        }

        private void PrepareZone()
        {
            _stateMachine.ChangeState(GameState.PreparingZone);

            int zone = _runState.CurrentZone;
            _currentZoneType = _zoneRules.GetZoneType(zone);
            _currentWheel = progression.GetWheel(_currentZoneType, zone);
            _resolvedSliceRewards = ResolveSliceRewards(_currentWheel);

            wheelView.Build(_currentWheel, zone, _scaler, _resolvedSliceRewards);
            zoneStripView?.Show(zone, _currentZoneType);

            _stateMachine.ChangeState(GameState.WaitingForSpin);
            wheelView.SetSpinInteractable(true);
            gameplayView?.SetInputEnabled(true);
            gameplayView?.SetCollectVisible(_stateMachine.CanLeave(_currentZoneType));
        }

        private RewardData[] ResolveSliceRewards(WheelConfig config)
        {
            if (config == null) return Array.Empty<RewardData>();

            var resolved = new RewardData[config.SliceCount];
            for (int i = 0; i < config.SliceCount; i++)
            {
                var slice = config.Slices[i];
                resolved[i] = slice.Pool != null ? slice.Pool.PickRandom(_random) : slice.Reward;
            }
            return resolved;
        }

        private void OnSpinRequested()
        {
            if (!_stateMachine.CanSpin) return;

            _stateMachine.ChangeState(GameState.Spinning);
            wheelView.SetSpinInteractable(false);
            gameplayView?.SetInputEnabled(false);
            gameplayView?.SetCollectVisible(false);

            WheelResult result = _selector.Select(_currentWheel);
            wheelView.PlaySpin(result.SliceIndex, () => Resolve(result));
        }

        private void Resolve(WheelResult result)
        {
            if (result.IsBomb)
            {
                _stateMachine.ChangeState(GameState.BombHit);
                bombPopup?.Show(gameConfig.ReviveCost, _wallet.CanAfford(gameConfig.ReviveCost));
                return;
            }

            _stateMachine.ChangeState(GameState.ShowingReward);

            RewardData actualReward = result.Reward != null
                ? result.Reward
                : (_resolvedSliceRewards != null && result.SliceIndex < _resolvedSliceRewards.Length
                    ? _resolvedSliceRewards[result.SliceIndex]
                    : null);

            RewardInstance won = _scaler.Create(actualReward, _runState.CurrentZone);
            _runState.AddReward(won);
            RefreshHud();

            if (rewardPopup != null)
                rewardPopup.Show(won, gameConfig.RewardDisplaySeconds, AdvanceAfterReward);
            else
                AdvanceAfterReward();
        }

        private void AdvanceAfterReward()
        {
            _runState.AdvanceZone();
            PrepareZone();
        }

        private void OnCollectRequested()
        {
            if (!_stateMachine.CanLeave(_currentZoneType)) return;

            _wallet.Commit(_runState.Collected);
            RefreshHud();
            _stateMachine.ChangeState(GameState.RunCompleted);
            BeginRun();
        }

        private void OnGiveUp()
        {
            _runState.ClearCollected();
            BeginRun();
        }

        private void OnRevive()
        {
            if (!_wallet.SpendGold(gameConfig.ReviveCost)) return;
            RefreshHud();
            bombPopup?.Hide();
            AdvanceAfterReward();
        }

        private void OnReviveAd()
        {
            bombPopup?.Hide();
            AdvanceAfterReward();
        }

        private void RefreshHud()
        {
            hudView?.SetAtRisk(_runState.Collected);
            hudView?.SetInventory(_wallet.Gold, _wallet.Cash, _wallet.PermanentItems);
            WalletRepository.Save(_wallet);
        }
    }
}