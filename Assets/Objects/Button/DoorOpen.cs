using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour
{
    public float moveDistance = 1.5f;        // How far to move on local X
    public float moveDuration = 1.0f;        // Time to move

    public AudioSource audioSource;
    public AudioClip doorSound;

    private Vector3 closedLocalPos;
    private Vector3 openLocalPos;

    private Coroutine moveCoroutine;

    private void Awake()
    {
        closedLocalPos = transform.localPosition;
        openLocalPos = new Vector3(closedLocalPos.x + moveDistance, closedLocalPos.y, closedLocalPos.z);
    }

    public void OpenDoor()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoor(openLocalPos));
        PlaySound();
    }

    public void CloseDoor()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoor(closedLocalPos));
        PlaySound();
    }

    private IEnumerator MoveDoor(Vector3 targetLocalPos)
    {
        Vector3 startLocalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetLocalPos;
    }

    private void PlaySound()
    {
        if (audioSource != null && doorSound != null)
        {
            audioSource.PlayOneShot(doorSound);
        }
    }
}
