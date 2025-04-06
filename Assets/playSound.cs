using UnityEngine;
using UnityEngine.UI;

public class playSound : MonoBehaviour
{
    public AudioClip clickSound;

    private Button button;

    private void Awake()
    {
        
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        SFXManager.instance.playSoundFXvoid(clickSound, transform, 1f);
    }
}
