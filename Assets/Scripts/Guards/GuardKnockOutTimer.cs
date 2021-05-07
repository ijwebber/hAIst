using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;


public class GuardKnockOutTimer : MonoBehaviour
{

    public TextMeshProUGUI statusText;


    public bool timerDisplayed = false;
    public Vector3 offset = new Vector3(0, 3, -0.1f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GuardMovement moveScript = GetComponent<GuardMovement>();

        //if guard is disabled and timer is currently not being displayed then send an RPC to everyone to display the 3 second disabled timer on top of the guard
        if (moveScript.guardDisabled && !timerDisplayed )
        {
            //PhotonView.Get(this).RPC("InstiateTimer", RpcTarget.All);



        }
    }

    

    //Just makes the text object and updates it everysecond
    IEnumerator secondsText()
    {   
        timerDisplayed = true;
        
        
        
        
        statusText.text = "3";
        yield return new WaitForSeconds(1.0f);
        statusText.text = "2";
        yield return new WaitForSeconds(1.0f);
        statusText.text = "1";
        yield return new WaitForSeconds(1.0f);
        statusText.text = "";
        timerDisplayed = false;

    }

    //This is called by everyone to start the timer on their client
    [PunRPC]
    void InstiateTimer()
    {
        StartCoroutine(secondsText());
    }
}
