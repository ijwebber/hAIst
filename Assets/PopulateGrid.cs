using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;
using System;


public class PopulateGrid : MonoBehaviourPunCallbacks
{
    List<RoomInfo> createdRooms = new List<RoomInfo>();
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
	}

    void Update()
    {
    }

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("We have received the Room list");
        //After this callback, update the room list
        createdRooms = roomList;
        DestroyChildren();
        Populate();

    }

    public void ButtonClick(string i)
    {
        PhotonNetwork.JoinRoom(i);
    }

	void Populate()
	{
		GameObject newObj; // Create GameObject instance

		for (int i = 0; i < createdRooms.Count; i++)
		{
			 // Create new instances of our prefab until we've created as many as we specified
			newObj = (GameObject)Instantiate(prefab, transform);
            newObj.transform.Find("RoomNameScroll").GetComponent<Text>().text = "  " + createdRooms[i].Name;
            newObj.transform.Find("RoomInfoScroll").GetComponent<Text>().text = createdRooms[i].PlayerCount + "/" + createdRooms[i].MaxPlayers;
            string roomName = createdRooms[i].Name;
            newObj.transform.Find("JoinGameButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {ButtonClick(roomName); });
			
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