using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{

    public PressButton button;

    // Start is called before the first frame update
    public DoorScript doorleft, doorright;
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "Timmy" && button.done) {
            StartCoroutine(openDoor(doorleft));
            StartCoroutine(openDoor(doorright));
        }
    }

    IEnumerator openDoor(DoorScript door) {
        door.OpenDoor();
        yield return null;
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.name == "Timmy" && button.done) {
            doorleft.CloseDoor();
            doorright.CloseDoor();
        }
    }
}
