using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadIntroScene : MonoBehaviour
{
    public void LoadStart()
    {
        SceneManager.LoadScene("IntroScene");
    }
}
