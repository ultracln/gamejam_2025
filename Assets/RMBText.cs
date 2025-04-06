using UnityEngine;
using System.Collections;

public class RMBText : MonoBehaviour
{
    public GameObject RMBtext;
    public float delayBeforeTutorial = 4f;  // Time after left click to show message
    public float displayTime = 5f;          // How long to show the message

    private bool hasLeftClicked = false;
    private bool tutorialShown = false;

    void Update()
    {
        // Detect first left click (Mouse0)
        if (Input.GetMouseButtonDown(0) && !hasLeftClicked)
        {
            hasLeftClicked = true;
            StartCoroutine(ShowTextAfterDelay());
        }
    }

    IEnumerator ShowTextAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeTutorial);

        // Show the RMB tutorial text
        RMBtext.GetComponent<CanvasGroup>().alpha = 1f;

        // Optional: Enable the object in case it's disabled
        RMBtext.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        // Hide the text again
        RMBtext.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
