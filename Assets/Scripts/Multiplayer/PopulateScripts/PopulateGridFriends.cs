using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;
using System;


public class PopulateGridFriends : MonoBehaviourPunCallbacks
{
    List<FriendInfo> FriendList = new List<FriendInfo>();
    List<RoomInfo> createdRooms = new List<RoomInfo>();


    public ScrollView scrollView;
    public GameObject prefab; // This is our prefab object that will be exposed in the inspector

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            //Re-join Lobby to get the latest Room list
            //PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
        else
        {
            //We are not connected, estabilish a new connection
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void Update()
    {
    }


    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        Debug.Log("Updated friend list");
        base.OnFriendListUpdate(friendList);
        FriendList = friendList;
        //Refresh();
        DestroyChildren();
        Populate();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("We have received the Room list");
        //After this callback, update the room list
        createdRooms = roomList;
    }
    public void OnRefresh()
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

    public void ButtonClick(string i)
    {
        PhotonNetwork.JoinRoom(i);
    }

    public int GetRoomCount(string roomName)
    {
        for (int i = 0; i < createdRooms.Count; i++)
        {
            if (createdRooms[i].Name.Equals(roomName))
            {
                return createdRooms[i].PlayerCount;
            }
        }
        return -1;
    }

    public bool IsRoomOpen(string roomName)
    {
        for (int i = 0; i < createdRooms.Count; i++)
        {
            if (createdRooms[i].Name.Equals(roomName))
            {
                return createdRooms[i].IsOpen;
            }
        }
        return false;
    }

    void Populate()
    {
        GameObject newObj; // Create GameObject instance

        for (int i = 0; i < FriendList.Count; i++)
        {
            // Create new instances of our prefab until we've created as many as we specified
            newObj = (GameObject)Instantiate(prefab, transform);
            newObj.transform.Find("FriendName").GetComponent<Text>().text = FriendList[i].UserId;
            if (FriendList[i].IsOnline)
            {
                //Debug.Log(FriendList[i].UserId + " Is online.");
                newObj.transform.Find("Offline").gameObject.SetActive(false);
                if (FriendList[i].IsInRoom)
                {
                    int playerCount = GetRoomCount(FriendList[i].Room);
                    bool roomOpen = IsRoomOpen(FriendList[i].Room);
                    if (roomOpen)
                    {
                        newObj.transform.Find("RoomInfoScroll").GetComponent<Text>().text = playerCount + "/" + 4;
                        newObj.transform.Find("RoomInfoScroll").gameObject.SetActive(true);
                        newObj.transform.Find("JoinGameButton").gameObject.SetActive(true);
                        string roomName = FriendList[i].Room;
                        newObj.transform.Find("JoinGameButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { ButtonClick(roomName); });
                    }
                    else
                    {
                        newObj.transform.Find("RoomInfoScroll").gameObject.SetActive(false);
                        newObj.transform.Find("JoinGameButton").gameObject.SetActive(false);

                    }
                }


            }
            else
            {
                //Debug.Log(FriendList[i].UserId + "Is offline.");
            }
            //newObj.transform.Find("RoomInfoScroll").GetComponent<Text>().text = createdRooms[i].PlayerCount + "/" + createdRooms[i].MaxPlayers;
            //string roomName = createdRooms[i].Name;
            //newObj.transform.Find("JoinGameButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { ButtonClick(roomName); });

        }

    }

    void DestroyChildren()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}