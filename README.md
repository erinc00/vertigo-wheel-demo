# Vertigo Wheel — Wheel of Fortune Demo

A revolver-style "spin the wheel" risk-reward game built for the Vertigo Games Game Developer technical demo. Built with **Unity 2021.3 LTS**.

## Gameplay

- Each zone, the player spins an 8-slot wheel. One slot is a **bomb** (loses everything collected this run); the rest are rewards.
- Rewards scale up as zones progress, and reward **types** also evolve through a tiered wheel system (Normal zones unlock bigger chests, weapon skins and consumables as you go deeper).
- Every **5th zone** is a **Safe Zone** (silver wheel, no bomb).
- Every **30th zone** is a **Super Zone** (golden wheel, no bomb, special rewards including cosmetics).
- The player can **Collect** (bank everything permanently) only while the wheel is idle and the zone is Safe or Super.
- Hitting a bomb forfeits everything collected this run; the player can **Give Up** (restart) or **Revive** (spend gold to keep going — bonus feature).
- A **Menu** button lets the player return to the start screen at any time the wheel is idle; doing so forfeits the current run's pending rewards (same as a bomb).
- Permanently collected rewards (Gold, Cash, chests, skins, etc.) persist across app sessions via local save data — they are never lost on restart.

## Architecture (SOLID / OOP)

```
Core (plain C#, no DOTween/UI dependency, fully unit-testable)
  ZoneRules · WeightedWheelResultSelector · RunState · Wallet · RewardScaler
  RewardData / RewardPool / WheelConfig / ZoneProgressionConfig / GameConfig (ScriptableObject)
  WalletRepository / WalletSaveData / RewardDatabase (local persistence)
  GameStateMachine

Views (MonoBehaviour, Assembly-CSharp)
  WheelView (DOTween spin) · GameplayView · HudView · ZoneStripView
  RewardPopupView · BombPopupView · StartScreenView

Gameplay
  GameFlowController (composition root; no singletons)

Tests (EditMode)
  ZoneRules / WeightedWheelResultSelector / RunState / Wallet / GameStateMachine
```

- The spin **result is always selected before the animation plays** — the wheel only visualizes a predetermined outcome, guaranteeing the icon shown always matches the reward given.
- Reward **type and amount** are fully data-driven: every wheel's 8 slices, their rewards, weights, and optional **reward pools** (for randomized skins/consumables/cosmetics) are configured entirely from the Unity Editor via ScriptableObjects — no hardcoded content.
- Zone reward **tiers**: each zone type (Normal/Safe/Super) can have multiple `WheelConfig` tiers mapped to zone ranges, so reward variety genuinely improves as the player progresses, not just the numeric amount.
- `ScriptableObject` assets hold only configuration; all mutable run-time state (`RunState`, `Wallet`) lives in plain C# objects, never on the asset itself.

## UI technical compliance

- Canvas Scaler: **Expand** mode, verified clean at **20:9, 16:9 and 4:3** aspect ratios.
- All text uses **TextMeshPro**.
- Every UI element whose content changes at runtime ends with **`_value`** (e.g. `ui_text_zone_value`, `ui_image_wheel_base_value`).
- Naming goes from general to specific (`ui_image_spin_silver`, `ui_button_collect`, ...).
- Unnecessary `Raycast Target` / `Maskable` are disabled on decorative elements.
- UI Animators live in **separate child transforms**, never on the root panel.
- Anchors and pivots are set for full responsiveness across aspect ratios.
- Button references are wired automatically via **`OnValidate`** + marker components — never dragged manually in the Inspector.
- Sliced sprites are used for frames/panels/buttons; icons and renders are never stretched (`Preserve Aspect`).
- No Unity `OnClick()` Editor wiring anywhere — every listener is added from code.

## Bonus features implemented

- ScriptableObject-driven content (rewards, wheels, zone tiers, reward pools)
- DOTween for the spin animation, popup punches, bomb shake, and reward flash VFX
- Sprite Atlas packing for the UI sprite set
- Unity 2021 LTS
- In-app **Continue/Revive** system (gold cost) plus a bonus "Revive via Ad" stub
- Local persistence of permanent wallet/inventory across sessions

## Project setup

1. Open with **Unity 2021.3.x LTS**.
2. Import **DOTween** (Demigiant, free) and run its Setup from the DOTween Utility Panel.
3. Import **TextMeshPro Essential Resources** (Window → TextMeshPro).
4. Open `Assets/_Project/Scenes/scene_gameplay`.
5. Press Play — the start screen appears; click **Start Game**.

## Tests

Window → General → Test Runner → EditMode → Run All. Covers zone-type resolution, weighted slice selection, run-state/wallet bookkeeping, and state-machine input gating.

## Build

- Platform: Android, Orientation: Portrait, Scripting Backend: Mono.
- **Release** build (Development Build unchecked).
- APK is published under this repository's **Releases** as `VertigoWheel.apk`.

## External packages

- DOTween (Demigiant)
- TextMeshPro (Unity package)
