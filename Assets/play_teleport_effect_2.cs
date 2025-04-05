using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class play_teleport_effect_2 : MonoBehaviour
{
    public Volume globalVolume;
    public KeyCode activationKey = KeyCode.I;

    public float bloomMin = 0f;
    public float bloomMax = 500f;

    public float bloomScatterMin = 0.5f;
    public float bloomScatterMax = 0.8f;

    public float effectDuration = 2f;

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
            float easedT = Mathf.Pow(t, 2); // Optional: ease-out curve

            if (bloom != null)
            {
                bloom.intensity.value = Mathf.Lerp(bloomMax, bloomMin, easedT);
                bloom.scatter.value = Mathf.Lerp(bloomScatterMax, bloomScatterMin, easedT);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Final safety reset
        if (bloom != null)
        {
            bloom.intensity.value = bloomMin;
            bloom.scatter.value = bloomScatterMin;
        }
    }
}
