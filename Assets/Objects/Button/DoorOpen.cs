using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour
{
    public float moveDistance = 1.5f;
    public float moveDuration = 1.0f;

    public AudioSource audioSource;
    public AudioClip doorSound;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private Coroutine moveCoroutine;

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(moveDistance, 0, 0);
    }

    public void OpenDoor()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoor(openPosition));
        PlaySound();
    }

    public void CloseDoor()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoor(closedPosition));
        PlaySound();
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    private void PlaySound()
    {
        if (audioSource != null && doorSound != null)
        {
            audioSource.PlayOneShot(doorSound);
        }
    }
}
