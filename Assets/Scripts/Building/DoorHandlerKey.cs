﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorHandlerKey : MonoBehaviourPun
{

    public KeyPad keyPad;
    bool opened = false;
    private List<int> l = new List<int>();

    // Start is called before the first frame update
    public DoorScript doorleft, doorright;
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "Timmy" && keyPad.codeCorrect && !opened) {
            this.gameObject.GetComponent<PhotonView>().RPC("EnterMetal", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void EnterMetalDone() {
        opened = true;
    }

    [PunRPC]
    void EnterMetal() {
        if (!opened) {
            StartCoroutine(openDoor(doorleft));
            StartCoroutine(openDoor(doorright));
            opened = true;
            this.gameObject.GetComponent<PhotonView>().RPC("EnterMetalDone", RpcTarget.Others);
        }
    }

    IEnumerator openDoor(DoorScript door) {
        door.OpenDoor();
        yield return null;
    }
}
