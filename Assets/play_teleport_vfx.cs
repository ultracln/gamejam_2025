using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class play_teleport_vfx : MonoBehaviour
{
    public Volume globalVolume;
    public KeyCode activationKey = KeyCode.O;

    public float bloomMin = 0f;
    public float bloomMax = 500f;

    public float bloomScatterMin = 0.5f;
    public float bloomScatterMax = 0.8f;

    public float effectDuration = 2f;

    public bool resetAfterFinish = false;

    private Bloom bloom;
    private Coroutine effectCoroutine;

    void Start()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out bloom);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            StartCoroutine(ApplyEffect());
        }
    }

    IEnumerator ApplyEffect()
    {
        float elapsedTime = 0f;

        while (elapsedTime < effectDuration)
        {
            float t = elapsedTime / effectDuration;  // Normalized time (0 to 1)

            if (bloom != null)
            {
                bloom.intensity.value = Mathf.Lerp(bloomMin, bloomMax, Mathf.Pow(t, 2));  // Increase Chromatic Aberration
                bloom.scatter.value = Mathf.Lerp(bloomScatterMin, bloomScatterMax, Mathf.Pow(t, 2));
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset effects after the duration
        if (bloom != null && resetAfterFinish)
        {
            bloom.intensity.value = bloomMin;
            bloom.scatter.value = bloomScatterMin;
        }
    }
}