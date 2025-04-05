using UnityEngine;
using UnityEngine.Rendering;

public class CubeColorChecker : MonoBehaviour
{
    public GameObject floorCubesParent;
    public GameObject wallCubesParent;
    public int numberOfCubes = 36;
    public DoorOpen doorOpen;

    public void CompareCubeColors()
    {
        int crtCompared = 0;
        foreach (Transform floorCube in floorCubesParent.transform)
        {
            Transform wallCube = wallCubesParent.transform.Find(floorCube.name);
            
            if (wallCube != null)
            {
                Color floorColor = floorCube.GetComponent<Renderer>().material.color;
                Color wallColor = wallCube.GetComponent<Renderer>().material.color;

                if (ColorsMatchRounded(floorColor, wallColor))
                    crtCompared++;
                else
                {
                    Debug.Log("DIFFERENT: ");
                    Debug.Log(floorColor);
                    Debug.Log(wallColor);
                }
                    
            }
        }
        if (crtCompared >= numberOfCubes)
            doorOpen.OpenDoor();
        else
            doorOpen.CloseDoor();
        
    }

    bool ColorsMatchRounded(Color a, Color b)
    {
        return Mathf.RoundToInt(a.r * 255) == Mathf.RoundToInt(b.r * 255) &&
               Mathf.RoundToInt(a.g * 255) == Mathf.RoundToInt(b.g * 255) &&
               Mathf.RoundToInt(a.b * 255) == Mathf.RoundToInt(b.b * 255);
    }


}
