using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public void GoToMainMenu()
    {
        AudioManager.instance?.PlayButton();
        SceneManager.LoadScene("MainMenu");
    }
}
