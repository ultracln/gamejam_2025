using System.Collections.Generic;
using UnityEngine;

public class CloneReplayer : MonoBehaviour
{
    public List<ActionRecorder.PlayerAction> actions = new List<ActionRecorder.PlayerAction>();
    private int currentIndex = 0;

    void Update()
    {
        if (actions == null || actions.Count == 0) return;

        // Play through the recorded actions based on time
        if (currentIndex < actions.Count)
        {
            transform.position = actions[currentIndex].position;
            currentIndex++;
        }
    }
}
