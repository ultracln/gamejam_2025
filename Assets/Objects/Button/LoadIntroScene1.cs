using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadIntroScene : MonoBehaviour
{
    public void LoadStart()
    {
        StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("IntroScene");
    }
}
