using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CountDownTimer : MonoBehaviourPunCallbacks
{

    public Text timerText;
    public Image badge;
    public SoundController soundController;
    private float timeLeftOnceSpotted = 5f;

    private bool timerStarted = false;
    private bool endStarted = false;

    void Start()
    {
        timerText.text = "";
        badge.color = new Color(1,1,1,0);
        soundController = GameObject.FindObjectOfType<SoundController>();
}
    public override void OnRoomPropertiesUpdate(Hashtable endTriggered)
    {
        base.OnRoomPropertiesUpdate(endTriggered);

        if (endTriggered["triggered"] != null)
        {
            if ((bool)endTriggered["triggered"] && !timerStarted)
            {
                StartCoroutine(endTimer());
            }

        }
    }
    private void Update()
    {
        
        if (timeLeftOnceSpotted > 0 && timerStarted)
        {
            badge.color = new Color(1,1,1,1);
            

            float minutes = Mathf.FloorToInt(timeLeftOnceSpotted / 60);
            float seconds = Mathf.FloorToInt(timeLeftOnceSpotted % 60);
            if (seconds >= 10)
            {
                timerText.text = "0" + minutes + ":" + seconds;
            }
            else
            {   
                timerText.text = "0" + minutes + ":" +  "0" + seconds;
            }
            if (timeLeftOnceSpotted <= 30) {
                if (Mathf.FloorToInt(timeLeftOnceSpotted) % 2 == 1) {
                    timerText.color = new Color(1,1,1,1);
                } else {
                    timerText.color = new Color(0.8490566f,0,0,1);
                }
            }
            if (soundController.getSoundValue())
            {
                timeLeftOnceSpotted -= Time.deltaTime;
            }
            
        }
        else if(timerStarted && !endStarted)
        {
            timerText.text = "";
            badge.color = new Color(1,1,1,0);
            endStarted = true;

            if (!CameraSystem.Instance.disableCutScenes)
            {
                StartCoroutine(CameraSystem.Instance.playSwatScene());
            }
            
        }
    }
    IEnumerator endTimer()
    {
        yield return new WaitForSeconds(0.5f);
        timerStarted = true;
    }

}
