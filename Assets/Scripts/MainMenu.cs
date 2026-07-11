using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip menuMusic;

    void Start()
    {
        AudioManager.instance?.PlayMusic(menuMusic);
    }

    public void StartEasy() => StartGame(0);
    public void StartMedium() => StartGame(1);
    public void StartHard() => StartGame(2);

    void StartGame(int difficulty)
    {
        PlayerPrefs.SetInt("Difficulty", difficulty);
        PlayerPrefs.Save();
        AudioManager.instance?.PlayButton();
        SceneManager.LoadScene("Level1");
    }
}
