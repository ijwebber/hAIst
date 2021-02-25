using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class EndScreenController : MonoBehaviourPunCallbacks
{

    public GameObject[] playerRows;

    private void Awake()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
            return;
        }

        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() {{"win", false}});
        }

        foreach (GameObject row in playerRows) {
            row.SetActive(false);
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            GameObject row = playerRows[i];
            row.SetActive(true);

            

            Player player = PhotonNetwork.PlayerList[i];
            row.transform.Find("Player").gameObject.GetComponent<Text>().text = player.NickName;;

            row.transform.Find("Value").gameObject.GetComponent<Text>().text = "$" + ((int) player.CustomProperties["score"]).ToString();

            row.transform.Find("Items").gameObject.GetComponent<Text>().text = ((int) player.CustomProperties["itemsStolen"]).ToString();
            row.transform.Find("Special").gameObject.GetComponent<Text>().text = ((int) player.CustomProperties["specialStolen"]).ToString();
        }



    }

    public void QuitButton() {
        OnLeftRoom();
    }

    public void PlayAgainButton() {
        PhotonNetwork.LoadLevel("PreGameLobby");
        
    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
    }
}
