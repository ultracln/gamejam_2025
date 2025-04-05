using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerOnTeleporter : MonoBehaviour
{
    public float timeToStayOnTeleporter = 1f;
    public string targetObjectName = "SM_Teleporter"; // Name of the object to check
    private float timeOnObject = 0f; // Tracks time player stays on the object
    public string targetColorBox = "colorBox";

    public CubeColorChecker checker;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == targetObjectName)
        {
            timeOnObject = 0f; // Reset timer when entering
        }

        if (other.gameObject.name.StartsWith(targetColorBox))
        {
            Renderer renderer = other.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color yellow;
                Color gray;
                ColorUtility.TryParseHtmlString("#FFBF3B", out yellow);
                gray = new Color(0.684f, 0.684f, 0.684f, 1f);

                if (renderer.material.color == yellow)
                    renderer.material.color = gray;
                else
                    renderer.material.color = yellow;

                checker.CompareCubeColors();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == targetObjectName)
        {
            timeOnObject += Time.deltaTime; // Increment time while staying

            if (timeOnObject >= timeToStayOnTeleporter) // If stayed for 1 second or more
            {
                LoadNextScene();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == targetObjectName)
        {
            timeOnObject = 0f; // Reset timer when leaving
        }
    }

    private void LoadNextScene()
    {
        // Get the current scene name
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Try to convert scene name into a number
        if (int.TryParse(currentSceneName, out int sceneNumber))
        {
            // Generate the next scene name (increment the number)
            string nextSceneName = (sceneNumber + 1).ToString("00"); // Keeps format like "01", "02", "03"

            // Check if the next scene exists before loading (recommended)
            if (Application.CanStreamedLevelBeLoaded(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogError("Scene " + nextSceneName + " does not exist!");
            }
        }
        else
        {
            Debug.LogError("Invalid scene name format: " + currentSceneName);
        }
    }
}
