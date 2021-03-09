﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class PUN2_GameLobby1 : MonoBehaviourPunCallbacks
{

    string gameVersion = "0.9";
    //The list of created rooms
    List<RoomInfo> createdRooms = new List<RoomInfo>();
    bool joiningRoom = false;


    [SerializeField] private GameObject UsernameMenu;
    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject LobbyMenu;


    [SerializeField] private GameObject menu_script;
    [SerializeField] private GameObject LobbyScript;

    [SerializeField] TMP_InputField UsernameInput;
    [SerializeField] private GameObject StartButton;
    public string GetUsersURL = "http://brasspig.unaux.com/get_users.php";
    private AuthenticationValues authValues;



    // Use this for initialization
    void Start()
    {
        StartCoroutine(GetRequest(GetUsersURL));
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



    public void ChangeUserNameInput()
    {
        if (UsernameInput.text.Length >= 4)
        {
            StartButton.SetActive(true);
        }
    }

    public void SetUserName()
    {
        UsernameMenu.SetActive(false);
        PhotonNetwork.NickName = UsernameInput.text;
        //menu_script.SetActive(true);
        authValues = new AuthenticationValues();
        authValues.UserId = UsernameInput.text;
        PhotonNetwork.AuthValues = authValues;
        Connect(); 
        LobbyScript.SetActive(true);
        LobbyMenu.SetActive(true);
        
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        StartMenu.SetActive(false);
        UsernameMenu.SetActive(true);
    }

    IEnumerator GetRequest(string uri) {
 
        using(UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
        
            if (webRequest.isNetworkError) {
                Debug.Log(webRequest.error);
                //or example
            } else {
                Debug.Log(webRequest.downloadHandler.text);

            }
        }
    }


    

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed got called. This can happen if the room is not existing or full or closed.");
        joiningRoom = false;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        //Set our player name
        //PhotonNetwork.NickName = playerName;
        //Load the Scene called GameLevel (Make sure it's added to build settings)
        //PhotonNetwork.LoadLevel("PreGameLobby");
    }
    public void OnJoinedLobby()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        PhotonNetwork.LoadLevel("PreGameLobby");

    }
}
