using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class menu_script : MonoBehaviourPunCallbacks
{



    //The list of created rooms
    List<RoomInfo> createdRooms = new List<RoomInfo>();
    //Use this name when creating a Room
    string roomName = "Room 1";
    Vector2 roomListScroll = Vector2.zero;
    bool joiningRoom = false;
    public GUISkin myskin = null;

    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    public void Start()
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

   

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("We have received the Room list");
        //After this callback, update the room list
        createdRooms = roomList;
    }

    void OnGUI()
    {
        GUI.skin = myskin;
        GUI.Window(0, new Rect(Screen.width / 2- 500, Screen.height / 2 - 200, 900, 400), LobbyWindow, "");
    }

    void LobbyWindow(int index)
    {

        //Scroll through available rooms
        roomListScroll = GUILayout.BeginScrollView(roomListScroll, true, true);

        if (createdRooms.Count == 0)
        {
            GUILayout.Label("No Rooms Are Availble. Please Create Your Own.");
        }
        else
        {
            for (int i = 0; i < createdRooms.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(createdRooms[i].Name, GUILayout.Width(350));
                GUILayout.Label(createdRooms[i].PlayerCount + "/" + createdRooms[i].MaxPlayers, GUILayout.Width(750));

                //GUILayout.FlexibleSpace();

                if (GUILayout.Button("Join Room"))
                {
                    joiningRoom = true;



                    //Join the Room
                    PhotonNetwork.JoinRoom(createdRooms[i].Name);
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();

        
    }

}
