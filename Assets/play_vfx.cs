using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class play_vfx : MonoBehaviour
{
    public AudioClip soundClip;

    public Volume globalVolume;
    public float maxHoldTime = 2f;  // Exact time before auto-reset
    public float effectDecreaseSpeed = 3f;  // Speed of effect fading

    public float chromaticAberrationMax = 1f;
    public float chromaticAberrationMin = 0f;

    public float lensDistortionMax = -0.75f;
    public float lensDistortionMin = 0f;

    public float vignetteMax = 0.5f;
    public float vignetteMin = 0f;

    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private Vignette vignette;

    public CloneManager cloneManager;

    private float holdTime = 0f;
    private bool effectLocked = false;
    private bool isFadingOut = false;

    private bool soundPlayed = false;  // To track if sound has been played
    private AudioSource currentAudioSource;

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

        if (Input.GetMouseButton(1) && !effectLocked && !isFadingOut)
        {
            // Play sound if it's not already played
            if (!soundPlayed)
            {
                currentAudioSource = SFXManager.instance.playSoundFX(soundClip, transform, 1f); // Store the reference to the AudioSource
                soundPlayed = true;
            }
            else if (currentAudioSource != null && !currentAudioSource.isPlaying)
            {
                // If the sound is paused, unpause it
                currentAudioSource.UnPause();
            }

            holdTime += Time.deltaTime;

            if (holdTime >= maxHoldTime)
            {
                isFadingOut = true;
            }
        }
        else if (isFadingOut)
        {
            holdTime -= Time.deltaTime * effectDecreaseSpeed;

            if (holdTime <= 0f)
            {
                holdTime = 0f;
                isFadingOut = false;
                effectLocked = true;
            }
        }
        else if (!Input.GetMouseButton(1))
        {
            if (currentAudioSource != null && currentAudioSource.isPlaying)
            {
                // Pause the sound when the mouse button is released
                currentAudioSource.Pause();
            }

            // Reset sound flag when the effect ends
            soundPlayed = false;

            holdTime -= Time.deltaTime * effectDecreaseSpeed;
            effectLocked = false;
        }

        holdTime = Mathf.Clamp(holdTime, 0f, maxHoldTime);
        float effectStrength = holdTime / maxHoldTime;

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberrationMin, chromaticAberrationMax, Mathf.Pow(effectStrength, 2));

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(lensDistortionMin, lensDistortionMax, effectStrength);

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(vignetteMin, vignetteMax, effectStrength);
    }

    private void ResetEffects()
    {
        holdTime = 0f;
        isFadingOut = false;
        effectLocked = false;
        soundPlayed = false;  // Reset sound flag

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = chromaticAberrationMin;

        if (lensDistortion != null)
            lensDistortion.intensity.value = lensDistortionMin;

        if (vignette != null)
            vignette.intensity.value = vignetteMin;
    }
}
