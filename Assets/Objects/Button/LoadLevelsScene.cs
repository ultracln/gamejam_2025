using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelsScene : MonoBehaviour
{
    public void LoadLevels()
    {
        StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("SelectLevel");
    }
}
