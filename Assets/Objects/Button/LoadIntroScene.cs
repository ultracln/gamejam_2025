using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPastScene : MonoBehaviour
{
    public void LoadPast()
    {
        string last = StaticScene.lastSceneName;
        StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(last);
    }
}
