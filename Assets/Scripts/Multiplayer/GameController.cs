using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameController : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    public int gameState = 0;
    public GameObject guardPrefab;
    public GameObject guardPrefab2;
    public GameObject guardPrefab3;
    public TextMeshProUGUI objectiveText;
    public GameObject starSprite;
    public SoundVisual soundMesh;
    public GameObject SpawnPoint;
    public GUISkin myskin = null;
    private int updatedGameState = -1;
    public GameObject EscapeMenu;
    private List<GameObject> specialItems = new List<GameObject>();
    private Window_QuestPointer questPointer;

    public string playerUsername;
    [SerializeField] private NewQuest questBox;
    [SerializeField] private NewQuest questMarker;

    [SerializeField] private PlayerUpdates playerUpdates;

    System.Random r = new System.Random();

    //just spawns in player object
    private void Awake()
    {
        //GameLobbyScript = GameObject.Find("_GameLobby").GetComponent<PUN2_GameLobby1>();
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Is not in the room, returning back to Lobby");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
            return;
        }
        playerUsername = PhotonNetwork.NickName;
        questPointer = GameObject.FindObjectOfType<Window_QuestPointer>();

        float xSpawnPos = SpawnPoint.transform.position.x + (float) (PhotonNetwork.LocalPlayer.ActorNumber * 0.6);
        Vector3 spawnpoint = SpawnPoint.transform.position;
        spawnpoint.x = xSpawnPos;

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnpoint, Quaternion.identity);

        // Set custom props
        int numOfSpecial = 0;
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            numOfSpecial = SetupItems();
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
        // for info testing
        // if (Input.GetKeyDown(KeyCode.M)) {
        //     playerUpdates.updateDisplay("M was just pressed");
        // }
        if (gameState != updatedGameState) {
            questBox.newQuest();
            questMarker.newQuest();
            updatedGameState = gameState;
            List <string> newText = new List<string>();
            switch (gameState)
            {
                case 0: // starting state
                    playerUpdates.updateDisplay("Game started");
                    updateDisp("Game started");
                    setNewQuest(new List<GameObject>() {GameObject.Find("MetalDoorHandler")}, new List<string> {"Look around"});
                    break;
                case 1: // point to code
                    setNewQuest(new List<GameObject>() {GameObject.Find("Entrance code display")}, new List<string> {"Find the code"});
                    break;
                case 2: // point to key objects
                    playerUpdates.updateDisplay("The key door has been unlocked");
                    updateDisp("The key door has been unlocked");
                    foreach (var item in specialItems) {
                        newText.Add("Steal " + item.GetComponent<CollectableItem>().itemName);
                    }
                    setNewQuest(specialItems, newText);
                    break;
                case 3: // point to key object 2
                    playerUpdates.updateDisplay("You have stolen a key painting");
                    updateDisp(PhotonNetwork.NickName + " has stolen a key painting");
                    List <GameObject> toSteal = new List<GameObject>();
                    foreach (var item in specialItems)
                    {
                        if (!item.GetComponent<CollectableItem>().stolen) {
                            toSteal.Add(item);
                            newText.Add("Steal " + item.GetComponent<CollectableItem>().itemName);
                        } else {
                            newText.Add("<s>Steal " + item.GetComponent<CollectableItem>().itemName + "</s>");
                        }
                    }
                    setNewQuest(toSteal, newText);
                    break;
                case 4: // point to exit
                    playerUpdates.updateDisplay("You have stolen a key painting");
                    updateDisp(PhotonNetwork.NickName + " has stolen a key painting");
                    setNewQuest(new List<GameObject>() {GameObject.Find("Van")}, new List<string> {"Get out!"});
                    break;
                default:
                    break;
            }
        }
    }

    public void QuitButton()
    {
        PhotonNetwork.LeaveRoom();   
    }

    //Go back to main meny when you leave game
    public override void OnLeftRoom()
    {
        Debug.Log("On left Room callback");
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.LoadLevel("GameLobby 1");
        Debug.Log("gamelobby 1 loaded");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        //PhotonNetwork.LoadLevel("GameLobby 1");
         //SceneManager.LoadScene("GameLobby 1");


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
        Hashtable setPlayer = new Hashtable() {{"score", 0}, {"itemsStolen", 0}, {"specialStolen", 0}, {"leave", false}, {"win", false}, {"disabled", false}};
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
                specialItems.Add(objs[i]);
                totalSpecial += 1;
            } else {
                value = Random.Range(10, 40) * 100;                
            }

            objs[i].GetComponent<CollectableItem>().UpdateObject(value, gameSelection);
            view.RPC("UpdateObject", RpcTarget.All, value, gameSelection);
        }

        return totalSpecial;
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProps) {
        base.OnRoomPropertiesUpdate(changedProps);

        if (changedProps["end"] != null) {
            if ((bool) changedProps["end"]) {
                if (PhotonNetwork.IsMasterClient) {
                    PhotonNetwork.LoadLevel("EndScreen");
                }
            }
        }
    }
    private void setNewQuest(List<GameObject> objs, List<string> objectives) {
        if (objs.Count == 0) {
            // questPointer.GetComponent<PhotonView>().RPC("updateTarget", RpcTarget.All, "null", gameState);
        } else {
            string[] serialisedObjects = new string[objs.Count];
            for (int i = 0; i < objs.Count; i++)
            {
                serialisedObjects[i] = objs[i].name;
            }
            this.GetComponent<PhotonView>().RPC("updateTarget", RpcTarget.All, serialisedObjects, gameState, objectives.ToArray());
        }
    }

    private void updateDisp(string message) {
        this.GetComponent<PhotonView>().RPC("displayMessage", RpcTarget.Others, message);
    }

    [PunRPC]
    void displayMessage(string message) {
        playerUpdates.updateDisplay(message);
    }

    [PunRPC]
    void updateTarget(string[] gameNames, int localgameState, string[] objectives) {
        gameState = localgameState;
        string newObjectives = "";
        foreach (string objective in objectives)
        {
            newObjectives += objective + "\n";
        }
        objectiveText.text = newObjectives;
        questPointer.updateTarget(gameNames, gameState);
    }
}
