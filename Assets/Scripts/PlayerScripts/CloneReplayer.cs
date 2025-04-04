using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class CloneReplayer : MonoBehaviour
{
    public List<ActionRecorder.PlayerAction> actions = new List<ActionRecorder.PlayerAction>();

    private StarterAssetsInputs inputScript;
    public int currentIndex = 0;

    void Start()
    {
        inputScript = GetComponent<StarterAssetsInputs>();

        if (inputScript == null)
        {
            Debug.LogError("GhostReplayer: StarterAssetsInputs component not found on ghost.");
        }
    }

    void Update()
    {
        if (actions == null || actions.Count == 0 || currentIndex >= actions.Count) return;

        var action = actions[currentIndex];

        // Feed the recorded inputs into the input system used by the animator
        inputScript.move = action.moveInput;
        inputScript.look = action.lookInput;
        inputScript.jump = action.jump;
        inputScript.sprint = action.sprint;

        currentIndex++;
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    // Setter for currentIndex (optional)
    public void SetCurrentIndex(int index)
    {
        currentIndex = index;
    }
}
