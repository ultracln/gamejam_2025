using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class play_vfx : MonoBehaviour
{
    public Volume globalVolume;
    public KeyCode activationKey = KeyCode.R;
    public float maxHoldTime = 2f;  // Exact time before auto-reset
    public float effectDecreaseSpeed = 3f;  // Speed of effect fading

    public float chromaticAberrationMax = 1f;
    public float chromaticAberrationMin = 0.3f;

    public float lensDistortionMax = -0.6f;
    public float lensDistortionMin = -0.2f;

    public float vignetteMax = 0.5f;
    public float vignetteMin = 0.2f;

    public float shakeAmount = 0.2f; // Amount of shake
    public float shakeDuration = 0.5f; // Duration of shake

    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private Vignette vignette;

    private Camera mainCamera;
    private Vector3 originalPosition;
    private bool isShaking;

    private float holdTime = 0f;
    private bool effectLocked = false;
    private bool isFadingOut = false;

    private void Start()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out chromaticAberration);
            globalVolume.profile.TryGet(out lensDistortion);
            globalVolume.profile.TryGet(out vignette);
        }

        // Cache the main camera
        mainCamera = Camera.main;
        originalPosition = mainCamera.transform.position;
    }

    private void Update()
    {
        if (Input.GetKey(activationKey) && !effectLocked && !isFadingOut)
        {
            holdTime += Time.deltaTime;

            // If max hold time is reached, start fading out
            if (holdTime >= maxHoldTime)
            {
                isFadingOut = true;
            }
            if (!isShaking)
            {
                StartCoroutine(ScreenShake());
            }
        }
        else if (isFadingOut)  // Smooth fade-out when time limit is reached
        {
            holdTime -= Time.deltaTime * effectDecreaseSpeed;

            if (holdTime <= 0f)
            {
                holdTime = 0f;
                isFadingOut = false;
                effectLocked = true; // Prevent reactivation until key is released
            }
        }
        else if (!Input.GetKey(activationKey)) // Key released
        {
            holdTime -= Time.deltaTime * effectDecreaseSpeed;
            effectLocked = false; // Unlock when key is released
        }

        holdTime = Mathf.Clamp(holdTime, 0f, maxHoldTime);  // Keep within bounds
        float effectStrength = holdTime / maxHoldTime;  // Normalize (0 to 1)

        // Apply post-processing effects
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberrationMin, chromaticAberrationMax, Mathf.Pow(effectStrength, 2));

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(lensDistortionMin, lensDistortionMax, effectStrength);

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(vignetteMin, vignetteMax, effectStrength);
    }

    private IEnumerator ScreenShake()
    {
        isShaking = true;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Randomly shake the camera position
            Vector3 shakeOffset = originalPosition + new Vector3(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), 0);
            mainCamera.transform.position = shakeOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset camera position after shake
        mainCamera.transform.position = originalPosition;
        isShaking = false;
    }
}