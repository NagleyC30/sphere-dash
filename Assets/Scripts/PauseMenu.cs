using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// In-level pause menu. Toggles with Escape (new Input System). Freezes the game
/// via Time.timeScale so the timer, player physics, and enemies all halt.
/// Wire a pause panel and its Resume / Restart / Main Menu buttons to the public
/// methods in the Inspector.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    public static bool IsPaused { get; private set; }

    void Start()
    {
        // Always start unpaused — important because timeScale is global and a
        // previous scene could have left it at 0 on a hard exit.
        IsPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        AudioManager.instance?.PlayButton();
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        AudioManager.instance?.PlayButton();
    }

    public void Restart()
    {
        AudioManager.instance?.PlayButton();
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMainMenu()
    {
        AudioManager.instance?.PlayButton();
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
}
