using UnityEngine;

public class ChangeColorOnKeyPress : MonoBehaviour
{
    private Renderer rend;

    void Start()
    {
        // Get the Renderer component from the GameObject
        rend = GetComponent<Renderer>();

        // Important: Make sure to create a unique instance of the material
        rend.material = new Material(rend.material);
    }

    void Update()
    {
        // Change color to red when you press the space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rend.material.color = Color.red;
        }

        // Change color back to white when you press 'R'
        if (Input.GetKeyDown(KeyCode.R))
        {
            rend.material.color = Color.white;
        }
    }
}
