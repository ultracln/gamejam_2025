using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartPlaying()
    {
        StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("01");
    }
}
