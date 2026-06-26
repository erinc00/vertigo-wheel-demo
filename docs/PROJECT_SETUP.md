# Editor Kurulum Kılavuzu (sahne + asset + build)

Kod tamamen hazır. Bu adımlar yalnızca Unity Editor'da yapılabilen görsel kurulum içindir (~1–2 saat).

## 0. Ön koşullar
- Unity **2021.3.x LTS**, Android Build Support modülü kurulu.
- DOTween import + **Setup DOTween** (Tools → Demigiant → DOTween Utility Panel).
- TextMeshPro Essentials import.

## 1. Sprite import ayarları (Art klasörü)
Tüm `Art/UI` ve `Art/Icons` sprite'larını seç → Inspector:
- **Texture Type: Sprite (2D and UI)**
- Generate Mip Maps: **kapalı**
- Compression: uygun (UI için genelde varsayılan)
- **Apply**

### Sliced border (SADECE bunlara)
Sprite Editor → Border ver, sahnede **Image Type = Sliced**:
- `ui_card_frame_12px_neutral` (border ~12px), `ui_card_frame_4px_zone` (~4px)
- `UI_button_orange_standard`, `UI_button_grey_standard` (sol/sağ ~30px)
- Panel/popup arka planları

> İkonlar, sandıklar, skin render'ları, çark tabanı, indicator **Sliced DEĞİL** → `Set Native Size` + `Preserve Aspect` (kodda zaten preserveAspect=true).

## 2. Sprite Atlas
`Art/Atlases` içinde 2-3 atlas (Create → 2D → Sprite Atlas): `atlas_gameplay_ui`, `atlas_reward_icons`, `atlas_common_ui`.
- İlgili klasörleri Objects for Packing'e ekle. **Allow Rotation: KAPALI**, Tight Packing ihtiyaca göre.

## 3. ScriptableObject verileri
`Create → Vertigo → ...` ile oluştur:
- **Reward** (`Data/Rewards`): gold, cash, chest tier'ları, weapon points, skin'ler, consumable, cosmetic. icon = ilgili sprite, kind, baseAmount, stackable (para/puan=true, skin=false).
- **Wheel Config** (`Data/Wheels`):
  - `wheel_normal` → WheelType=Normal, base=`ui_spin_bronze_base`, indicator=bronze, **8 dilim: 7 ödül + 1 bomba** (bomba slice'ında IsBomb=✔, icon=`ui_card_icon_death`).
  - `wheel_safe` → Safe, base=`ui_spin_silver_base`, **8 ödül, bomba YOK**.
  - `wheel_super` → Super, base=`ui_spin_golden_base`, **8 özel ödül** (tier3 skin / super chest), bomba YOK.
  - OnValidate hatalı kurulumu (safe/super'de bomba vb.) konsola yazar.
- **Zone Progression** (`Data/Zones`): ruleSettings(5,30), normal/safe/super wheel'ları ata, growthPerZone (örn 0.15).
- **Game Config** (`Data/Settings`): spinDuration 3.5, extraTurns 5, reviveCost 25.

## 4. Sahne hiyerarşisi (`Scenes/scene_gameplay`)
```
EventSystem                       (sahnede 1 tane)
Main Camera                       (solid color bg)
ui_canvas_gameplay  [Canvas + CanvasScaler]
  Canvas Scaler:
    UI Scale Mode = Scale With Screen Size
    Reference Resolution = 1080 x 1920
    Screen Match Mode = EXPAND          ← brief şartı
  └ ui_safe_area                        (SafeArea script opsiyonel)
     ├ ui_group_header
     │   ├ ui_image_zone_panel  + ui_text_zone_value (TMP) + ui_text_zone_type_value  → ZoneStripView
     │   └ ui_text_gold_value / ui_text_cash_value / ui_text_collected_value (TMP)     → HudView
     ├ ui_group_wheel                                                                   → WheelView
     │   ├ ui_image_wheel_base        (Image; sprite Build() ile gelir)
     │   ├ ui_transform_wheel_rotating  [+ RotatingWheelMarker]   ← SADECE bu döner, Animator YOK
     │   │     (slice'lar runtime'da buraya Instantiate edilir)
     │   ├ ui_image_wheel_pointer     (Image, sabit)
     │   └ ui_button_spin  [Button + Image=ui_spin_generic_button + SpinButtonMarker]
     └ ui_group_actions  [CanvasGroup → GameplayView.interactionGroup]                  → GameplayView
         └ ui_button_collect  [Button + Image=UI_button_grey_standard + CollectButtonMarker]
ui_canvas_popup  [ayrı Canvas]
  ├ ui_panel_reward                                                                     → RewardPopupView
  │   └ ui_animator_reward_content   (scale/animator burada)
  │        ├ ui_image_reward_value + ui_text_reward_value
  └ ui_panel_bomb                                                                       → BombPopupView
      └ ui_animator_bomb_content     (shake/animator burada)
           ├ ui_image (ui_card_icon_death) + "OH NO, A BOMB EXPLODED" (TMP)
           ├ ui_button_giveup  [Button + UI_button_grey_standard + GiveUpButtonMarker]
           ├ ui_button_revive  [Button + UI_button_orange_standard + ReviveButtonMarker] + ui_text_revive_cost_value
           └ ui_button_revive_ad [Button + ReviveAdButtonMarker]
```
- WheelSliceView **prefab**'ı yap (`Prefabs/UI`): tek Image (RaycastTarget kapalı) + WheelSliceView. WheelView.slicePrefab'a ata.
- **Marker'lar** sayesinde WheelView/GameplayView/BombPopupView referansları **OnValidate** ile otomatik dolar; elle sürükleme gerekmez. (Çıkmazsa objeyi seçip Inspector'da küçük bir değişiklik OnValidate'i tetikler.)
- Gereksiz tüm Image/TMP'de **Raycast Target kapalı** (kod ikon/base/indicator'da zaten kapatıyor; dekoratifleri elle kapat).

## 5. GameFlowController
Boş bir `GameFlow` objesine ekle → Inspector'da: progression, gameConfig, wheelView, gameplayView, hudView, zoneStripView, rewardPopup, bombPopup ata. randomSeed=0 (rastgele) / test için sabit.

## 6. Test
Window → General → Test Runner → EditMode → **Run All** (Core mantığı doğrulanır).

## 7. 3 aspect kontrolü
Game view → 20:9, 16:9, 4:3 preset ekle. Üçünde de HUD/çark/butonlar bozulmamalı (Expand + doğru anchor). 4:3 en sıkışık → çark ile üst HUD çakışmasın.

## 8. Build
Build Settings → Android. Player Settings: Orientation **Portrait**, ürün adı/ikon. **Development Build kapalı**. APK al → GitHub **Releases** `v1.0.0` asset.

## Not — asmdef / DOTween
`Vertigo.Core` (saf mantık, testlenebilir) ve `Vertigo.Tests` asmdef'tir; View'lar Assembly-CSharp'ta olduğundan **DOTween otomatik çalışır** (asmdef sürtüşmesi yok). Sorun çıkarsa `Vertigo.Core.asmdef`'i silmek her şeyi Assembly-CSharp'a indirger (yalnızca test izolasyonu kaybolur).
