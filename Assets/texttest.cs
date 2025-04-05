using UnityEngine;
using TMPro; // only if you're using TextMeshPro

public class texttest : MonoBehaviour
{
    public GameObject tutorialText; // drag the Text object here in Inspector

    void Start()
    {
        // Show text at start
        // tutorialText.SetActive(false);
        tutorialText.GetComponent<CanvasGroup>().alpha = 0f;


        // Hide after a few seconds (optional)
        // Invoke("HideText", 5f);
    }

    void HideText()
    {
        tutorialText.SetActive(false);
    }
}