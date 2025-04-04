using UnityEngine;
using System.Collections;

public class PressButton : MonoBehaviour
{
    public float pressDuration = 0.1f;
    public float returnDuration = 0.1f;

    public float interactionDistance = 3f;
    public Transform playerCamera;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;

    public AudioSource audioSource;
    public AudioClip pressSound;

    private bool isPressed = false;
    private Coroutine moveCoroutine;

    void Start()
    {
        originalPosition = transform.localPosition;
        pressedPosition = new Vector3(originalPosition.x, -0.18f, originalPosition.z);
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        bool isLookingAtButton = Physics.Raycast(ray, out hit, interactionDistance) && hit.transform == transform;

        if (isLookingAtButton && Input.GetMouseButton(0)) // Holding left click
        {
            if (!isPressed)
            {
                isPressed = true;
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(MoveToPosition(pressedPosition, pressDuration));
                if (audioSource && pressSound)
                    audioSource.PlayOneShot(pressSound);
            }
        }
        else
        {
            if (isPressed)
            {
                isPressed = false;
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(MoveToPosition(originalPosition, returnDuration));
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.localPosition;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;
    }
}
