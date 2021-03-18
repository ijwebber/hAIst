using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public DoorScript doorleft, doorright;
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "Timmy") {
            StartCoroutine(openDoor(doorleft));
            StartCoroutine(openDoor(doorright));
        }
    }

    IEnumerator openDoor(DoorScript door) {
        door.OpenDoor();
        yield return null;
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.name == "Timmy") {
            doorleft.CloseDoor();
            doorright.CloseDoor();
        }
    }
}
