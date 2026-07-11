using UnityEngine;

/// <summary>
/// Central place for all persistent data (PlayerPrefs-backed).
/// Static so any script can read/write without a scene object:
///   SaveManager.AddCoins(10);
///   int best = SaveManager.GetBestScore("Level1");
/// Later this can be swapped for a JSON save file without touching callers.
/// </summary>
public static class SaveManager
{
    // ---- Keys ----
    const string CoinsKey = "Coins";
    const string MusicVolumeKey = "MusicVolume";
    const string SfxVolumeKey = "SfxVolume";
    const string HighestLevelKey = "HighestLevelUnlocked";
    const string BestScorePrefix = "BestScore_";
    const string BestTimePrefix = "BestTime_";

    // ---- Coins (shop currency) ----
    public static int Coins
    {
        get => PlayerPrefs.GetInt(CoinsKey, 0);
        set
        {
            PlayerPrefs.SetInt(CoinsKey, Mathf.Max(0, value));
            PlayerPrefs.Save();
        }
    }

    public static void AddCoins(int amount) => Coins = Coins + amount;

    /// <summary>Returns true and deducts if the player can afford it; false otherwise.</summary>
    public static bool TrySpendCoins(int amount)
    {
        if (amount <= 0 || Coins < amount) return false;
        Coins -= amount;
        return true;
    }

    // ---- Audio settings (0..1) ----
    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MusicVolumeKey, 0.6f);
        set
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }

    public static float SfxVolume
    {
        get => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        set
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }

    // ---- Level unlock progress (1-based level number) ----
    public static int HighestLevelUnlocked => PlayerPrefs.GetInt(HighestLevelKey, 1);

    public static void UnlockLevel(int level)
    {
        if (level > HighestLevelUnlocked)
        {
            PlayerPrefs.SetInt(HighestLevelKey, level);
            PlayerPrefs.Save();
        }
    }

    public static bool IsLevelUnlocked(int level) => level <= HighestLevelUnlocked;

    // ---- Best score per level ----
    public static int GetBestScore(string levelName) =>
        PlayerPrefs.GetInt(BestScorePrefix + levelName, 0);

    public static void SetBestScore(string levelName, int score)
    {
        if (score > GetBestScore(levelName))
        {
            PlayerPrefs.SetInt(BestScorePrefix + levelName, score);
            PlayerPrefs.Save();
        }
    }

    // ---- Best time remaining per level (more time left = better run) ----
    public static float GetBestTime(string levelName) =>
        PlayerPrefs.GetFloat(BestTimePrefix + levelName, 0f);

    public static void SetBestTime(string levelName, float timeRemaining)
    {
        if (timeRemaining > GetBestTime(levelName))
        {
            PlayerPrefs.SetFloat(BestTimePrefix + levelName, timeRemaining);
            PlayerPrefs.Save();
        }
    }

    // ---- Shop: ownership (skins bought, one-off upgrades) ----
    public static bool IsOwned(string id) => PlayerPrefs.GetInt("Owned_" + id, 0) == 1;

    public static void SetOwned(string id)
    {
        PlayerPrefs.SetInt("Owned_" + id, 1);
        PlayerPrefs.Save();
    }

    // ---- Shop: stackable upgrade levels (e.g. speed) ----
    public static int GetUpgradeLevel(string id) => PlayerPrefs.GetInt("Upgrade_" + id, 0);

    public static void IncrementUpgrade(string id)
    {
        PlayerPrefs.SetInt("Upgrade_" + id, GetUpgradeLevel(id) + 1);
        PlayerPrefs.Save();
    }

    // ---- Shop: equipped cosmetic color (applied to the player sphere) ----
    // Stored as RGB floats since PlayerPrefs can't hold a Color. Default white.
    public static Color EquippedColor
    {
        get => new Color(
            PlayerPrefs.GetFloat("SkinR", 1f),
            PlayerPrefs.GetFloat("SkinG", 1f),
            PlayerPrefs.GetFloat("SkinB", 1f));
        set
        {
            PlayerPrefs.SetFloat("SkinR", value.r);
            PlayerPrefs.SetFloat("SkinG", value.g);
            PlayerPrefs.SetFloat("SkinB", value.b);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Wipes all saved data. Handy for a debug/reset button.</summary>
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
