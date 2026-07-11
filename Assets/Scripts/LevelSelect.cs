using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Level-select screen driven by saved unlock progress. Configure one entry per
/// level in the Inspector (button + level number + scene name). Locked levels
/// are shown non-interactable; an optional label shows the best score.
///
/// Reads SaveManager.IsLevelUnlocked / GetBestScore — populated automatically as
/// the player wins levels (GameManager.SaveProgress).
/// </summary>
public class LevelSelect : MonoBehaviour
{
    [System.Serializable]
    public class LevelEntry
    {
        public Button button;
        public int levelNumber = 1;         // 1-based; level 1 is always unlocked
        public string sceneName = "Level1";
        public TMPro.TextMeshProUGUI bestScoreLabel; // optional
    }

    public LevelEntry[] levels;

    void Start()
    {
        foreach (LevelEntry entry in levels)
        {
            if (entry == null || entry.button == null) continue;

            bool unlocked = SaveManager.IsLevelUnlocked(entry.levelNumber);
            entry.button.interactable = unlocked;

            if (entry.bestScoreLabel != null)
            {
                entry.bestScoreLabel.text = unlocked
                    ? "Best: " + SaveManager.GetBestScore(entry.sceneName)
                    : "Locked";
            }

            LevelEntry captured = entry; // avoid closure capturing the loop var
            entry.button.onClick.AddListener(() => LoadLevel(captured));
        }
    }

    void LoadLevel(LevelEntry entry)
    {
        if (!SaveManager.IsLevelUnlocked(entry.levelNumber)) return;
        AudioManager.instance?.PlayButton();
        SceneManager.LoadScene(entry.sceneName);
    }
}
