using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorHandler : MonoBehaviourPun
{

    public PressButton button;
    private List<int> l = new List<int>();

    // Start is called before the first frame update
    public DoorScript doorleft, doorright;
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "Timmy" && button.done) {
            StartCoroutine(openDoor(doorleft));
            StartCoroutine(openDoor(doorright));

            this.gameObject.GetComponent<PhotonView>().RPC("Enter", RpcTarget.Others);
        }
    }

    [PunRPC]
    void Enter()
    {
        l.Add(1);
    }

    IEnumerator openDoor(DoorScript door) {
        door.OpenDoor();
        yield return null;
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.name == "Timmy" && button.done) {

            this.gameObject.GetComponent<PhotonView>().RPC("Exit", RpcTarget.Others);
        }
    }

    [PunRPC]
    void Exit()
    {
        if (l.Count != 0)
        {
            l.RemoveAt(0);
        }

        if (l.Count == 0)
        {
            doorleft.CloseDoor();
            doorright.CloseDoor();
        }
    }
}
