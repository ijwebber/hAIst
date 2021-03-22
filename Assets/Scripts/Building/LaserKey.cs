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
        if (keypad.codeCorrect)
        {
            this.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.Others);
            this.GetComponent<LineRenderer>().enabled = false;
            if (this.GetComponent<Laser>() != null)
            {
                this.GetComponent<Laser>().disabled = true;
            }
            else
            {
                this.GetComponent<LaserDown>().disabled = true;
            }
        }
    }
}
