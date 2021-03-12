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
        int numOfSpecial = 3;
        SetProps(numOfSpecial);
        List<GameObject> starItems = new List<GameObject>();
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            starItems = SetupItems(numOfSpecial);
        }

        foreach (GameObject item in starItems)
        {
            Debug.Log("instantiated starsprite");
            PhotonNetwork.InstantiateRoomObject("StarItem", new Vector3(item.transform.position.x, 16.1f, item.transform.position.z-1), Quaternion.Euler(90,0,0));
        }
        
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


    // Set score to 0 && special item numbers
    void SetProps(int numOfSpecial) {
        Hashtable setPlayer = new Hashtable() {{"score", 0}, {"itemsStolen", 0}, {"specialStolen", 0}, {"leave", false}, {"win", false}};
		PhotonNetwork.LocalPlayer.SetCustomProperties(setPlayer);      

        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            Hashtable setRoom = new Hashtable() {{"score", 0}, {"special", 0}, {"specialMax", numOfSpecial}, {"win", false}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(setRoom);
        }
    }

    List<GameObject> SetupItems(int numOfSpecial) {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("steal");

        // Generate 3 random numbers within the range of the objects
        List<int> rand = RandomExtension(0, objs.Length, numOfSpecial);
        List<GameObject> specialObjs = new List<GameObject>();

        for (int i = 0; i < objs.Length; i++){
            int gameSelection = r.Next(0,3);
            PhotonView view = objs[i].GetComponent<PhotonView>();
            if (rand.Contains(i)) {
                int value = Random.Range(60, 100) * 100;
                objs[i].GetComponent<CollectableItem>().UpdateObject(true, value,gameSelection);
                view.RPC("UpdateObject", RpcTarget.All, true, value,gameSelection);
                specialObjs.Add(objs[i]);
            } else {
                int value = Random.Range(10, 40) * 100;
                objs[i].GetComponent<CollectableItem>().UpdateObject(false, value,gameSelection);
                view.RPC("UpdateObject", RpcTarget.All, false, value,gameSelection);
            } 
        }

        return specialObjs;
    }

    List<int> RandomExtension(int x, int y, int n) {
        List<int> rand = new List<int>();

        while (rand.Count < n)
        {
            int num = Random.Range(x, y);
            if (!rand.Contains(num)) {
                rand.Add(num);
            }
        }

        return rand;
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
