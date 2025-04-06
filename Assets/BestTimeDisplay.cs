using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BestTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float high_score;

    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Check if the high score for the current scene exists
        if (StaticScene.sceneHighScores.ContainsKey(currentSceneName))
        {
            high_score = StaticScene.sceneHighScores[currentSceneName]; // Retrieve the high score
        }
        else
        {
            high_score = 0f; // Default value when no high score is found
        }
    }
    // Update is called once per frame
    void Update()
    {
        // aici iau din scenemanager variabila statica in care am retinut best time-ul aferent nivelului curent (probabil un dictionar {String, float})
        timerText.text = $"Best: {FormatTime(high_score)}";
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000);
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}
