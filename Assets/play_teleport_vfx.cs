using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class play_teleport_vfx : MonoBehaviour
{
    // Singleton instance
    public static play_teleport_vfx instance;

    [Header("Post Processing Effect")]
    public Volume globalVolume;
    public float bloomMin = 0f;
    public float bloomMax = 500f;
    public float bloomScatterMin = 0.5f;
    public float bloomScatterMax = 0.8f;
    public float effectDuration = 2f;
    public bool resetAfterFinish = false;

    [Header("Key Input Settings")]
    public KeyCode activationKey = KeyCode.O;

    private Bloom bloom;
    private Coroutine effectCoroutine;

    // Awake ensures that only one instance of the class exists
    void Awake()
    {
        if (instance == null)
        {
            instance = this; // Set the singleton instance
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        // Optionally, keep the object persistent across scene loads
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out bloom);
        }
    }

    void Update()
    {
        // This part is for manual triggering via key press; if not needed, remove.
        if (Input.GetKeyDown(activationKey))
        {
            StartCoroutine(ApplyEffect());
        }
    }

    public void TriggerEffect() // Public method to trigger effect externally
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }
        effectCoroutine = StartCoroutine(ApplyEffect());
    }

    IEnumerator ApplyEffect()
    {
        float elapsedTime = 0f;

        while (elapsedTime < effectDuration)
        {
            float t = elapsedTime / effectDuration;  // Normalized time (0 to 1)

            if (bloom != null)
            {
                bloom.intensity.value = Mathf.Lerp(bloomMin, bloomMax, Mathf.Pow(t, 2));
                bloom.scatter.value = Mathf.Lerp(bloomScatterMin, bloomScatterMax, Mathf.Pow(t, 2));
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (bloom != null && resetAfterFinish)
        {
            bloom.intensity.value = bloomMin;
            bloom.scatter.value = bloomScatterMin;
        }
    }
}
