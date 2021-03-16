using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;
using System;


public class PopulateGridLobby : MonoBehaviourPunCallbacks
{
    
    public ScrollView scrollView;
    public GameObject prefab; // This is our prefab object that will be exposed in the inspector

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
        OnRefresh();
    }

    void Update()
    {
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DestroyChildren();
        Populate();
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        DestroyChildren();
        Populate();
    }

    public void OnRefresh()
    {
        DestroyChildren();
        Populate();
    }



    public void ButtonClick(string i)
    {
        PhotonNetwork.JoinRoom(i);
    }

    

    void Populate()
    {
        Debug.Log("LOBBY POPULATE");
        if (PhotonNetwork.PlayerList.Length > 0)
        {
            GameObject newObj; // Create GameObject instance
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                // Create new instances of our prefab until we've created as many as we specified
                newObj = (GameObject)Instantiate(prefab, transform);
                newObj.transform.Find("FriendName").GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
                
                //newObj.transform.Find("RoomInfoScroll").GetComponent<Text>().text = createdRooms[i].PlayerCount + "/" + createdRooms[i].MaxPlayers;
                //string roomName = createdRooms[i].Name;
                //newObj.transform.Find("JoinGameButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { ButtonClick(roomName); });

            }
        }
        else
        {
            GameObject newObj;
            newObj = (GameObject)Instantiate(prefab, transform);
            newObj.transform.Find("FriendName").GetComponent<Text>().text = "Lobby is empty.";
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