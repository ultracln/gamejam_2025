using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartPlaying()
    {
        SceneManager.LoadScene("01");
    }
}
