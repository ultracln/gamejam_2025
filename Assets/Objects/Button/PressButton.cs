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

    public DoorOpen doorOpen = null;
    public GameObject box = null;

    public string buttonID;
    public static string LastPressedButtonID;

    private bool isExternallyPressed = false;

    void Start()
    {
        originalPosition = transform.localPosition;
        pressedPosition = new Vector3(originalPosition.x, -0.18f, originalPosition.z);
    }

    void Update()
    {
        if (!isExternallyPressed)
            HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        bool isLookingAtButton = Physics.Raycast(ray, out hit, interactionDistance) && hit.transform == transform;

        if (isLookingAtButton && Input.GetMouseButton(0)) // Holding left click
        {
            if (!isPressed)
            {
                LastPressedButtonID = buttonID;

                isPressed = true;
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(MoveToPosition(pressedPosition, pressDuration));

                if (audioSource && pressSound)
                    audioSource.PlayOneShot(pressSound);

                if (box != null)
                {
                    box.SetActive(true);
                }

                if (doorOpen != null)
                {
                    doorOpen.OpenDoor();
                }
            }
        }
        else
        {
            if (isPressed)
            {
                isPressed = false;
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(MoveToPosition(originalPosition, returnDuration));

                if (box != null)
                {
                    box.SetActive(false);
                }

                if (doorOpen != null)
                {
                    doorOpen.CloseDoor();
                }
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

    public void TriggerPressExternally(float holdDuration)
    {
        if (!isPressed)
        {
            Debug.Log("Clone pressed button");
            isPressed = true;
            isExternallyPressed = true;

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveToPosition(pressedPosition, pressDuration));

            if (audioSource && pressSound)
                audioSource.PlayOneShot(pressSound);

            if (box != null)
            {
                box.SetActive(true);
            }

            if (doorOpen != null)
            {
                doorOpen.OpenDoor();
            }

            // Optional: Return after hold duration
            StartCoroutine(ReturnAfterHold(holdDuration));
        }
    }

    private IEnumerator ReturnAfterHold(float duration)
    {
        yield return new WaitForSeconds(duration);

        isPressed = false;
        isExternallyPressed = false;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToPosition(originalPosition, returnDuration));

        if (box != null)
        {
            box.SetActive(false);
        }

        if (doorOpen != null)
        {
            doorOpen.CloseDoor();
        }
    }
}
