using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    public bool isGamePaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        UpdateCursorState();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCursorState(); // Ensure it updates when switching scenes
    }

    public void SetPaused(bool paused)
    {
        isGamePaused = paused;
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        bool showCursor =
            sceneName == "IntroScene" ||
            sceneName == "SelectLevel" ||
            sceneName == "Settings" ||
            isGamePaused;

        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = showCursor;
    }
}
