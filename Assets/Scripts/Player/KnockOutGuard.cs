using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KnockOutGuard : MonoBehaviour
{   

    private bool inRangeOfGuard = false;
    private int guardViewID = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inRangeOfGuard && guardViewID != -1)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GuardMovement guardScript = PhotonNetwork.GetPhotonView(guardViewID).GetComponentInParent<GuardMovement>();
                guardScript.guardDisabled = true;
            }
        }

        inRangeOfGuard = false;
        guardViewID = -1;
    }


    [PunRPC]
    void KnockOut(int viewID)
    {
        inRangeOfGuard = true;
        guardViewID = viewID;
    }
}
