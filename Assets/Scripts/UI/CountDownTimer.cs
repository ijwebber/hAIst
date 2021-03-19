using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CountDownTimer : MonoBehaviourPunCallbacks
{

    public Text timerText;
    private float timeLeftOnceSpotted = 240f;

    private bool timerStarted = false;

    void Start()
    {

        timerText.text = "";

    }
    public override void OnRoomPropertiesUpdate(Hashtable setSpotted)
    {
        base.OnRoomPropertiesUpdate(setSpotted);

        if (setSpotted["spotted"] != null)
        {
            if ((bool)setSpotted["cutSceneDone"])
            {
                StartCoroutine(endTimer());
            }

        }
    }
    private void Update()
    {
        
        if (timeLeftOnceSpotted > 0 && timerStarted)
        {
            

            float minutes = Mathf.FloorToInt(timeLeftOnceSpotted / 60);
            float seconds = Mathf.FloorToInt(timeLeftOnceSpotted % 60);
            if (seconds >= 10)
            {
                timerText.text = "Police will arrive in " + "0" + minutes + ":" + seconds;
            }
            else
            {   
                timerText.text = "Police will arrive in " + "0" + minutes + ":" +  "0" + seconds;
            }

            timeLeftOnceSpotted -= Time.deltaTime;
        }
        else if(timerStarted)
        {
            timerText.text = "";

            

            Hashtable endHash = new Hashtable() { { "end", true }, { "win", false } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(endHash);
            
        }
    }
    IEnumerator endTimer()
    {
        yield return new WaitForSeconds(0.5f);
        timerStarted = true;
    }

}
