using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KnockOutGuard : MonoBehaviour
{   

    private bool inRangeOfGuard = false;
    private int guardViewID = -1;
    private GameObject tt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject guard = null;

        //if text object exists from previous frame, delete it as we need to update it for this frame
        if (tt)
        {
            Destroy(tt);
        }

        //if this player is behind a guard then get the guard gameobject using the id
        if (guardViewID != -1)
        {
            guard = PhotonNetwork.GetPhotonView(guardViewID).gameObject;
        }

        
        //if no longer in range set flags to prevent knocking out of range guard
        if (guard && !(Mathf.Abs(guard.transform.position.x - transform.position.x) <= 1.5f) && !(Mathf.Abs(guard.transform.position.z - transform.position.z) <= 1.5f))
        {
            inRangeOfGuard = false;
            guardViewID = -1;
        } 
        else if (guard && !guard.GetComponent<GuardMovement>().guardDisabled && guardViewID != -1) //if not already disabled, display locally to the player "E" to say that the player should press E to disable this guard
        {
            GuardKnockOutTimer knockoutscript = guard.GetComponent<GuardKnockOutTimer>();
            tt = Instantiate(knockoutscript.floatingTextPrefab, guard.transform.position + new Vector3(-0.1f, 3f, 0f), Quaternion.identity, guard.transform);
            tt.GetComponent<TextMesh>().text = "E";

            //if pressed destroy "E" text as timer text will take its place and set the guard's disabled flag to true
            if (Input.GetKeyDown(KeyCode.E))
            {

                Destroy(tt);
                guard.GetComponent<GuardMovement>().guardDisabled = true;

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
