using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CountDownTimer : MonoBehaviourPunCallbacks
{

    public Text timerText;
    public Image badge;
    public SoundController soundController;
    public const byte changeCountDownCode = 2;
    public GameController gameController;
    public float timeLeftOnceSpotted = 240f;

    private bool timerStarted = false;
    private bool endStarted = false;

    void Start()
    {
        timerText.text = "";
        badge.color = new Color(1,1,1,0);
        soundController = GameObject.FindObjectOfType<SoundController>();
        PhotonNetwork.NetworkingClient.EventReceived += OnchangeCounterEvent;
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

        if (PhotonNetwork.IsMasterClient)
        {
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                changeCountDownTimer(300f);
                gameController.playerUpdates.updateDisplay("Time change to: " + 300f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                changeCountDownTimer(360f);
                gameController.playerUpdates.updateDisplay("Time change to: " + 360f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                changeCountDownTimer(420f);
                gameController.playerUpdates.updateDisplay("Time change to: " + 420f);

            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                changeCountDownTimer(1f);
                gameController.playerUpdates.updateDisplay("Added 60 seconds to timer");

            }
        }
    }
    IEnumerator endTimer()
    {
        yield return new WaitForSeconds(0.5f);
        timerStarted = true;
    }

    public void changeCountDownTimer(float newTime)
    {
        object[] content = new object[] { newTime };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(changeCountDownCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnchangeCounterEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        
        if (eventCode == changeCountDownCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            float newTime = (float)data[0];

            if(newTime == 1f)
            {
                timeLeftOnceSpotted += 60f;
            }
            else
            {
                timeLeftOnceSpotted = newTime;
            }

            
        }
    }

}
