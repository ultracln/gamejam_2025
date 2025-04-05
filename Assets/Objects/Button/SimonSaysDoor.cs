using UnityEngine;

public class SimonSaysDoor : MonoBehaviour {

    public DoorOpen doorOpen;

    public DoorOpen door2;
    void Start() {
        doorOpen.OpenDoor();
    }

    public void CloseTheDoor()
    {
        doorOpen.CloseDoor();
    }

    public void OpenTheDoor()
    {
        door2.OpenDoor();
    }
}
