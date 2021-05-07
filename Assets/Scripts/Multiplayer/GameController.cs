using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Cinemachine;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameController : MonoBehaviourPunCallbacks
{

    public GameObject UpgradeUI;
    public GameObject UpgradeUIPrefab;
    public GameObject PlayerController;

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
    public GameObject CamSystem;
    private int updatedGameState = -1;
    public GameObject EscapeMenu;
    public List<GameObject> specialItems = new List<GameObject>();
    private Window_QuestPointer questPointer;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private AudioController audioController;
    
    public CinemachineVirtualCamera playerCam;

    //Upgrade Sprites
    public Sprite ninja;
    public Sprite speed_boots;
    public Sprite fast_hands;
    public Sprite vision;
    public Sprite shield;
    public Sprite self_revive;
    
    
    public string playerUsername;
    private List<string> playerList = new List<string>();
    public bool regress = false;
    [SerializeField] private NewQuest questBox;
    [SerializeField] private NewQuest questMarker;
    [SerializeField] private Obejctives pager;

    [SerializeField] public PlayerUpdates playerUpdates;
    private bool updateNeeded = false;


    System.Random r = new System.Random();

    //just spawns in player object
    private void Awake()
    {
        //GameLobbyScript = GameObject.Find("_GameLobby").GetComponent<PUN2_GameLobby1>();
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Is not in the room, returning back to Lobby");
            audioController.GetComponent<PhotonView>().RPC("StopAll", RpcTarget.All);
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
            return;
        }
        playerUsername = PhotonNetwork.NickName;
        foreach (var play in PhotonNetwork.PlayerList)
        {
            playerList.Add(play.NickName);
        }
        questPointer = GameObject.FindObjectOfType<Window_QuestPointer>();

        float xSpawnPos = SpawnPoint.transform.position.x + (float) (PhotonNetwork.LocalPlayer.ActorNumber * 0.6);
        Vector3 spawnpoint = SpawnPoint.transform.position;
        spawnpoint.x = xSpawnPos;
        spawnpoint.y = 11f;
        string prefab_to_instantiate = "character_prefab_" + PlayerPrefs.GetString("skin");
        GameObject player = PhotonNetwork.Instantiate(prefab_to_instantiate, spawnpoint, Quaternion.identity);
        playerCam.Follow = player.gameObject.transform.Find("Timmy").transform;
        playerCam.LookAt = player.gameObject.transform.Find("Timmy").transform;

        

        


        // Set custom props
        int numOfSpecial = 0;
        // if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        numOfSpecial = SetupItems();
        // }
        SetProps(numOfSpecial);

        SetSpotted();
        
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.Instantiate(guardPrefab.name, guardPrefab.transform.position, Quaternion.identity);
            PhotonNetwork.Instantiate(guardPrefab2.name, guardPrefab2.transform.position, Quaternion.identity);
            PhotonNetwork.Instantiate(guardPrefab3.name, guardPrefab3.transform.position, Quaternion.identity);
        }
        // PhotonNetwork.InstantiateRoomObject(soundMesh.name, soundMesh.transform.position, soundMesh.transform.rotation);

        Debug.Log("Spawned a player");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        
    }

    private void PopulateUpgradeUI()
    {
        foreach (Transform child in UpgradeUI.transform)
        {   
            GameObject.Destroy(child.gameObject);
        }
        if (PlayerController.GetComponent<PlayerController>().upgrades.fast_hands > 0)
        {
            GameObject newObj; // Create GameObject instance
            newObj = (GameObject)Instantiate(UpgradeUIPrefab, UpgradeUI.transform);
            newObj.transform.GetChild(1).GetComponent<Image>().sprite = fast_hands;
        }
        if (PlayerController.GetComponent<PlayerController>().upgrades.speed_boots > 0)
        {
            GameObject newObj; // Create GameObject instance
            newObj = (GameObject)Instantiate(UpgradeUIPrefab, UpgradeUI.transform);
            newObj.transform.GetChild(1).GetComponent<Image>().sprite = speed_boots;
        }
        if (PlayerController.GetComponent<PlayerController>().upgrades.vision > 0)
        {
            GameObject newObj; // Create GameObject instance
            newObj = (GameObject)Instantiate(UpgradeUIPrefab, UpgradeUI.transform);
            newObj.transform.GetChild(1).GetComponent<Image>().sprite = vision;
        }
        if (PlayerController.GetComponent<PlayerController>().upgrades.ninja > 0)
        {
            GameObject newObj; // Create GameObject instance
            newObj = (GameObject)Instantiate(UpgradeUIPrefab, UpgradeUI.transform);
            newObj.transform.GetChild(1).GetComponent<Image>().sprite = ninja;
        }
        if (PlayerController.GetComponent<PlayerController>().upgrades.self_revive)
        {
            GameObject newObj; // Create GameObject instance
            newObj = (GameObject)Instantiate(UpgradeUIPrefab, UpgradeUI.transform);
            newObj.transform.GetChild(1).GetComponent<Image>().sprite = self_revive;
        }
        if (PlayerController.GetComponent<PlayerController>().upgrades.shield)
        {
            GameObject newObj; // Create GameObject instance
            newObj = (GameObject)Instantiate(UpgradeUIPrefab, UpgradeUI.transform);
            newObj.transform.GetChild(1).GetComponent<Image>().sprite = shield;
        }




    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CamSystem.GetComponent<CameraSystem>().introDone)
        {
            PopulateUpgradeUI();
            EscapeMenu.SetActive(!EscapeMenu.activeSelf);
        } 

        if (PhotonNetwork.PlayerList.Length < playerList.Count) {
            List<string> currentList = new List<string>(playerList);
            foreach (var play in PhotonNetwork.PlayerList)
            {
                if (playerList.Contains(play.NickName)) {
                    Debug.Log(play.NickName + " is still here");
                    currentList.Remove(play.NickName);
                }
            }
            playerList.Remove(currentList[0]);
            playerUpdates.updateDisplay(currentList[0] + " has left the heist");
            // updateDisp("A thief has left the heist");
        }
        // for info testing
        // if (Input.GetKeyDown(KeyCode.M)) {
        //     playerUpdates.updateDisplay("M was just pressed");
        // }
        if (gameState != updatedGameState || updateNeeded) {
            bool localChange = false;
            bool localRegress = regress;
            bool localUpdateNeeded = updateNeeded;
            if (!updateNeeded) {
                questBox.newQuest();
                questMarker.newQuest();
                pager.newQuest();
                if (gameState > updatedGameState) {
                    // change originated from here
                    updatedGameState = gameState;
                    localChange = true;
                } else if (regress) {
                    if (gameState < updatedGameState) {
                        Debug.Log("A PAINTING HAS BEEN CAPTURED!!");
                        localChange = true;
                        updatedGameState = gameState;
                    }
                    gameState = Mathf.Min(gameState,updatedGameState);
                    regress = false;
                }

                // gameState = updatedGameState;
                gameState = Mathf.Max(gameState, updatedGameState);
            } else {
                updateNeeded = false;
            }
            List <string> newText = new List<string>();
            List<GameObject> nextQuestItems = new List<GameObject>();
            switch (gameState) 
            {
                case 0: // starting state
                    playerUpdates.updateDisplay("Game started");
                    playerUpdates.updateDisplay("Press TAB to check your pager for tips");
                    setNewQuest(new List<GameObject>() {GameObject.Find("MetalDoorHandler")}, new List<string> {"Look around"}, localRegress);
                    break;
                case 1: // point to code
                    setNewQuest(new List<GameObject>() {GameObject.Find("Entrance code display")}, new List<string> {"Find the code"}, localRegress);
                    break;
                case 2: // point to key objects
                    foreach (var item in specialItems) {
                        if (item.GetComponent<CollectableItem>().stolen) {
                            newText.Add("<i><s>Steal " + item.GetComponent<CollectableItem>().itemName + "</i></s>");
                        } else {
                            if (item.GetComponent<CollectableItem>().guardPoint == null) {
                                if (item.GetComponent<CollectableItem>().hidden && !item.GetComponent<CollectableItem>().keyPad.GetComponent<KeyPad>().codeCorrect) {
                                    if (item.GetComponent<CollectableItem>().discovered) {
                                        newText.Add("Find code for back room");
                                        nextQuestItems.Add(item.GetComponent<CollectableItem>().codeDisplay);
                                    } else {
                                        newText.Add("Find " + item.GetComponent<CollectableItem>().itemName);
                                    }
                                    nextQuestItems.Add(item.GetComponent<CollectableItem>().keyPad);
                                } else {
                                    newText.Add("Steal " + item.GetComponent<CollectableItem>().itemName);
                                    nextQuestItems.Add(item);
                                }
                            } else {
                                newText.Add("Recapture " + item.GetComponent<CollectableItem>().itemName + " from the guards!");
                                nextQuestItems.Add(item.GetComponent<CollectableItem>().guardPoint);
                            }
                        }
                    }
                    setNewQuest(nextQuestItems, newText, localRegress);
                    break;
                case 3: // middle key objects stolen
                    foreach (var item in specialItems) {
                        if (item.GetComponent<CollectableItem>().stolen) {
                            newText.Add("<i><s>Steal " + item.GetComponent<CollectableItem>().itemName + "</i></s>");
                        } else {
                            if (item.GetComponent<CollectableItem>().guardPoint == null) {
                                if (item.GetComponent<CollectableItem>().hidden && !item.GetComponent<CollectableItem>().keyPad.GetComponent<KeyPad>().codeCorrect) {
                                    if (item.GetComponent<CollectableItem>().discovered) {
                                        newText.Add("Find code for back room");
                                        nextQuestItems.Add(item.GetComponent<CollectableItem>().codeDisplay);
                                    } else {
                                        newText.Add("Find " + item.GetComponent<CollectableItem>().itemName);
                                    }
                                    nextQuestItems.Add(item.GetComponent<CollectableItem>().keyPad);
                                } else {
                                    newText.Add("Steal " + item.GetComponent<CollectableItem>().itemName);
                                    nextQuestItems.Add(item);
                                }
                            } else {
                                newText.Add("Recapture " + item.GetComponent<CollectableItem>().itemName + " from the guards!");
                                nextQuestItems.Add(item.GetComponent<CollectableItem>().guardPoint);
                            }
                        }
                    }
                    setNewQuest(nextQuestItems, newText, localRegress);
                    break;
                case 4: // middle key objects stolen
                    foreach (var item in specialItems) {
                        if (item.GetComponent<CollectableItem>().stolen) {
                            newText.Add("<i><s>Steal " + item.GetComponent<CollectableItem>().itemName + "</i></s>");
                        } else {
                            if (item.GetComponent<CollectableItem>().guardPoint == null) {
                                if (item.GetComponent<CollectableItem>().hidden && !item.GetComponent<CollectableItem>().keyPad.GetComponent<KeyPad>().codeCorrect) {
                                    if (item.GetComponent<CollectableItem>().discovered) {
                                        newText.Add("Find code for back room");
                                        nextQuestItems.Add(item.GetComponent<CollectableItem>().codeDisplay);
                                    } else {
                                        newText.Add("Find " + item.GetComponent<CollectableItem>().itemName);
                                    }
                                    nextQuestItems.Add(item.GetComponent<CollectableItem>().keyPad);
                                } else {
                                    newText.Add("Steal " + item.GetComponent<CollectableItem>().itemName);
                                    nextQuestItems.Add(item);
                                }
                            } else {
                                newText.Add("Recapture " + item.GetComponent<CollectableItem>().itemName + " from the guards!");
                                nextQuestItems.Add(item.GetComponent<CollectableItem>().guardPoint);
                            }
                        }
                    }
                    setNewQuest(nextQuestItems, newText, localRegress);
                    break;
                case 5: // point to exit
                    setNewQuest(new List<GameObject>() {GameObject.Find("Van")}, new List<string> {"Get out!", "Optional: Steal as much as you can carry!"}, localRegress);
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

    public void updateQuest() {
        this.photonView.RPC("updateThis", RpcTarget.All);
    }

    [PunRPC]
    void updateThis() {
        updateNeeded = true;
    }


    //Go back to main meny when you leave game
    public override void OnLeftRoom()
    {
        Debug.Log("On left Room callback");
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        audioController.GetComponent<PhotonView>().RPC("StopAll", RpcTarget.All);
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
            Hashtable setRoom = new Hashtable() {{"score", 0}, {"special", 0}, {"specialMax", numOfSpecial}, {"win", false}, {"isDriverBusy", false}};
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
                    audioController.GetComponent<PhotonView>().RPC("StopAll", RpcTarget.All);
                    PhotonNetwork.LoadLevel("EndScreen");
                }
            }
        } else if (changedProps["special"] != null) {
            int i = (int) changedProps["special"] - 1;

            int remaining = (int) PhotonNetwork.CurrentRoom.CustomProperties["specialMax"] - (int) changedProps["special"];
            if (remaining == 0) {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("exit");
                foreach (GameObject tag in objs)
                {
                    GameObject mainObj = tag.transform.parent.gameObject;
                    mainObj.SetActive(false);
                }
            }
        }
    }
    private void setNewQuest(List<GameObject> objs, List<string> objectives, bool regressTest) {
        if (objs.Count == 0) {
            // questPointer.GetComponent<PhotonView>().RPC("updateTarget", RpcTarget.All, "null", gameState);
        } else {
            string[] serialisedObjects = new string[objs.Count];
            for (int i = 0; i < objs.Count; i++)
            {
                serialisedObjects[i] = objs[i].name;
            }
            this.GetComponent<PhotonView>().RPC("updateTarget", RpcTarget.All, serialisedObjects, gameState, regressTest, objectives.ToArray());
        }
    }

    public void updateDisp(string message) {
        this.GetComponent<PhotonView>().RPC("displayMessage", RpcTarget.Others, message);
    }

    [PunRPC]
    void displayMessage(string message) {
        playerUpdates.updateDisplay(message);
    }

    [PunRPC]
    void updateTarget(string[] gameNames, int localgameState, bool regressTest, string[] objectives) {
        regress = regressTest;
        updatedGameState = localgameState;
        string newObjectives = "";
        foreach (string objective in objectives)
        {
            newObjectives += objective + "\n";
        }
        objectiveText.text = newObjectives;
        questPointer.updateTarget(gameNames, updatedGameState);
    }
}
