using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartEasy()
    {
        PlayerPrefs.SetInt("Difficulty", 0);
        SceneManager.LoadScene("Level1");
    }

    public void StartMedium()
    {
        PlayerPrefs.SetInt("Difficulty", 1);
        SceneManager.LoadScene("Level1");
    }

    public void StartHard()
    {
        PlayerPrefs.SetInt("Difficulty", 2);
        SceneManager.LoadScene("Level1");
    }
}
