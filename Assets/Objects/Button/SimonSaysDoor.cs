using UnityEngine;

public class SimonSaysDoor : MonoBehaviour {

    public DoorOpen doorOpen;
    void Start() {
        doorOpen.OpenDoor();
    }

    public void CloseTheDoor()
    {
        doorOpen.CloseDoor();
    }
}
