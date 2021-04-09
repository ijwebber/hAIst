using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


using Photon.Pun;
using Photon.Realtime;
using TMPro;


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

    // MENUS
    [SerializeField] private GameObject GuestMenu;
    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject UserManagerMenu;
    [SerializeField] private GameObject ExistingUserMenu;
    [SerializeField] private GameObject NewUserMenu;
    [SerializeField] private GameObject HomeMenu;
    [SerializeField] private GameObject LobbyMenu;
    [SerializeField] private GameObject PreGameMenu;
    [SerializeField] private GameObject CreditsMenu;
    [SerializeField] private GameObject PreGameHome;
    [SerializeField] private GameObject MapScreen;

    // SCRIPTS
    [SerializeField] private GameObject menu_script;
    [SerializeField] private GameObject LobbyScript;
    [SerializeField] private GameObject ContentLobby;
    [SerializeField] private GameObject ContentFriends;



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
    public string[] FriendList;
    public GameObject AddFriendStatus;
    public TMP_InputField AddFriendInput;
    public bool IsGuest = false;
    public Button FriendsMenuButton;


    // PRE GAME OBJECTS
    public Button BalanceButtonPreGame;
    public GameObject RoomNameButton;
    public GameObject LobbyScreen;

    // PHOTON NETWORK GAMEOBJECTS 

    // MAP SCREEN OBJECTS
    [SerializeField] private GameObject Notes;


    // Use this for initialization
    void Start()
    {
        
    }

    void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {
            thief_1.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;
            thief_1_home.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;
            PreGameMenu.SetActive(true);
            ThiefController();
            PreGameScript.GetComponent<PreGame>().SetReadyChecks();

            DB_Controller.GetComponent<DB_Controller>().GetCoinBalance(PhotonNetwork.NickName);

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
        MicMenu.SetActive(false);
        MicCheck = false;
        Home_Home.SetActive(false);
        AddFriendStatus.SetActive(false);
        ContentFriends.GetComponent<PopulateGridFriends>().OnRefresh();
    }

    public void EnableMicThreshold() {
        MicMenu.SetActive(true);
        MicCheck = true;
        Home_Home.SetActive(false);
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
        MicMenu.SetActive(false);
        MicCheck = false;
        AddFriendStatus.SetActive(false);
        Home_Home.SetActive(true);
    }

    IEnumerator UpdateFriendList()
    {
        for (; ; )
        {
            if (PhotonNetwork.IsConnectedAndReady & FriendList != null)
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

    public void GetFriends()
    {
        DB_Controller.GetComponent<DB_Controller>().GetFriends(UsernameLoginInput.text);
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

    // PRE GAME MENU

    public void EnableHomeScreen()
    {
        if (!bg.animating) {
            LobbyScreen.SetActive(false);
            if (MapScreen.activeInHierarchy) {
                MapScreen.SetActive(false);
                bg.unZoom(PreGameHome);
            } else {
                PreGameHome.SetActive(true);
            }
        }

    }


    // zoom in (make sure to call unzoom if map is active when navigating away)
    public void EnableMapScreen()
    {
        bg.zoom();
        GameObject.Find("MapIndicator").SetActive(false);
        LobbyScreen.SetActive(false);
        PreGameHome.SetActive(false);
        // MapScreen.SetActive(true);
    }

    public void ToggleNotes() {
        Notes.SetActive(!Notes.activeInHierarchy);
    }


    public void EnableLobbyScreen()
    {
        if (!bg.animating) {
            PreGameHome.SetActive(false);
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

        HomeMenu.SetActive(true);
        if (IsGuest)
        {
            FriendsMenuButton.interactable = false;
        }
        //LobbyScript.SetActive(true);
        //LobbyMenu.SetActive(true);
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
        GetFriends();
        StartCoroutine("UpdateFriendList");

        //JoinNewRooom();
        PhotonNetwork.AutomaticallySyncScene = true;
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
