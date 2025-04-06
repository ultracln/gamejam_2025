using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonHint : MonoBehaviour
{
    public GameObject tutorialText;     // First tutorial (near button)
    public GameObject tutorialText2;    // Second tutorial (after click)

    public Transform cameraTransform;   // Main camera reference
    public float activationDistance = 3f;

    public float delayBeforeSecondTut = 1f;
    public float secondTutDuration = 3f;

    private bool isShowing = false;
    private bool playerHasClicked = false;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, cameraTransform.position);

        // Show first tutorial if player is nearby and hasn't clicked yet
        if (distance <= activationDistance && !isShowing && !playerHasClicked && SceneManager.GetActiveScene().name != StaticScene.lastSceneName)
        {
            tutorialText.GetComponent<CanvasGroup>().alpha = 1f;
            isShowing = true;
        }
        else if (distance > activationDistance && isShowing)
        {
            tutorialText.GetComponent<CanvasGroup>().alpha = 0f;
            isShowing = false;
        }

        // Check for first left click
        if (Input.GetMouseButtonDown(0) && isShowing && !playerHasClicked)
        {
            playerHasClicked = true;

            // Hide first tutorial
            tutorialText.GetComponent<CanvasGroup>().alpha = 0f;
            isShowing = false;

            if (SceneManager.GetActiveScene().name != StaticScene.lastSceneName)
            {
                // Start coroutine to show second tutorial
                StartCoroutine(ShowSecondTutorial());
            }
        }
    }

    IEnumerator ShowSecondTutorial()
    {
        yield return new WaitForSeconds(delayBeforeSecondTut);

        // Show second tutorial
        tutorialText2.GetComponent<CanvasGroup>().alpha = 1f;
        Debug.Log("acum ar tb sa se afiseze tut 2");

        yield return new WaitForSeconds(secondTutDuration);

        // Hide second tutorial
        tutorialText2.GetComponent<CanvasGroup>().alpha = 0f;
    }
}