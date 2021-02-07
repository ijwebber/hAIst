﻿using System.Collections;
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


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("We have received the Room list");
        //After this callback, update the room list
        createdRooms = roomList;
    }

    void OnGUI()
    {
        GUI.skin = myskin;
        GUI.Window(0, new Rect(Screen.width / 2 - 450, Screen.height / 2 - 200, 900, 400), LobbyWindow, "Lobby");
    }

    void LobbyWindow(int index)
    {
        //Connection Status and Room creation Button
        GUILayout.BeginHorizontal();

        GUILayout.Label("Status: " + PhotonNetwork.NetworkClientState);

        if (joiningRoom || !PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)
        {
            GUI.enabled = false;
        }

        GUILayout.FlexibleSpace();

        //Room name text field
        roomName = GUILayout.TextField(roomName, GUILayout.Width(250));

        if (GUILayout.Button("Create Room", GUILayout.Width(125)))
        {
            if (roomName != "")
            {
                joiningRoom = true;

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.IsOpen = true;
                roomOptions.IsVisible = true;
                roomOptions.MaxPlayers = (byte)4; //Set any number

                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
            }
        }

        GUILayout.EndHorizontal();

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
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(createdRooms[i].Name, GUILayout.Width(400));
                GUILayout.Label(createdRooms[i].PlayerCount + "/" + createdRooms[i].MaxPlayers);

                GUILayout.FlexibleSpace();

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

        //Set player name and Refresh Room button
        GUILayout.BeginHorizontal();

        GUILayout.Label("Player Name: " + PhotonNetwork.NickName, GUILayout.Width(85));
        //Player name text field
        //playerName = GUILayout.TextField(PhotonNetwork.NickName, GUILayout.Width(250));

        GUILayout.FlexibleSpace();

        GUI.enabled = (PhotonNetwork.NetworkClientState == ClientState.JoinedLobby || PhotonNetwork.NetworkClientState == ClientState.Disconnected) && !joiningRoom;
        if (GUILayout.Button("Refresh", GUILayout.Width(100)))
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

        GUILayout.EndHorizontal();

        if (joiningRoom)
        {
            GUI.enabled = true;
            GUI.Label(new Rect(900 / 2 - 50, 400 / 2 - 10, 100, 20), "Connecting...");
        }
    }

}
