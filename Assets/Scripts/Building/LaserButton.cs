using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LaserButton : MonoBehaviour
{
    public PressButton button;

    // Update is called once per frame
    void Update()
    {
        // Disables laser if buttons have been pressed
        if (button.done)
        {
            this.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
        }
    }
}
