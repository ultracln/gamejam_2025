using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System;

public class PlayerOnTeleporter : MonoBehaviour
{
    public float timeToStayOnTeleporter = 1f;
    public string targetObjectName = "SM_Teleporter"; // Name of the object to check
    private float timeOnObject = 0f; // Tracks time player stays on the object
    public string targetColorBox = "colorBox";
    public Color[] highlightColors;
    private Dictionary<int, Coroutine> activeCoroutines = new Dictionary<int, Coroutine>();
    private Dictionary<int, Color> pastColors = new Dictionary<int, Color>(); // Make sure this is properly filled


    public CubeColorChecker checker = null;
    public SimonSaysDoor simonSaysDoor;
    public SimonSaysSequence simonSaysSequence = null;

    // This is your "history" of active highlights
    private List<List<int>> highlightTimeline = new List<List<int>>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == targetObjectName)
        {
            timeOnObject = 0f; // Reset timer when entering
        }

        if (other.gameObject.name.StartsWith(targetColorBox))
        {
            Renderer renderer = other.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color yellow;
                Color gray;
                ColorUtility.TryParseHtmlString("#FFBF3B", out yellow);
                gray = new Color(0.684f, 0.684f, 0.684f, 1f);

                if (renderer.material.color == yellow)
                    renderer.material.color = gray;
                else
                    renderer.material.color = yellow;

                checker.CompareCubeColors();
            }
        }

        if (other.gameObject.name == "PlaySimonSaysBox")
        {
            Renderer renderer = other.GetComponent<Renderer>();
            if (renderer.material.color != Color.black)
            {
                simonSaysSequence.playColors();
                simonSaysDoor.CloseTheDoor();

                renderer.material.color = Color.black;
            }
        }

        if (other.gameObject.name.StartsWith("ColoredBox"))
        {
            Renderer renderer = other.GetComponent<Renderer>();
            string numberPart = other.gameObject.name.Substring("ColoredBox".Length); // Get everything after "ColoredBox"

            if (int.TryParse(numberPart, out int boxNumber))
            {
                // Store the original color if we haven't already
                if (!pastColors.ContainsKey(boxNumber))
                {
                    pastColors[boxNumber] = renderer.material.color;
                }

                // Stop previous coroutine for this box, if any
                if (activeCoroutines.TryGetValue(boxNumber, out Coroutine existingCoroutine))
                {
                    StopCoroutine(existingCoroutine);
                    renderer.material.color = pastColors[boxNumber];
                }

                // Start new coroutine and store reference
                Coroutine newCoroutine = StartCoroutine(ChangeColorTemporary(boxNumber, renderer, highlightColors[boxNumber], 3f));
                activeCoroutines[boxNumber] = newCoroutine;

                // Record current active highlight box numbers
                List<int> currentActive = new List<int>(activeCoroutines.Keys);
                highlightTimeline.Add(currentActive);

                if (highlightTimeline.Count > 5)
                {
                    highlightTimeline.RemoveAt(0); // Remove the oldest
                }

                if (AreTimelinesEqual(highlightTimeline, RememberSequence.targetPattern))
                {
                    simonSaysDoor.OpenTheDoor();
                }
            }
        }
    }

    bool AreTimelinesEqual(List<List<int>> a, List<List<int>> b)
    {
        if (a.Count != b.Count)
            return false;

        for (int i = 0; i < b.Count; i++)
        {
            List<int> listA = new List<int>(a[i]);
            List<int> listB = new List<int>(b[i]);

            // Sort both to compare without caring about order inside each inner list
            listA.Sort();
            listB.Sort();

            if (listA.Count != listB.Count)
                return false;

            for (int j = 0; j < listA.Count; j++)
            {
                if (listA[j] != listB[j])
                    return false;
            }
        }

        return true;
    }


    private IEnumerator ChangeColorTemporary(int boxNumber, Renderer renderer, Color newColor, float duration)
    {
        renderer.material.color = newColor;
        yield return new WaitForSeconds(duration);

        // Revert to original color
        if (pastColors.ContainsKey(boxNumber))
        {
            renderer.material.color = pastColors[boxNumber];
        }

        // Remove from coroutine tracking
        activeCoroutines.Remove(boxNumber);
    }



    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == targetObjectName)
        {
            timeOnObject += Time.deltaTime; // Increment time while staying

            if (timeOnObject >= timeToStayOnTeleporter) // If stayed for 1 second or more
            {
                LoadNextScene();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == targetObjectName)
        {
            timeOnObject = 0f; // Reset timer when leaving
        }
    }

    private void LoadNextScene()
    {
        // Get the current scene name
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Try to convert scene name into a number
        if (int.TryParse(currentSceneName, out int sceneNumber))
        {
            // Generate the next scene name (increment the number)
            string nextSceneName = (sceneNumber + 1).ToString("00"); // Keeps format like "01", "02", "03"

            // Check if the next scene exists before loading (recommended)
            if (Application.CanStreamedLevelBeLoaded(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogError("Scene " + nextSceneName + " does not exist!");
            }
        }
        else
        {
            Debug.LogError("Invalid scene name format: " + currentSceneName);
        }
    }
}
