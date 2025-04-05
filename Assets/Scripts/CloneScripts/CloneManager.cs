using System.Collections.Generic;
using UnityEngine;

public class CloneManager : MonoBehaviour
{
    public static List<List<ActionRecorder.PlayerAction>> allClones = new List<List<ActionRecorder.PlayerAction>>();
    public int maxClones = 3;

    public void AddClone(List<ActionRecorder.PlayerAction> actions)
    {
        allClones.Add(new List<ActionRecorder.PlayerAction>(actions));
    }

    public void Clear()
    {
        allClones.Clear();
    }

    public List<List<ActionRecorder.PlayerAction>> GetAllClones()
    {
        return allClones;
    }
}
