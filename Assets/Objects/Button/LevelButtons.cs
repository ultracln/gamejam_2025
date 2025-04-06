using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButtons : MonoBehaviour
{
    public CloneManager cloneManager;
    public void LoadSceneFromSelf()
    {
        // Get the TextMeshPro component (TMP_Text)
        TMP_Text buttonText = GetComponentInChildren<TMP_Text>();

        if (buttonText != null)
        {
            string sceneName = buttonText.text;

            // Check if the scene exists before attempting to load it
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                cloneManager.Clear();
                StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("Scene '" + sceneName + "' does not exist in Build Settings!");
            }
        }
        else
        {
            Debug.LogError("Button TMP_Text component not found!");
        }
    }
}
