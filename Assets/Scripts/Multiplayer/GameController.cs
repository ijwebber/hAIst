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

// centralised game settings
public class GameController : MonoBehaviourPunCallbacks
{

    public GameObject UpgradeUI;
    public GameObject UpgradeUIPrefab;
    public GameObject PlayerController;

    [SerializeField] private GameObject tutorial;
    public GameObject MuteButton;
    public Sprite sound_on;
    public Sprite sound_off;
    private bool mute;

    public GameObject OptionsPanel;


    public GameObject playerPrefab;
    public int gameState = 0;
    public GameObject guardPrefab;
    public GameObject guardPrefab2;
    public GameObject guardPrefab3;
    public GameObject sleepyGuard;
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

    private void Awake()
    {
#if UNITY_EDITOR
    Debug.unityLogger.logEnabled = true;
#else
    Debug.unityLogger.logEnabled = false;
#endif
        //GameLobbyScript = GameObject.Find("_GameLobby").GetComponent<PUN2_GameLobby1>();
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Is not in the room, returning back to Lobby");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
            return;
        }
        playerUsername = PhotonNetwork.NickName;
        foreach (var play in PhotonNetwork.PlayerList)
        {
            playerList.Add(play.NickName);
        }
        questPointer = GameObject.FindObjectOfType<Window_QuestPointer>();

        int actNo = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[i]) {
                actNo = i;
            }
        }
        // spawn in various players
        float xSpawnPos = SpawnPoint.transform.position.x + (float) (actNo * 0.6);
        Vector3 spawnpoint = SpawnPoint.transform.position;
        spawnpoint.x = xSpawnPos;
        spawnpoint.y = 11f;
        string prefab_to_instantiate = "character_prefab_" + PlayerPrefs.GetString("skin");
        GameObject player = PhotonNetwork.Instantiate(prefab_to_instantiate, spawnpoint, Quaternion.identity);
        playerCam.Follow = player.gameObject.transform.Find("Timmy").transform;
        playerCam.LookAt = player.gameObject.transform.Find("Timmy").transform;

        // Set custom props
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            int numOfSpecial = 0;
            numOfSpecial = SetupItems();
            SetProps(numOfSpecial);
        }

        // set up special objects
        GameObject[] stealObjs = GameObject.FindGameObjectsWithTag("steal");
        foreach (GameObject obj in stealObjs)
        {
            if (obj.GetComponent<CollectableItem>().special) {
                specialItems.Add(obj);
            }
        }


        SetSpotted();
        
        
        // spawn guards
        PhotonNetwork.InstantiateRoomObject(guardPrefab.name, guardPrefab.transform.position, Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject(guardPrefab2.name, guardPrefab2.transform.position, Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject(guardPrefab3.name, guardPrefab3.transform.position, Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject(sleepyGuard.name, sleepyGuard.transform.position, Quaternion.identity);
        if (PhotonNetwork.IsMasterClient) {
            GameObject.FindObjectOfType<GuardController>().setGuards();
        }
        
        // PhotonNetwork.InstantiateRoomObject(soundMesh.name, soundMesh.transform.position, soundMesh.transform.rotation);

        Debug.Log("Spawned a player");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        mute = false;
        
    }

    public void gameStart() {
        // init game state
        gameState = 0;
        tutorial.GetComponent<Animator>().SetTrigger("startTutorial");
    }

    // consumable upgrade DB handling
    public void SetReviveUsed()
    {
        foreach (Transform child in UpgradeUI.transform)
        {
            if (child.GetChild(1).gameObject.GetComponent<Image>().sprite.name.Equals("revive"))
            {
                child.GetChild(2).gameObject.SetActive(true);
            }
        }
    }
    public void SetShieldUsed()
    {
        foreach (Transform child in UpgradeUI.transform)
        {
            if (child.GetChild(1).gameObject.GetComponent<Image>().sprite.name.Equals("shield"))
            {
                child.GetChild(2).gameObject.SetActive(true);
            }
        }
    }
    public void PopulateUpgradeUI()
    {
        Debug.Log("Populating Upgrade UI");
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
    public void ResumeGame()
    {
        EscapeMenu.SetActive(false);
    }

    public void EnableOptionsPanel()
    {
        OptionsPanel.SetActive(true);
    }
    public void DisableOptionsPanel()
    {
        OptionsPanel.SetActive(false);
    }

    public void MutePressed()
    {
        if (mute)
        {
            MuteButton.GetComponent<Image>().sprite = sound_on;
            mute = false;
            AudioListener.volume = 1;

        }
        else
        {
            MuteButton.GetComponent<Image>().sprite = sound_off;
            mute = true;
            AudioListener.volume = 0;
        }


    }


    void Update()
    {
        // escape menu on ESC pressed
        if (Input.GetKeyDown(KeyCode.Escape) && CamSystem.GetComponent<CameraSystem>().introDone)
        {
            //PopulateUpgradeUI();
            EscapeMenu.SetActive(!EscapeMenu.activeSelf);
            OptionsPanel.SetActive(false);

        } 

        // display update when someone leaves
        if (PhotonNetwork.PlayerList.Length < playerList.Count) {
            List<string> currentList = new List<string>(playerList);
            foreach (var play in PhotonNetwork.PlayerList)
            {
                if (playerList.Contains(play.NickName)) {
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
            bool localRegress = regress;
            bool localUpdateNeeded = updateNeeded;
            if (!updateNeeded) {
                questBox.newQuest();
                questMarker.newQuest();
                pager.newQuest();
                if (gameState > updatedGameState) {
                    // change originated from this client
                    updatedGameState = gameState;
                } else if (regress) {
                    // if game state has decreased
                    if (gameState < updatedGameState) {
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
            // gamestate controller
            switch (gameState) 
            {
                case 0: // starting state
                    playerUpdates.updateDisplay("Game started");
                    playerUpdates.updateDisplay("Press TAB to check your pager for tips");
                    setNewQuest(new List<GameObject>() {GameObject.Find("MetalDoorHandler")}, new List<string> {"Look around"}, localRegress);
                    break;
                case 1: // point to code
                    setNewQuest(new List<GameObject>() {GameObject.Find("Entrance code display"), GameObject.Find("Entrance keypad")}, new List<string> {"Find and enter the code"}, localRegress);
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
                                    if (item != null) {
                                        newText.Add("Steal " + item.GetComponent<CollectableItem>().itemName);
                                        nextQuestItems.Add(item);
                                    } else {
                                        newText.Add("<i><s>Steal " + item.GetComponent<CollectableItem>().itemName + "</i></s>");
                                    }
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
                                    if (item != null) {
                                        newText.Add("Steal " + item.GetComponent<CollectableItem>().itemName);
                                        nextQuestItems.Add(item);
                                    } else {
                                        newText.Add("<i><s>Steal " + item.GetComponent<CollectableItem>().itemName + "</i></s>");
                                    }
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
                                    if (item != null) {
                                        newText.Add("Steal " + item.GetComponent<CollectableItem>().itemName);
                                        nextQuestItems.Add(item);
                                    } else {
                                        newText.Add("<i><s>Steal " + item.GetComponent<CollectableItem>().itemName + "</i></s>");
                                    }
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
        /*if (PhotonNetwork.IsMasterClient)
        {
            ChangeMasterClientifAvailble();
            PhotonNetwork.SendAllOutgoingCommands();
        }
        else
        {
            PhotonNetwork.LeaveRoom();

        }*/
        PhotonNetwork.LeaveRoom();
    }

    public void ChangeMasterClientifAvailble()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            return;
        }

        PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient.GetNext());
    }

    public void updateQuest() {
        this.photonView.RPC("updateThis", RpcTarget.All);
    }

    [PunRPC]
    // sync quest update
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
        foreach (Player p in PhotonNetwork.PlayerList) {
            Hashtable setPlayer = new Hashtable() {{"score", 0}, {"downs",0}, {"revives", 0}, {"alerts",0}, {"itemsStolen", 0}, {"specialStolen", 0}, {"leave", false}, {"win", false}, {"disabled", false}};
		    p.SetCustomProperties(setPlayer);     
        }

        Hashtable setRoom = new Hashtable() {{"score", 0}, {"special", 0}, {"specialMax", numOfSpecial}, {"win", false}, {"isDriverBusy", false}, {"specialStolen", 0}};
        PhotonNetwork.CurrentRoom.SetCustomProperties(setRoom);
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
                totalSpecial += 1;
            } else {
                value = Random.Range(10, 40) * 100;                
            }

            objs[i].GetComponent<CollectableItem>().UpdateObjectMaster(value, gameSelection);
            view.RPC("UpdateObject", RpcTarget.All, value, gameSelection);
        }

        return totalSpecial;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProps) {
        base.OnRoomPropertiesUpdate(changedProps);

        if (changedProps["end"] != null) {
            if ((bool) changedProps["end"]) {
                if (PhotonNetwork.IsMasterClient) {
                    PhotonNetwork.LoadLevel("EndScreen");
                }
            }
        } else if (changedProps["special"] != null) {
            int i = (int) changedProps["special"] - 1;

            int remaining = (int) PhotonNetwork.CurrentRoom.CustomProperties["specialMax"] - (int) changedProps["special"];
            if (remaining == 0) {

                if (!CameraSystem.Instance.disableCutScenes)
                {
                    StartCoroutine(CameraSystem.Instance.explodeExitCutScene());
                }
                /*
                GameObject[] objs = GameObject.FindGameObjectsWithTag("exit");
                foreach (GameObject tag in objs)
                {
                    GameObject mainObj = tag.transform.parent.gameObject;
                    mainObj.SetActive(false);
                }*/
            }
        }
    }

    // update quest and pointer
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

    // display message on other clients
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
