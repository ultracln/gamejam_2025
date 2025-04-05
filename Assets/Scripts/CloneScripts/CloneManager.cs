using System.Collections.Generic;
using System.Numerics;

public static class CloneManager
{
    public static List<List<ActionRecorder.PlayerAction>> allClones = new List<List<ActionRecorder.PlayerAction>>();
    public static int maxClones = 3;

    public static void AddClone(List<ActionRecorder.PlayerAction> actions)
    {
        if (allClones.Count >= maxClones)
        {
            // TODO: full clones logic
            allClones.RemoveAt(0); // remove oldest
        }

        allClones.Add(actions);
    }

    public static void Clear()
    {
        allClones.Clear();
    }
}
