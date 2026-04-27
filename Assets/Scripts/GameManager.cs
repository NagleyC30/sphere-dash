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
    public TextMeshProUGUI actionButtonText;

    public int totalPickUps = 12;
    public float timeLimit = 60f;
    public string nextSceneName = "Level2";
    public string winButtonLabel = "Next Level";

    private int score = 0;
    private float timeRemaining;
    private bool gameOver = false;
    private bool playerWon = false;

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
        UpdateScoreUI();
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
        timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();

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
        winLoseText.gameObject.SetActive(true);
        actionButton.gameObject.SetActive(true);
        winLoseText.text = "YOU WIN!";
        winLoseText.color = Color.green;
        actionButtonText.text = winButtonLabel;
    }

    public void TriggerLose()
    {
        if (gameOver) return;
        gameOver = true;
        playerWon = false;
        StopAllMovement();
        winLoseText.gameObject.SetActive(true);
        actionButton.gameObject.SetActive(true);
        winLoseText.text = "YOU LOSE!";
        winLoseText.color = Color.red;
        actionButtonText.text = "Restart";
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
        if (playerWon)
            SceneManager.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}