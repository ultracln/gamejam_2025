using UnityEngine;

public class EmissionPulse : MonoBehaviour
{
    public Renderer targetRenderer; // Assign the Renderer in Inspector
    public float minEmission = 1f;
    public float maxEmission = 4f;
    public float speed = 1f;

    private Material runtimeMaterial;
    private Color baseEmissionColor;

    void Start()
    {
        // Make a runtime instance so we don't modify the shared material
        runtimeMaterial = targetRenderer.material;

        // Cache the base emission color once
        baseEmissionColor = runtimeMaterial.GetColor("_EmissionColor");

        runtimeMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed * Mathf.PI * 2f) + 1f) / 2f;
        float emissionStrength = Mathf.Lerp(minEmission, maxEmission, t);

        runtimeMaterial.SetColor("_EmissionColor", baseEmissionColor * emissionStrength);
    }
}
