using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelsScene : MonoBehaviour
{
    public void LoadLevels()
    {
        SceneManager.LoadScene("SelectLevel");
    }
}
