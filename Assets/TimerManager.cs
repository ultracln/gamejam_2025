using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    public float timeElapsed = 0f;
    public bool isTiming = true;

    private string initialSceneName;

    void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads

            initialSceneName = SceneManager.GetActiveScene().name; // Store the scene name at the start
        }
        else
        {
            // If an instance already exists, destroy this one to prevent duplicates
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (isTiming)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    public void ResetTimer()
    {
        // Reset the timer only if we're loading a **new** scene
        if (SceneManager.GetActiveScene().name != StaticScene.lastSceneName)
        {
            timeElapsed = 0f; // Reset the timer
            initialSceneName = SceneManager.GetActiveScene().name; // Update the initial scene name
        }
    }

    public float GetTime()
    {
        return timeElapsed;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the scene is reloaded (same scene), do not reset the timer
        if (scene.name == StaticScene.lastSceneName)
        {
            // Do not reset the timer when reloading the same scene
            Debug.Log("Scene reloaded, keeping timer running...");
        }
        else
        {
            // Reset the timer if it's a different scene
            Debug.Log("New scene loaded, resetting timer...");
            ResetTimer();
        }
    }
}
