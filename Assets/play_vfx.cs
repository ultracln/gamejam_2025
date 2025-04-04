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
    public float lensDistortionMax = -0.6f;
    public float vignetteMax = 0.5f;

    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private Vignette vignette;

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

        // Apply effect
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Lerp(0.3f, chromaticAberrationMax, Mathf.Pow(effectStrength, 2));

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(0f, lensDistortionMax, effectStrength);

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(0.2f, vignetteMax, effectStrength);
    }
}
