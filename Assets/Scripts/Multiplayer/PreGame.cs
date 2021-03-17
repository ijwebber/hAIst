using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
//using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class PreGame : MonoBehaviourPunCallbacks
{

    public Button StartGameButton;
    public Button SetReadyButton;
    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
    private ExitGames.Client.Photon.Hashtable customPropertiesRoom = new ExitGames.Client.Photon.Hashtable();

    public GameObject thief_1;
    public GameObject thief_2;
    public GameObject thief_3;
    public GameObject thief_4;




    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        //Debug.Log("USERID: "+PhotonNetwork.LocalPlayer.UserId);

        
    }

    // Update is called once per frame
    void Update()
    {
   
        
    }

    //just spawns in player object
    private void Awake()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Is not in the room, returning back to Lobby");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
            return;
        }

        customProperties.Add("ready", "false");
        customPropertiesRoom = PhotonNetwork.CurrentRoom.CustomProperties;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

    }


    public void SetReady() 
    {
        Debug.Log("Set Ready Function");
        if (customProperties["ready"].Equals("false"))
        {
            customProperties["ready"] = "true";
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
            Debug.Log("Ready state has beed updated to :" + (string)customProperties["ready"]);
            SetReadyButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            customProperties["ready"] = "false";
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
            Debug.Log("Ready state has beed updated to :" + (string)customProperties["ready"]);
            SetReadyButton.GetComponent<Image>().color = Color.red;
        }
        
        
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log(targetPlayer.NickName + " changed a value");
        int num_ready = GetNumReady();
        customPropertiesRoom["num_ready"] = num_ready;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customPropertiesRoom);
        SetReadyChecks();
        CheckAllReady();

    }

    public void QuitButton()
    {
        PhotonNetwork.LeaveRoom();   
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("ArtLevel");
    }
    
    public int GetNumReady()
    {
        Debug.Log("GET NUM READY");
        int total_ready = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) 
        {
            if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("true")) 
            {
                total_ready ++;
            }
       }
        return total_ready;
    }


    public void SetReadyChecks()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_1.GetComponentInChildren<Text>().text))
            {
              if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("true"))
              {
                  Debug.Log("READY CHECK");
                  thief_1.transform.GetChild(1).gameObject.SetActive(true);
              }
              else
              {
                  thief_1.transform.GetChild(1).gameObject.SetActive(false);
              }
            }
            else if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_2.GetComponentInChildren<Text>().text))
            {
              if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("true"))
              {
                  Debug.Log("READY CHECK");
                  thief_2.transform.GetChild(1).gameObject.SetActive(true);
              }
              else
              {
                  thief_2.transform.GetChild(1).gameObject.SetActive(false);
              }
            }
            else if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_3.GetComponentInChildren<Text>().text))
            {
              if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("true"))
              {
                  Debug.Log("READY CHECK");
                  thief_3.transform.GetChild(1).gameObject.SetActive(true);
              }
              else
              {
                  thief_3.transform.GetChild(1).gameObject.SetActive(false);
              }
            }
            else if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_4.GetComponentInChildren<Text>().text))
            {
              if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("true"))
              {
                  Debug.Log("READY CHECK");
                  thief_4.transform.GetChild(1).gameObject.SetActive(true);
              }
              else
              {
                  thief_4.transform.GetChild(1).gameObject.SetActive(false);
              }
            }
        }
    }

    public bool CheckAllReady() 
    {
        int total_ready = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) 
        {
            if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("false")) 
            {
                StartGameButton.interactable = false;
                return false;
            }
            total_ready ++;

        }
        if (total_ready > 0) {
            //PhotonNetwork.LoadLevel("ArtLevel");
            if (PhotonNetwork.IsMasterClient)
            {
                StartGameButton.interactable = true;
            }
            return true; 
        }
        StartGameButton.interactable = false;
        return false;
    }


    //Go back to main meny when you leave game
    public override void OnLeftRoom()
    {
        //We have left the Room, return back to the GameLobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
    }
}
