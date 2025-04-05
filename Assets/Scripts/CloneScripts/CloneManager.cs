using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CloneManager: MonoBehaviour
{
    public List<List<ActionRecorder.PlayerAction>> allClones = new List<List<ActionRecorder.PlayerAction>>();
    public int maxClones = 3;

    public void AddClone(List<ActionRecorder.PlayerAction> actions)
    {
        if (allClones.Count >= maxClones)
        {
            // TODO: full clones logic
            allClones.RemoveAt(0); // remove oldest
        }

        allClones.Add(actions);
    }

    public void Clear()
    {
        allClones.Clear();
    }
}
