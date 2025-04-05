using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonSaysSequence : MonoBehaviour
{
    public GameObject[] boxes; // Assign in Inspector
    public Color[] highlightColors; // Assign in code or Inspector
    public float flashDuration = 1f;
    public int sequenceLength = 5; // Number of flashes per run

    private Color[] originalColors;

    void Start()
    {
        originalColors = new Color[boxes.Length];

        for (int i = 0; i < boxes.Length; i++)
        {
            Renderer rend = boxes[i].GetComponent<Renderer>();
            rend.material = new Material(rend.material); // Ensure unique material
            originalColors[i] = rend.material.color;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(PlayRandomColorSequence());
        }
    }

    IEnumerator PlayRandomColorSequence()
    {
        List<List<int>> sequence = new List<List<int>>();

        for (int i = 0; i < sequenceLength; i++)
        {
            if (i == 3)
            {
                // 4th step: 2 random boxes
                sequence.Add(GetUniqueRandomIndices(2));
            }
            else if (i == 4)
            {
                // 5th step: 3 random boxes
                sequence.Add(GetUniqueRandomIndices(3));
            }
            else
            {
                // Steps 1–3: single box
                sequence.Add(new List<int> { Random.Range(0, boxes.Length) });
            }
        }

        // Debug print the sequence
        Debug.Log("Generated Sequence: ");
        foreach (var step in sequence)
        {
            string stepString = string.Join(", ", step);
            Debug.Log("Step: [" + stepString + "]");
        }

        // Play the sequence
        foreach (List<int> step in sequence)
        {
            // Off pause
            SetAllBoxesToOriginal();
            yield return new WaitForSeconds(0.2f);

            // Highlight all boxes in this step
            foreach (int index in step)
            {
                boxes[index].GetComponent<Renderer>().material.color = highlightColors[index];
            }

            yield return new WaitForSeconds(flashDuration);

            // Turn them off again
            SetAllBoxesToOriginal();
            yield return new WaitForSeconds(0.2f);
        }


        SetAllBoxesToOriginal();
    }

    void SetAllBoxesToOriginal()
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].GetComponent<Renderer>().material.color = originalColors[i];
        }
    }

    List<int> GetUniqueRandomIndices(int count)
    {
        List<int> indices = new List<int>();
        while (indices.Count < count)
        {
            int rand = Random.Range(0, boxes.Length);
            if (!indices.Contains(rand))
            {
                indices.Add(rand);
            }
        }
        return indices;
    }
}