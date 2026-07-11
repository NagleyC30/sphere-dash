using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Data-driven shop. Spends coins earned in levels (SaveManager) on items that
/// actually apply in-game (PlayerLoadout / PlayerAbilities read the same saves):
///   - Skin:    cosmetic sphere color. Buy once, then equip freely.
///   - Upgrade: stackable level (e.g. permanent speed). Buyable repeatedly.
///   - OneOff:  a single unlock, e.g. "start each level with a shield".
///
/// Configure the `items` array in the Inspector and hook a coins label.
/// Buy listeners are wired here in Start(), so no OnClick setup in the editor.
/// </summary>
public class Shop : MonoBehaviour
{
    public enum Kind { Skin, Upgrade, OneOff }

    [System.Serializable]
    public class ShopItem
    {
        public string id;                 // stable key used in save data
        public Kind kind = Kind.Skin;
        public int cost = 50;
        public Color skinColor = Color.white; // Skin only
        public Button buyButton;
        public TextMeshProUGUI statusLabel;   // optional: shows price/owned/equipped
    }

    public ShopItem[] items;
    public TextMeshProUGUI coinsLabel;

    void Start()
    {
        foreach (ShopItem item in items)
        {
            if (item == null || item.buyButton == null) continue;
            ShopItem captured = item;
            item.buyButton.onClick.AddListener(() => TryBuy(captured));
        }
        Refresh();
    }

    void TryBuy(ShopItem item)
    {
        switch (item.kind)
        {
            case Kind.Skin:
                // Already owned → just equip. Otherwise pay, then own + equip.
                if (SaveManager.IsOwned(item.id) || SaveManager.TrySpendCoins(item.cost))
                {
                    SaveManager.SetOwned(item.id);
                    SaveManager.EquippedColor = item.skinColor;
                    AudioManager.instance?.PlayButton();
                }
                break;

            case Kind.Upgrade:
                if (SaveManager.TrySpendCoins(item.cost))
                {
                    SaveManager.IncrementUpgrade(item.id);
                    AudioManager.instance?.PlayButton();
                }
                break;

            case Kind.OneOff:
                if (!SaveManager.IsOwned(item.id) && SaveManager.TrySpendCoins(item.cost))
                {
                    SaveManager.SetOwned(item.id);
                    AudioManager.instance?.PlayButton();
                }
                break;
        }
        Refresh();
    }

    void Refresh()
    {
        if (coinsLabel != null) coinsLabel.text = "Coins: " + SaveManager.Coins;

        foreach (ShopItem item in items)
        {
            if (item == null) continue;
            bool affordable = SaveManager.Coins >= item.cost;

            if (item.statusLabel != null)
                item.statusLabel.text = StatusText(item);

            if (item.buyButton != null)
                item.buyButton.interactable = ButtonEnabled(item, affordable);
        }
    }

    string StatusText(ShopItem item)
    {
        switch (item.kind)
        {
            case Kind.Skin:
                if (!SaveManager.IsOwned(item.id)) return item.cost + " coins";
                return SaveManager.EquippedColor == item.skinColor ? "Equipped" : "Equip";

            case Kind.Upgrade:
                return "Lv " + SaveManager.GetUpgradeLevel(item.id) + "  (" + item.cost + ")";

            case Kind.OneOff:
                return SaveManager.IsOwned(item.id) ? "Owned" : item.cost + " coins";
        }
        return "";
    }

    bool ButtonEnabled(ShopItem item, bool affordable)
    {
        switch (item.kind)
        {
            case Kind.Skin:   return SaveManager.IsOwned(item.id) || affordable; // equip or buy
            case Kind.OneOff: return !SaveManager.IsOwned(item.id) && affordable;
            default:          return affordable; // upgrades are repeatable
        }
    }

    public void BackToMenu()
    {
        AudioManager.instance?.PlayButton();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
