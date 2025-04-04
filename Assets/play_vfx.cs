using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class play_vfx : MonoBehaviour

{

    public Volume globalVolume;  // Assign your Global Volume in the Inspector
    public KeyCode activationKey = KeyCode.P; // Change to any key you want
    public float effectDuration = 2f; // How long the effect lasts

    VolumeProfile profile;
    ChromaticAberration chromaticAberration; // Example effect


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (globalVolume != null)
        {
            profile = globalVolume.profile;

            // Check if Vignette exists in the profile
            if (!profile.TryGet(out chromaticAberration))
            {
                Debug.LogError("Vignette effect not found in Volume Profile!");
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            StartCoroutine(ApplyEffect());
        }


    }

    private IEnumerator ApplyEffect()
    {
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = 1f; // Set effect intensity
            yield return new WaitForSeconds(effectDuration);
            chromaticAberration.intensity.value = 0f; // Reset effect
        }
    }
}
