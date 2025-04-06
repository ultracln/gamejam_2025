using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerOnTeleporter : MonoBehaviour
{
    public float timeToStayOnTeleporter = 1f;
    public string targetObjectName = "SM_Teleporter"; // Name of the object to check
    private float timeOnObject = 0f; // Tracks time player stays on the object
    public string targetColorBox = "colorBox";
    public Color[] highlightColors;
    private Dictionary<int, Coroutine> activeCoroutines = new Dictionary<int, Coroutine>();
    private Dictionary<int, Color> pastColors = new Dictionary<int, Color>(); // Make sure this is properly filled

    public CloneManager cloneManager;
    public CubeColorChecker checker = null;
    public SimonSaysDoor simonSaysDoor = null;
    public SimonSaysSequence simonSaysSequence = null;
    private HashSet<int> currentlyHighlighted = new HashSet<int>();

    public AudioClip teleporterAudioClip;
    public AudioClip pressurePlateAudioClip;

    private float lastSimonSaysTriggerTime = -999f;
    private float simonSaysCooldown = 1f; // 1 second cooldown


    public void InitChecker()
    {
        if (checker == null)
        {
            checker = FindObjectOfType<CubeColorChecker>();
            if (checker == null)
            {
                Debug.LogWarning("CubeColorChecker not found in scene.");
            }
        }

        if (simonSaysDoor == null)
        {
            simonSaysDoor = FindObjectOfType<SimonSaysDoor>();
            if (simonSaysDoor == null)
            {
                Debug.LogWarning("simonSaysDoor not found in scene.");
            }
        }

        if (simonSaysSequence == null)
        {
            simonSaysSequence = FindObjectOfType<SimonSaysSequence>();
            if (simonSaysSequence == null)
            {
                Debug.LogWarning("simonSaysSequence not found in scene.");
            }
        }

        /*if (teleporterAudioClip == null)
        {
            teleporterAudioClip = FindObjectOfType<AudioClip>();
            if (teleporterAudioClip == null)
            {
                Debug.LogWarning("teleporterAudioClip not found in scene.");
            }
        }

        if (pressurePlateAudioClip == null)
        {
            pressurePlateAudioClip = FindObjectOfType<AudioClip>();
            if (pressurePlateAudioClip == null)
            {
                Debug.LogWarning("pressurePlateAudioClip not found in scene.");
            }
        }*/

        if (teleporterAudioClip == null)
        {
            teleporterAudioClip = Resources.Load<AudioClip>("Audio/teleporter");
            if (teleporterAudioClip == null)
            {
                Debug.LogWarning("teleporterAudioClip not found in Resources/Audio/TeleporterSound.");
            }
        }

        if (pressurePlateAudioClip == null)
        {
            pressurePlateAudioClip = Resources.Load<AudioClip>("Audio/pressure_plate");
            if (pressurePlateAudioClip == null)
            {
                Debug.LogWarning("pressurePlateAudioClip not found in Resources/Audio/PressurePlateSound.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == targetObjectName)
        {
            timeOnObject = 0f; // Reset timer when entering
            SFXManager.instance.playSoundFX(teleporterAudioClip, transform, 1f);
        }

        if (other.gameObject.name.StartsWith(targetColorBox))
        {
            SFXManager.instance.playSoundFX(pressurePlateAudioClip, transform, 1f);
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
            SFXManager.instance.playSoundFX(pressurePlateAudioClip, transform, 1f);
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
            if (Time.time - lastSimonSaysTriggerTime < simonSaysCooldown)
                return; // Ignore if triggered too recently

            lastSimonSaysTriggerTime = Time.time;
            SFXManager.instance.playSoundFX(pressurePlateAudioClip, transform, 1f);
            Renderer renderer = other.GetComponent<Renderer>();
            string numberPart = other.gameObject.name.Substring("ColoredBox".Length);

            if (int.TryParse(numberPart, out int boxNumber))
            {
                if (!pastColors.ContainsKey(boxNumber))
                    pastColors[boxNumber] = renderer.material.color;

                // Stop and reset previous coroutine
                if (activeCoroutines.TryGetValue(boxNumber, out Coroutine existingCoroutine))
                {
                    if (existingCoroutine != null)
                    {
                        StopCoroutine(existingCoroutine);
                        renderer.material.color = pastColors[boxNumber];
                    }
                }

                // Start new highlight
                Coroutine newCoroutine = StartCoroutine(ChangeColorTemporary(boxNumber, renderer, highlightColors[boxNumber], 1f));
                activeCoroutines[boxNumber] = newCoroutine;
                currentlyHighlighted.Add(boxNumber);

                // Always track time since last input
                float timeSinceLast = Time.time - StaticScene.lastHighlightTime;
                StaticScene.lastHighlightTime = Time.time;

                if (StaticScene.highlightTimeline.Count > 0 && timeSinceLast <= 1f && StaticScene.highlightTimeline[^1].Count == 1)
                {
                    // Add to the last group if within 1 second and only one item is there
                    StaticScene.highlightTimeline[^1].Add(boxNumber);
                }
                else
                {
                    // Add new group with current box
                    StaticScene.highlightTimeline.Add(new List<int> { boxNumber });
                }

                // Cap timeline to 4 entries
                if (StaticScene.highlightTimeline.Count > 4)
                {
                    StaticScene.highlightTimeline.RemoveAt(0);
                }



                // Debug print the sequence
                Debug.Log("Generated Sequence: ");
                foreach (var step in StaticScene.highlightTimeline)
                {
                    string stepString = string.Join(", ", step);
                    Debug.Log(stepString);
                }


                // Check match
                if (AreTimelinesEqual(StaticScene.highlightTimeline, RememberSequence.targetPattern))
                {
                    simonSaysDoor.OpenTheDoor();
                }
            }
        }
    }

    private IEnumerator ChangeColorTemporary(int boxNumber, Renderer renderer, Color newColor, float duration)
    {
        // Apply the highlight color
        renderer.material.color = newColor;

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        // Revert to original color if it exists
        if (pastColors.TryGetValue(boxNumber, out Color originalColor))
        {
            renderer.material.color = originalColor;
        }

        // Remove from the currently highlighted set
        currentlyHighlighted.Remove(boxNumber);

        // Clear the coroutine reference
        activeCoroutines[boxNumber] = null;
    }



    bool AreTimelinesEqual(List<List<int>> a, List<List<int>> b)
    {
        if (a == null || b == null) return false;
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == targetObjectName)
        {
            timeOnObject += Time.deltaTime; // Increment time while staying

            if (timeOnObject >= timeToStayOnTeleporter) // If stayed for 1 second or more
            {
                // record the new best
                float level_completion_time = TimerManager.Instance.GetTime();

                string currentSceneName = SceneManager.GetActiveScene().name;
                float high_score;

                // Check if the high score for the current scene exists
                if (StaticScene.sceneHighScores.ContainsKey(currentSceneName))
                {
                    high_score = StaticScene.sceneHighScores[currentSceneName]; // Retrieve the high score
                    StaticScene.sceneHighScores[currentSceneName] = Mathf.Min(level_completion_time, high_score);
                } else
                {
                    StaticScene.sceneHighScores.Add(currentSceneName, level_completion_time);
                }

                
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
                cloneManager.Clear();

                StaticScene.lastSceneName = SceneManager.GetActiveScene().name;
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
