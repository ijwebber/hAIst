using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LaserKey : MonoBehaviour
{
    public KeyPad keypad;

    // Update is called once per frame
    void Update()
    {
        // Disables laser if correct code has been entered
        if (keypad.codeCorrect)
        {
            this.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
        }
    }
}
