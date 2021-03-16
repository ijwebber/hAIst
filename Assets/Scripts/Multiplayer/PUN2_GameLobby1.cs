﻿using System.Collections;
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
    bool joiningRoom = false;
    public GameObject DB_Controller;
    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    // MENUS
    [SerializeField] private GameObject GuestMenu;
    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject UserManagerMenu;
    [SerializeField] private GameObject ExistingUserMenu;
    [SerializeField] private GameObject NewUserMenu;
    [SerializeField] private GameObject HomeMenu;


    [SerializeField] private GameObject LobbyMenu;

    // SCRIPTS
    [SerializeField] private GameObject menu_script;
    [SerializeField] private GameObject LobbyScript;

    

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
    public GameObject thief_2;
    public GameObject thief_3;
    public GameObject thief_4;
    public GameObject FriendsMenu;
    public string[] FriendList;

    // PHOTON NETWORK GAMEOBJECTS 

    // Use this for initialization
    void Start()
    {
        
        StartCoroutine(UpdateFriendList());
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
    }

    public void EnableLobbyMenu()
    {
        FriendsMenu.SetActive(false);
    }

    IEnumerator UpdateFriendList()
    {
        for (; ; )
        {
            if (PhotonNetwork.IsConnected & FriendList.Length != 0)
            {
            PhotonNetwork.FindFriends(FriendList);
            }
            yield return new WaitForSeconds(1f);

        }
    }

    public void GetFriends()
    {
        DB_Controller.GetComponent<DB_Controller>().GetFriends(UsernameLoginInput.text);
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
        HomeMenu.SetActive(true);
        thief_1.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;
        
        //LobbyScript.SetActive(true);
        //LobbyMenu.SetActive(true);
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
        //JoinNewRooom();
        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        //PhotonNetwork.LoadLevel("PreGameLobby");

    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
    
    }
    #endregion
}
