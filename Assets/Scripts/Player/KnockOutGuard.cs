using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class KnockOutGuard : MonoBehaviourPun
{   

    private bool inRangeOfGuard = false;
    private int guardViewID = -1;
    
    private TextMeshProUGUI guardStatusText;
    private AudioController audioController;
    public GameObject guard;
    // Start is called before the first frame update
    void Start()
    {
        audioController = GameObject.FindObjectOfType<AudioController>();
    }

    // Update is called once per frame
    void Update()
    {
        //if text object exists from previous frame, delete it as we need to update it for this frame
        // if (guardStatusText && guard && !guard.GetComponent<GuardMovement>().guardDisabled)
        // {
        //     guardStatusText.text = "";
        // }

        //if this player is behind a guard then get the guard gameobject using the id
        if (guardViewID != -1)
        {
            guard = PhotonNetwork.GetPhotonView(guardViewID).gameObject;
            //guardStatusText = guard.GetComponent<GuardKnockOutTimer>().statusText;
        }

        
        //if no longer in range set flags to prevent knocking out of range guard
        if (guard && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(guard.transform.position.x, guard.transform.position.z)) >= 5)
        {
            inRangeOfGuard = false;
            guardViewID = -1;
            guard = null;
        } 
        else if (guard && !guard.GetComponent<GuardMovement>().guardDisabled && guardViewID != -1 && !GetComponent<PlayerMovement>().disabled) //if not already disabled, display locally to the player "E" to say that the player should press E to disable this guard
        {
            GuardKnockOutTimer knockoutscript = guard.GetComponent<GuardKnockOutTimer>();
            
            // guardStatusText.text = "E";
            
            //if pressed destroy "E" text as timer text will take its place and set the guard's disabled flag to true
            if (Input.GetKeyDown(KeyCode.E))
            {
                // guardStatusText.text = "";
                audioController.PlayGuardGrunt();
                guard.GetComponent<GuardMovement>().removeSpecials();
                guard.GetComponent<GuardMovement>().guardDisabled = true;
                guard.GetComponent<PhotonView>().RPC("syncGuardDisabled", RpcTarget.All, true);
                // guard.GetComponent<GuardMovement>().transferSpecials(this.GetComponent<PlayerController>());
            }
        }
       

        //need to reset these as guard will send an RPC next frame anyways if this player is behind it
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
