using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorHandler : MonoBehaviourPun
{

    public PressButton button;
    [SerializeField] private AudioController audioController;
    private List<int> l = new List<int>();

    public DoorScript doorleft, doorright;
    void OnTriggerEnter(Collider other) {
        // trigger door open if button is triggered
        if (other.gameObject.name == "Timmy" && button.done) {
            StartCoroutine(openDoor(doorleft));
            StartCoroutine(openDoor(doorright));

            this.gameObject.GetComponent<PhotonView>().RPC("Enter", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void Enter()
    {
        // add 1 to list. Used for ensuring all players have left before closing door
        l.Add(1);
    }

    IEnumerator openDoor(DoorScript door) {
        door.OpenDoor();
        yield return null;
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.name == "Timmy" && button.done) {
            this.gameObject.GetComponent<PhotonView>().RPC("Exit", RpcTarget.MasterClient);
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
            this.gameObject.GetComponent<PhotonView>().RPC("CloseDoors", RpcTarget.All);
        }
    }

    [PunRPC]
    void CloseDoors() {
        doorleft.CloseDoor();
        doorright.CloseDoor();
    }
}
