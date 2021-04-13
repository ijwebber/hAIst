using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


using Photon.Pun;
using Photon.Realtime;
using TMPro;

/* PLACES TO ADD NEW UPGRADES:
1. BuyUpgrade function
2. Upgrade image in upgrade menu (and description)
3. Image in inventory
4. Text object in pregamelobby script
5. Switch statement in DB_Controller
6. SetUsername function
*/
public class PUN2_GameLobby1 : MonoBehaviourPunCallbacks
{

    string gameVersion = "0.9";
    public Slider slider;
    public AnimateBG bg;
    public TextMeshProUGUI textAsset;
    public Slider Multiplier;
    bool joiningRoom = false;
    public GameObject DB_Controller;
    public GameObject PreGameScript;
    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    public int PlayerBalance;
    public Dictionary<string, int> PlayerInventory = new Dictionary<string, int>();

    // MENUS
    [SerializeField] private GameObject GuestMenu;
    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject UserManagerMenu;
    [SerializeField] private GameObject ExistingUserMenu;
    [SerializeField] private GameObject NewUserMenu;
    [SerializeField] private GameObject HomeMenu;
    [SerializeField] private GameObject LobbyMenu;
    [SerializeField] private GameObject NewLobbyMenu;

    [SerializeField] private GameObject PreGameMenu;
    [SerializeField] private GameObject CreditsMenu;
    [SerializeField] private GameObject RejoinWaitPanel;

    [SerializeField] private GameObject PreGameHome;
    [SerializeField] private GameObject MapScreen;

    // SCRIPTS
    [SerializeField] private GameObject menu_script;
    [SerializeField] private GameObject LobbyScript;
    [SerializeField] private GameObject ContentLobby;
    [SerializeField] private GameObject ContentFriends;
    [SerializeField] public GameObject ContentFriendsNew;




    // USER MANAGER GAMEOBJECTS
    public GameObject StatusGuest;

    // EXISTING USER GAMEOBJECTS
    public TMP_InputField UsernameLoginInput;
    [SerializeField] TMP_InputField PasswordLoginInput;
    public GameObject Status;

    // NEW USER GAMEOBJECTS
    public GameObject NewStatus;
    public TMP_InputField UsernameCreationInput;
    [SerializeField] TMP_InputField PasswordCreationInput;

    // GUEST GAMEOBJECTS
    [SerializeField] TMP_InputField UsernameInput;
    [SerializeField] private GameObject StartButton;
    private AuthenticationValues authValues;

    // HOME SCREEN GAMEOBJECTS
    public Button BalanceButton;
    public GameObject thief_1;
    public GameObject thief_1_home;
    public GameObject thief_2;
    public GameObject thief_3;
    public GameObject thief_4;
    public GameObject FriendsMenu;
    public GameObject MicMenu;
    private bool MicCheck;
    public GameObject Home_Home;
    public GameObject NewHome;
    public GameObject UpgradeMenu;
    public GameObject InventoryMenu;

    public GameObject UnlockPanel;
    public GameObject InventoryWaitPanel;

    public string[] FriendList;
    public GameObject AddFriendStatus;
    public TMP_InputField AddFriendInput;
    public bool IsGuest = false;
    public Button FriendsMenuButton;
    public Button UpgradesMenuButton;
    public GameObject BalanceInfoHome;

    public TMP_Text speed_boots_Inventory;
    public TMP_Text shield_Inventory;
    public TMP_Text vision_Inventory;
    public TMP_Text self_revive_Inventory;
    public TMP_Text fast_hands_Inventory;

    public TMP_Text speed_boots_InventoryNew;
    public TMP_Text shield_InventoryNew;
    public TMP_Text vision_InventoryNew;
    public TMP_Text self_revive_InventoryNew;
    public TMP_Text fast_hands_InventoryNew;



    // LOBBY MENU OBJECTS
    public Button BalanceButtonLobby;



    // PRE GAME OBJECTS
    public Button BalanceButtonPreGame;
    public GameObject BalanceInfoPre;
    public GameObject RoomNameButton;
    public GameObject LobbyScreen;
    public GameObject UpgradeScreen;

    // PHOTON NETWORK GAMEOBJECTS 

    // MAP SCREEN OBJECTS
    [SerializeField] private GameObject Notes;


    // Use this for initialization
    void Start()
    {
        
    }

    void Awake()
    {
        //DontDestroyOnLoad(this);
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("IsConnectedRejoin");
            ReJoinAfterLeave();
        }

    }

    void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
                //Set the App version before connecting
                PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
                // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
                PhotonNetwork.ConnectUsingSettings();
        }


    }


    // USER MANAGER MENU
    public void ExistingUser()
    {
        UserManagerMenu.SetActive(false);
        ExistingUserMenu.SetActive(true);
    }
    public void Guest()
    {
        UserManagerMenu.SetActive(false);
        GuestMenu.SetActive(true);
    }
    public void NewButton()
    {
        UserManagerMenu.SetActive(false);
        NewUserMenu.SetActive(true);
    }

    // NEW USER MENU
    public void NewAccount()
    {
        DB_Controller.GetComponent<DB_Controller>().Create(UsernameCreationInput.text, PasswordCreationInput.text);
    }

    // EXISTING USER MENU
    public void SignIn()
    {
        DB_Controller.GetComponent<DB_Controller>().Login(UsernameLoginInput.text, PasswordLoginInput.text);
    }

    // GUEST MENU
    public void SignInGuest()
    {
        DB_Controller.GetComponent<DB_Controller>().CheckUsername(UsernameInput.text);
    }

    // HOME MENU
    public void EnableFriendsMenu()
    {
        FriendsMenu.SetActive(true);
        //Home_Home.SetActive(false);
        NewHome.SetActive(false);
        UpgradeMenu.SetActive(false);
        //InventoryMenu.SetActive(false);
        NewLobbyMenu.SetActive(false);
        MicMenu.SetActive(false);
        MicCheck = false;
        AddFriendStatus.SetActive(false);
        ContentFriends.GetComponent<PopulateGridFriends>().OnRefresh();
    }

    public void EnableMicThreshold() {
        DB_Controller.GetComponent<DB_Controller>().getThresholds(PhotonNetwork.NickName);
        MicMenu.SetActive(true);
        MicCheck = true;
        NewHome.SetActive(false);
        NewLobbyMenu.SetActive(false);
        UpgradeMenu.SetActive(false);
        FriendsMenu.SetActive(false);
        AddFriendStatus.SetActive(false);
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    void Update() {
        if (MicMenu) {
            Microphone.Update();
        }
        string[] devices = Microphone.devices;

        float[] volumes = Microphone.volumes;

        if (devices.Length > 1) {
            int index = 0;
            string deviceName = devices[index];
            if (deviceName == null)
            {
                deviceName = string.Empty;
            }

            float volume = 0;
            if (Multiplier.value != null) {
                volume = volumes[index]*Multiplier.value;
            }
            slider.value = volume;
        }
    }
#endif
    public void updateSlider() {
        textAsset.text = Multiplier.value + " \n(Default = 240)";
    }

    public void EnableLobbyMenu()
    {
        FriendsMenu.SetActive(false);
        UpgradeMenu.SetActive(false);
        MicMenu.SetActive(false);
        //InventoryMenu.SetActive(false);
        NewLobbyMenu.SetActive(false);



        AddFriendStatus.SetActive(false);
        ContentFriendsNew.GetComponent<PopulateGridFriends>().OnRefresh();
        //Home_Home.SetActive(true);
        NewHome.SetActive(true);

    }

    public void EnableUpgradeMenu()
    {
        FriendsMenu.SetActive(false);
        AddFriendStatus.SetActive(false);
        //Home_Home.SetActive(false);
        NewHome.SetActive(false);

        //InventoryMenu.SetActive(false);
        NewLobbyMenu.SetActive(false);
        MicMenu.SetActive(false);

        GetInventory();
        UpgradeMenu.SetActive(true);
    }


    public void EnableRoomMenu()
    {
        FriendsMenu.SetActive(false);
        MicMenu.SetActive(false);
        MicCheck = false;
        AddFriendStatus.SetActive(false);
        NewHome.SetActive(false);

        //Home_Home.SetActive(false);
        UpgradeMenu.SetActive(false);

        //InventoryMenu.SetActive(true);
        LobbyScript.SetActive(true);
        NewLobbyMenu.SetActive(true);
        GetInventory();

    }

    IEnumerator UpdateFriendList()
    {
        for (; ; )
        {
            if (PhotonNetwork.IsConnectedAndReady && FriendList != null)
            {
                if (FriendList.Length == 1 & FriendList[0] == "")
                {
                }
                else
                {
                    PhotonNetwork.FindFriends(FriendList);
                }
                
            }
            yield return new WaitForSeconds(1f);

        }
    }

    public void GetFriends(string username, int type)
    {
        DB_Controller.GetComponent<DB_Controller>().GetFriends(username,type);
    }

    public void AddFriend()
    {
        DB_Controller.GetComponent<DB_Controller>().CheckIfExists(PhotonNetwork.NickName, AddFriendInput.text);
        
    }

    public void ChangeAddFriendInput()
    {
        AddFriendStatus.SetActive(false);
    }

    public void PlayButton()
    {
        LobbyScript.SetActive(true);
        LobbyMenu.SetActive(true);
    }

    // UPGRADES

    public void BuyUpgradeSpeedBoots()
    {
        if (PlayerBalance > 2000)
        {
            PlayerBalance = PlayerBalance - 2000;
            DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
            DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "speed_boots");

        }
    }

    public void BuyUpgradeShield()
    {
        if (PlayerBalance > 2000)
        {
            PlayerBalance = PlayerBalance - 2000;
            DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
            DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "shield");

        }
    }

    public void BuyUpgradeVision()
    {
        if (PlayerBalance > 2000)
        {
            PlayerBalance = PlayerBalance - 2000;
            DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
            DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "vision");

        }
    }


    public void BuyUpgradeSelfRevive()
    {
        if (PlayerBalance > 5000)
        {
            PlayerBalance = PlayerBalance - 5000;
            DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
            DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "self_revive");

        }
    }

    public void BuyUpgradeFastHands()
    {
        if (PlayerBalance > 5000)
        {
            PlayerBalance = PlayerBalance - 5000;
            DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
            DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "fast_hands");
        }
    }

    public void GetInventory()
    {
        List<string> keys = new List<string>(PlayerInventory.Keys);
        foreach (string key in keys)
        {
            PlayerInventory[key] = 0;
        }
        DB_Controller.GetComponent<DB_Controller>().GetUpgradeList(PhotonNetwork.NickName);
        
    }




    // PRE GAME MENU


    public void EnableHomeScreen()
    {
        if (!bg.animating) {
            LobbyScreen.SetActive(false);
            UpgradeScreen.SetActive(false);
            if (MapScreen.activeInHierarchy) {
                MapScreen.SetActive(false);
                bg.unZoom(PreGameHome);
            } else {
                PreGameHome.SetActive(true);
            }
        }

    }

    public void EnableUpgradeScreen()
    {
        if (!bg.animating)
        {
            PreGameHome.SetActive(false);
            LobbyScreen.SetActive(false);

            if (MapScreen.activeInHierarchy)
            {
                MapScreen.SetActive(false);

                bg.unZoom(UpgradeScreen);
            }
            else
            {
                UpgradeScreen.SetActive(true);
            }
            //ContentLobby.GetComponent<PopulateGridLobby>().OnRefresh();
        }

    }

    // zoom in (make sure to call unzoom if map is active when navigating away)
    public void EnableMapScreen()
    {
        LobbyScreen.SetActive(false);
        PreGameHome.SetActive(false);
        UpgradeScreen.SetActive(false);
        bg.zoom();
        GameObject.Find("MapIndicator").SetActive(false);
        

        // MapScreen.SetActive(true);
    }

    public void ToggleNotes() {
        Notes.SetActive(!Notes.activeInHierarchy);
    }


    public void EnableLobbyScreen()
    {
        if (!bg.animating) {
            PreGameHome.SetActive(false);
            UpgradeScreen.SetActive(false);

            if (MapScreen.activeInHierarchy) {
                MapScreen.SetActive(false);
                bg.unZoom(LobbyScreen);
            } else {
                LobbyScreen.SetActive(true);
            }
            ContentLobby.GetComponent<PopulateGridLobby>().OnRefresh();
        }
    }




    public void ThiefController()
    {
        if (PhotonNetwork.PlayerListOthers.Length == 0)
        {
            thief_2.SetActive(false);
            thief_3.SetActive(false);
            thief_4.SetActive(false);

        }
        if (PhotonNetwork.PlayerListOthers.Length == 1)
        {
            thief_2.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[0].NickName;
            thief_2.SetActive(true);
            thief_3.SetActive(false);
            thief_4.SetActive(false);

        }
        else if (PhotonNetwork.PlayerListOthers.Length == 2)
        {
            thief_2.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[0].NickName;
            thief_3.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[1].NickName;
            thief_2.SetActive(true);
            thief_3.SetActive(true);
            thief_4.SetActive(false);
        }
        else if (PhotonNetwork.PlayerListOthers.Length == 3)
        {
            thief_2.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[0].NickName;
            thief_3.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[1].NickName;
            thief_4.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[2].NickName;
            thief_2.SetActive(true);
            thief_3.SetActive(true);
            thief_4.SetActive(true);
        }
    }





        // HELPER FUNCTIONS
    public void ChangeUserNameInput()
    {
        Status.SetActive(false);
        StatusGuest.SetActive(false);
    }
    public void SetUserName()
    {
        ExistingUserMenu.SetActive(false);
        GuestMenu.SetActive(false);
        PhotonNetwork.NickName = UsernameLoginInput.text;
        //menu_script.SetActive(true);
        authValues = new AuthenticationValues();
        authValues.UserId = UsernameLoginInput.text;
        PhotonNetwork.AuthValues = authValues;
        Connect();
        thief_1.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;
        thief_1_home.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

        Debug.Log("NICKNAME: " + PhotonNetwork.NickName);

        if (IsGuest)
        {
            FriendsMenuButton.interactable = false;
            UpgradesMenuButton.interactable = false;
            BalanceInfoHome.SetActive(false);
            BalanceInfoPre.SetActive(false);

        }
        // ADD NEW UPGRADES HERE
        PlayerInventory.Add("speed_boots", 0);
        PlayerInventory.Add("shield", 0);
        PlayerInventory.Add("vision", 0);
        PlayerInventory.Add("self_revive", 0);
        PlayerInventory.Add("fast_hands", 0);
        HomeMenu.SetActive(true);
        ContentFriendsNew.GetComponent<PopulateGridFriends>().OnRefresh();


    }

    public void ReJoinAfterLeave()
    {
        //StartMenu.SetActive(false);
        RejoinWaitPanel.SetActive(true);
        DB_Controller.GetComponent<DB_Controller>().GetCoinBalance(PhotonNetwork.NickName);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        

        thief_1.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;
        thief_1_home.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

        if (IsGuest)
        {
            FriendsMenuButton.interactable = false;
            UpgradesMenuButton.interactable = false;
            BalanceInfoHome.SetActive(false);
            BalanceInfoPre.SetActive(false);

        }
        // ADD NEW UPGRADES HERE
        PlayerInventory.Add("speed_boots", 0);
        PlayerInventory.Add("shield", 0);
        PlayerInventory.Add("vision", 0);
        PlayerInventory.Add("self_revive", 0);
        PlayerInventory.Add("fast_hands", 0);
        HomeMenu.SetActive(true);
    }



    public void BackToUserManager()
    {
        ExistingUserMenu.SetActive(false);
        NewUserMenu.SetActive(false);
        GuestMenu.SetActive(false);
        UserManagerMenu.SetActive(true);
    }


    public void JoinNewRooom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = (byte)4;
        roomOptions.PublishUserId = true;
        if (!customProperties.ContainsKey("num_ready"))
        {
            customProperties.Add("num_ready", 0);
        }
        roomOptions.CustomRoomProperties = customProperties;
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName, roomOptions, TypedLobby.Default);

    }
    
    
    // START MENU
    public void StartGame()
    {
        StartMenu.SetActive(false);
        UserManagerMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToHome()
    {
        CreditsMenu.SetActive(false);
        StartMenu.SetActive(true);
    }

    public void OpenCredits()
    {
        CreditsMenu.SetActive(true);
        StartMenu.SetActive(false);

    }



    #region Callbacks
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + cause.ToString() + " ServerAddress: " + PhotonNetwork.ServerAddress);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        //After we connected to Master server, join the Lobby
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed got called. This can happen if the room is not existing or full or closed.");
        joiningRoom = false;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        
    }
    public override void OnJoinedLobby()
    {
        GetFriends(PhotonNetwork.NickName, 1);
        StartCoroutine("UpdateFriendList");
        //ContentFriendsNew.GetComponent<PopulateGridFriends>().OnRefresh();


        //JoinNewRooom();
        PhotonNetwork.AutomaticallySyncScene = true;
        //RejoinWaitPanel.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log("PLAYER ENTERED ROOM");
        ThiefController();
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        ThiefController();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        NewLobbyMenu.SetActive(false);
        PreGameMenu.SetActive(true);
        RoomNameButton.GetComponentInChildren<Text>().text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        StopCoroutine("UpdateFriendList");
        ThiefController();
        //PhotonNetwork.LoadLevel("PreGameLobby");

    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
    
    }
    #endregion
}
