using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameController : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject guardPrefab;
    public GameObject guardPrefab2;
    public GameObject guardPrefab3;
    public SoundVisual soundMesh;
    public GameObject SpawnPoint;
    public GUISkin myskin = null;
    public GameObject EscapeMenu;


    private Inventory inventory;

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
        //PhotonNetwork.Instantiate(guardPrefab.name, new Vector3(-36.33f, 13.363f, 6.43f), Quaternion.identity);


        // Set score custom props
        SetProps();

        //PhotonNetwork.InstantiateRoomObject(guardPrefab.name, guardPrefab.transform.position, Quaternion.identity);
        //PhotonNetwork.InstantiateRoomObject(guardPrefab2.name, guardPrefab2.transform.position, Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject(guardPrefab3.name, guardPrefab3.transform.position, Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject(soundMesh.name, soundMesh.transform.position, soundMesh.transform.rotation);

        Debug.Log("Spawned a player");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        
    }

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

    public void QuitButton()
    {
        PhotonNetwork.LeaveRoom();   
    }



    // Leave Game button
    /*void OnGUI()
    {
        GUI.skin = myskin;

        if (PhotonNetwork.CurrentRoom == null)
            return;


        //Show the Room name
        //GUI.Label(new Rect(135, 5, 200, 25), PhotonNetwork.CurrentRoom.Name);

        //Show the list of the players connected to this Room
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //Show if this player is a Master Client. There can only be one Master Client per Room so use this to define the authoritative logic etc.)
            //string isMasterClient = (PhotonNetwork.PlayerList[i].IsMasterClient ? ": MasterClient" : "");
            GUI.Label(new Rect(5, 35 + 30 * i, 200, 25), PhotonNetwork.PlayerList[i].NickName);
        }
    }*/

    //Go back to main meny when you leave game
    public override void OnLeftRoom()
    {
        //We have left the Room, return back to the GameLobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
        //inventory.Hide();
    }


    // Set players score custom property to 0 
    void SetProps() {
        Hashtable setScore = new Hashtable() {{"score", 0}};
		PhotonNetwork.LocalPlayer.SetCustomProperties(setScore);
        PhotonNetwork.CurrentRoom.SetCustomProperties(setScore);

        Hashtable setSpecial = new Hashtable() {{"special", 0}, {"specialMax", 3}};
        PhotonNetwork.CurrentRoom.SetCustomProperties(setSpecial);
    }

}
