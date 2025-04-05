using System.Collections;
using UnityEngine;

public class bakingTime : MonoBehaviour
{
    /*public GameObject tutorialText;

    IEnumerator Start()
    {
        Debug.Log("bakin time");
        tutorialText.SetActive(true);
        yield return null; // Wait one frame
        tutorialText.SetActive(false);
        Debug.Log("bakin time is over");
    }*/

    public CanvasGroup canvasGroup; // Drag in CanvasGroup component

    IEnumerator Start()
    {
        canvasGroup.alpha = 0f; // Hide before first frame
        // canvasGroup.gameObject.SetActive(true); // Ensure it's active
        yield return null;
        canvasGroup.alpha = 0f; // Still hidden after warmup
    }
}
