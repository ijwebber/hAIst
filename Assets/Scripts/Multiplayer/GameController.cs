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
    public GameObject starSprite;
    public SoundVisual soundMesh;
    public GameObject SpawnPoint;
    public GUISkin myskin = null;
    public GameObject EscapeMenu;

    System.Random r = new System.Random();

    //just spawns in player object
    private void Awake()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Is not in the room, returning back to Lobby");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
            return;
        }

        float xSpawnPos = SpawnPoint.transform.position.x + (float) (PhotonNetwork.LocalPlayer.ActorNumber * 0.6);
        Vector3 spawnpoint = SpawnPoint.transform.position;
        spawnpoint.x = xSpawnPos;

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnpoint, Quaternion.identity);

        // Set custom props
        int numOfSpecial = 0;
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            numOfSpecial = SetupItems(numOfSpecial);
        }
        SetProps(numOfSpecial);

        SetSpotted();
        
        PhotonNetwork.InstantiateRoomObject(guardPrefab.name, guardPrefab.transform.position, Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject(guardPrefab2.name, guardPrefab2.transform.position, Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject(guardPrefab3.name, guardPrefab3.transform.position, Quaternion.identity);
        // PhotonNetwork.InstantiateRoomObject(soundMesh.name, soundMesh.transform.position, soundMesh.transform.rotation);

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
    }
    // Set spotted to false as the players have not been seen by any guards, if a player is seen guard calls police and players have to escape in a set time.
    void SetSpotted()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Hashtable setSpotted = new Hashtable() { { "spotted", false }, {"spottingGuardLocation", null }, {"cutSceneDone", false } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(setSpotted);
        }
    }

    // Set score to 0 && special item numbers
    void SetProps(int numOfSpecial) {
        Hashtable setPlayer = new Hashtable() {{"score", 0}, {"itemsStolen", 0}, {"specialStolen", 0}, {"leave", false}, {"win", false}};
		PhotonNetwork.LocalPlayer.SetCustomProperties(setPlayer);      

        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            Hashtable setRoom = new Hashtable() {{"score", 0}, {"special", 0}, {"specialMax", numOfSpecial}, {"win", false}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(setRoom);
        }
    }

    // Returns number of special items
    int SetupItems() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("steal");
        
        int totalSpecial = 0;

        for (int i = 0; i < objs.Length; i++){
            int gameSelection = r.Next(0,3);
            PhotonView view = objs[i].GetComponent<PhotonView>();
            int value;
            if (objs[i].GetComponent<CollectableItem>().special) {
                value = Random.Range(60, 100) * 100;
                special += 1;
            } else {
                value = Random.Range(10, 40) * 100;                
            }

            objs[i].GetComponent<CollectableItem>().UpdateObject(value, gameSelection);
            view.RPC("UpdateObject", RpcTarget.All, false, value, gameSelection);
        }

        return totalSpecial;
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProps) {
        base.OnRoomPropertiesUpdate(changedProps);

        if (changedProps["end"] != null) {
            if ((bool) changedProps["end"]) {
                PhotonNetwork.LoadLevel("EndScreen");
            }
        }
    }


}
