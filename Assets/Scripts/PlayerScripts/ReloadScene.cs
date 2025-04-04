using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    private float rightClickHoldTime = 0f;
    public float requiredHoldDuration = 2f;

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            rightClickHoldTime += Time.deltaTime;

            if (rightClickHoldTime >= requiredHoldDuration)
            {
                ReloadSceneOnClick();
            }
        }
        else
        {
            // Reset the timer if the button is released
            rightClickHoldTime = 0f;
        }
    }

    void ReloadSceneOnClick()
    {
        // Get the currently active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}