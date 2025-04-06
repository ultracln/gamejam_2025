using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public CloneManager cloneManager;
    public void StartPlaying()
    {
        cloneManager.Clear();
        StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("01");
    }
}
