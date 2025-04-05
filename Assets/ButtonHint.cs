using UnityEngine;

public class ButtonHint : MonoBehaviour
{
    public GameObject tutorialText;     // Drag the World Space text object here
    public Transform cameraTransform;   // Drag the main camera here
    public float activationDistance = 3f;

    private bool isShowing = false;

    private bool playerHasCLicked = false;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, cameraTransform.position);
        // Debug.Log(distance);

        if (distance <= activationDistance && !isShowing && !playerHasCLicked)
        {
            Debug.Log("AR TREBUI ACUM AFISAT ");
            // tutorialText.SetActive(true);
            tutorialText.GetComponent<CanvasGroup>().alpha = 1f;
            isShowing = true;
        }
        else if (distance > activationDistance && isShowing)
        {
             //tutorialText.SetActive(false);
            tutorialText.GetComponent<CanvasGroup>().alpha = 0f;
            isShowing = false;
        }

        // Check for left click (once)
        if (Input.GetMouseButtonDown(0) && isShowing && !playerHasCLicked)
        {
            playerHasCLicked = true;
            // tutorialText.SetActive(false);
            tutorialText.GetComponent<CanvasGroup>().alpha = 0f;
            isShowing = false;
            // Debug.Log("Hint disabled after click");
        }
    }
}
