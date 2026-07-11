using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI winLoseText;
    public Button actionButton;
    public Button mainMenuButton;
    public TextMeshProUGUI actionButtonText;

    public int totalPickUps = 12;
    public float timeLimit = 60f;
    public string nextSceneName = "Level2";
    public string winButtonLabel = "Next Level";

    [Header("Progression / Rewards")]
    public int nextLevelNumber = 2;   // level unlocked on win (1-based)
    public int coinsPerPickup = 1;    // coins awarded per cube collected
    public int winTimeBonusCoins = 1; // coins per second left on the clock at win

    [Header("Audio")]
    public AudioClip levelMusic;

    private int score = 0;
    private float timeRemaining;
    private bool gameOver = false;
    private bool playerWon = false;
    private int lastWholeSecond = -1;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ApplyDifficulty();
        timeRemaining = timeLimit;
        winLoseText.gameObject.SetActive(false);
        actionButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        UpdateScoreUI();

        AudioManager.instance?.PlayMusic(levelMusic);
    }

    void ApplyDifficulty()
    {
        int difficulty = PlayerPrefs.GetInt("Difficulty", 1);
        if (difficulty == 0) timeLimit = 90f;
        else if (difficulty == 1) timeLimit = 60f;
        else timeLimit = 40f;
    }

    void Update()
    {
        if (gameOver) return;

        timeRemaining -= Time.deltaTime;

        int secondsLeft = Mathf.CeilToInt(timeRemaining);
        timerText.text = "Time: " + secondsLeft.ToString();

        // Tick on each new second during the final countdown.
        if (secondsLeft != lastWholeSecond)
        {
            lastWholeSecond = secondsLeft;
            if (secondsLeft > 0 && secondsLeft <= 5)
                AudioManager.instance?.PlayCountdownTick();
        }

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            TriggerLose();
        }
    }

    public void AddScore()
    {
        score++;
        UpdateScoreUI();
        AudioManager.instance?.PlayPickup();
        if (score >= totalPickUps)
            TriggerWin();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score + " / " + totalPickUps;
    }

    void TriggerWin()
    {
        if (gameOver) return;
        gameOver = true;
        playerWon = true;
        StopAllMovement();
        SaveProgress();
        winLoseText.gameObject.SetActive(true);
        actionButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        winLoseText.text = "YOU WIN!";
        winLoseText.color = Color.green;
        actionButtonText.text = winButtonLabel;
        AudioManager.instance?.PlayWin();
    }

    // Persist rewards + records for a successful run.
    void SaveProgress()
    {
        string level = SceneManager.GetActiveScene().name;
        SaveManager.SetBestScore(level, score);
        SaveManager.SetBestTime(level, timeRemaining);

        int coins = score * coinsPerPickup
                    + Mathf.CeilToInt(timeRemaining) * winTimeBonusCoins;
        SaveManager.AddCoins(coins);

        SaveManager.UnlockLevel(nextLevelNumber);
    }

    public void TriggerLose()
    {
        if (gameOver) return;
        gameOver = true;
        playerWon = false;
        StopAllMovement();
        winLoseText.gameObject.SetActive(true);
        actionButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        winLoseText.text = "YOU LOSE!";
        winLoseText.color = Color.red;
        actionButtonText.text = "Restart";
        AudioManager.instance?.PlayLose();
    }

    void StopAllMovement()
    {
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        foreach (EnemyChase enemy in FindObjectsByType<EnemyChase>(FindObjectsSortMode.None))
            enemy.enabled = false;
    }

    public void ActionButtonPressed()
    {
        AudioManager.instance?.PlayButton();
        if (playerWon)
            SceneManager.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMainMenu()
    {
        AudioManager.instance?.PlayButton();
        SceneManager.LoadScene("MainMenu");
    }
}