using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign your pause Canvas here in Inspector
    public static bool IsPaused { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        pauseMenuUI.SetActive(IsPaused);

        Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsPaused;
        AudioListener.pause = IsPaused;

        CursorManager.Instance.isGamePaused = IsPaused;
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AudioListener.pause = false;

        CursorManager.Instance.isGamePaused = false;
    }

    public void RestartLevel()
    {
        ResumeGame();
        StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        ResumeGame();
        StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("IntroScene"); // Replace with your main menu scene name
    }

    public void QuitToSelectLevel()
    {
        ResumeGame();
        StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("SelectLevel"); // Replace with your main menu scene name
    }

    public void QuitToSettings()
    {
        ResumeGame();
        StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("SelectLevel"); // Replace with your main menu scene name
    }
}
