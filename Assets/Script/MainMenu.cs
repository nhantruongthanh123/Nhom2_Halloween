using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc phải có thư viện này

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("playScene");
    }

    public void QuitGame()
    {  
        Application.Quit();
    }
}