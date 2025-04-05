using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class play_vfx : MonoBehaviour
{
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
        // Abort effect entirely if max clones are already reached
        /*if (CloneManager.allClones.Count >= cloneManager.maxClones)
        {
            ResetEffects(); // Optional: fade out instantly if already active
            return;
        }*/

        if (Input.GetMouseButton(1) && !effectLocked && !isFadingOut)
        {
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

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = chromaticAberrationMin;

        if (lensDistortion != null)
            lensDistortion.intensity.value = lensDistortionMin;

        if (vignette != null)
            vignette.intensity.value = vignetteMin;
    }
}