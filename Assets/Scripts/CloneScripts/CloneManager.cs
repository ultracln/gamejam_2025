using System.Collections.Generic;
using UnityEngine;

public class CloneManager : MonoBehaviour
{
    public static List<List<ActionRecorder.PlayerAction>> allClones = new List<List<ActionRecorder.PlayerAction>>();
    public static List<GameObject> allCloneObjects = new List<GameObject>(); // New: Track clone GameObjects

    public int maxClones = 3;

    public void AddClone(List<ActionRecorder.PlayerAction> actions)
    {
        allClones.Add(new List<ActionRecorder.PlayerAction>(actions));
    }


    public void RegisterCloneObject(GameObject clone)
    {
        allCloneObjects.Add(clone);
    }

    public void Clear()
    {
        foreach (var clone in allCloneObjects)
        {
            if (clone != null) Destroy(clone);
        }

        allClones.Clear();
        allCloneObjects.Clear();
    }

    public List<List<ActionRecorder.PlayerAction>> GetAllClones()
    {
        return allClones;
    }
}
