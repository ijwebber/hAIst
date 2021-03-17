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
    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
    private ExitGames.Client.Photon.Hashtable customPropertiesRoom = new ExitGames.Client.Photon.Hashtable();



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
        }
        else
        {
            customProperties["ready"] = "false";
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
            Debug.Log("Ready state has beed updated to :" + (string)customProperties["ready"]);
        }
        
        
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log(targetPlayer.NickName + " changed a value");
        int num_ready = GetNumReady();
        customPropertiesRoom["num_ready"] = num_ready;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customPropertiesRoom);
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
