using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
//using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class PreGameLobbyController : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    public GUISkin myskin = null;
    public GameObject SpawnPoint;
    public GameObject EscapeMenu;



    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
    private ExitGames.Client.Photon.Hashtable customPropertiesRoom = new ExitGames.Client.Photon.Hashtable();



    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!EscapeMenu.activeSelf)
            {
                EscapeMenu.SetActive(true);
            }
            else if (EscapeMenu.activeSelf)
            {
                EscapeMenu.SetActive(false);
            }
        }
        
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
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, SpawnPoint.transform.position, Quaternion.identity);

        customProperties.Add("ready", "false");
        customPropertiesRoom = PhotonNetwork.CurrentRoom.CustomProperties;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        Debug.Log("Spawned a player");

    }


    void OnGUI()
    {
        GUI.skin = myskin;

        if (PhotonNetwork.CurrentRoom == null)
            return;

        

        GUI.Label(new Rect(Screen.width-250, 35,200,40), "Players Ready: "+(int)PhotonNetwork.CurrentRoom.CustomProperties["num_ready"] + "/" + PhotonNetwork.CurrentRoom.MaxPlayers);

        //Show the list of the players connected to this Room
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //Show if this player is a Master Client. There can only be one Master Client per Room so use this to define the authoritative logic etc.)
            //string isMasterClient = (PhotonNetwork.PlayerList[i].IsMasterClient ? ": MasterClient" : "");
            GUI.Label(new Rect(5, 35 + 30 * i, 200, 35), PhotonNetwork.PlayerList[i].NickName);
            GUI.Label(new Rect(135, 35 + 30 * i, 200, 35), (string) PhotonNetwork.PlayerList[i].CustomProperties["ready"] );

        }
    }

    public void SetReady() 
    {
        Debug.Log("Set Ready Function");
        customProperties["ready"] = "true";
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        Debug.Log("Ready state has beed updated to :" + (string)customProperties["ready"]);
    }

    public void SetUnReady()
    {
        Debug.Log("Set Unready Function");
        customProperties["ready"] = "false";
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
     
        Debug.Log("Ready state has beed updated to :" + (string)customProperties["ready"]);
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

    public void CheckAllReady() 
    {
        int total_ready = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) 
        {
            if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("false")) 
            {
                return;
            }
            total_ready ++;

        }
        if (total_ready > 0) {
            PhotonNetwork.LoadLevel("BuildScene");
            
        }   

    }

    //Go back to main meny when you leave game
    public override void OnLeftRoom()
    {
        //We have left the Room, return back to the GameLobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
    }
}
