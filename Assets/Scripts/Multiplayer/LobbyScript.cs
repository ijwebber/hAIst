using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class LobbyScript : MonoBehaviourPunCallbacks
{

    //The list of created rooms
    List<RoomInfo> createdRooms = new List<RoomInfo>();
    //Use this name when creating a Room
    string roomName = "Room 1";
    Vector2 roomListScroll = Vector2.zero;
    bool joiningRoom = false;
    public GUISkin myskin = null;

    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    public Text StatusText;
    public InputField RoomNameInput;
    public Button CreateRoomButton;
    public Button RefreshButton;
    public Text PlayerName;

    private string RoomName;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
            {
                //Re-join Lobby to get the latest Room list
                PhotonNetwork.JoinLobby(TypedLobby.Default);
            }
        else
            {
                //We are not connected, estabilish a new connection
                PhotonNetwork.ConnectUsingSettings();
            }
        CreateRoomButton.interactable = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        StatusText.text = "Status: " + PhotonNetwork.NetworkClientState;
        PlayerName.text = "" + PhotonNetwork.NickName;



        if (StatusText.text.Equals("Status: JoinedLobby"))
        {
            //CreateRoomButton.interactable = true;
            RefreshButton.interactable = true;  
        }
        else{
            CreateRoomButton.interactable = false;
            RefreshButton.interactable = false;    
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
        joiningRoom = false;
        if (PhotonNetwork.IsConnected)
            {
                //Re-join Lobby to get the latest Room list
                PhotonNetwork.JoinLobby(TypedLobby.Default);
            }
        else
            {
                //We are not connected, estabilish a new connection
                PhotonNetwork.ConnectUsingSettings();
            }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
        joiningRoom = false;
        if (PhotonNetwork.IsConnected)
            {
                //Re-join Lobby to get the latest Room list
                PhotonNetwork.JoinLobby(TypedLobby.Default);
            }
        else
            {
                //We are not connected, estabilish a new connection
                PhotonNetwork.ConnectUsingSettings();
            }
    }


    // check room name
    public void ChangeRoomNameInput()
    {
        if (RoomNameInput.text.Length >= 4)
        {
            CreateRoomButton.interactable=true;
            RoomName = RoomNameInput.text;
        }
        else {
            CreateRoomButton.interactable=false;
        }
    }

    //create room
    public void CreateRoom()
    {
        joiningRoom = true;
        Debug.Log("ROOM NAME: "+ RoomName);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = (byte)4; //Set any number
        roomOptions.PublishUserId = true;
        if (!customProperties.ContainsKey("num_ready"))
        {
            customProperties.Add("num_ready", 0);
        } 
        roomOptions.CustomRoomProperties = customProperties;
        PhotonNetwork.CreateRoom(RoomName, roomOptions, TypedLobby.Default);
    }

    //refresh room list
    public void Refresh()
    {
        if (PhotonNetwork.IsConnected)
            {
                //Re-join Lobby to get the latest Room list
                PhotonNetwork.JoinLobby(TypedLobby.Default);
            }
            else
            {
                //We are not connected, estabilish a new connection
                PhotonNetwork.ConnectUsingSettings();
            }
    }
}
