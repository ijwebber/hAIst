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
    public GameObject totalRow;

    public GameObject winScreen;
    public GameObject lossScreen;

    public Button PlayAgainWin;
    public Button PlayAgainLose;

    private void Awake()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
            return;
        }

        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() {{"end", false}, {"special", 0}});
            PlayAgainWin.interactable = true;
            PlayAgainLose.interactable = true;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {{"ready", false}});

        bool wasWin = (bool) PhotonNetwork.CurrentRoom.CustomProperties["win"];

        if (wasWin) {
            winScreen.SetActive(true);
            lossScreen.SetActive(false);

            foreach (GameObject row in playerRows) {
                row.SetActive(false);
            }

            int totalItems = 0;
            int totalSpecial = 0;

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                GameObject row = playerRows[i];
                row.SetActive(true);

                Player player = PhotonNetwork.PlayerList[i];
                row.transform.Find("Player").gameObject.GetComponent<Text>().text = player.NickName;;
                row.transform.Find("Value").gameObject.GetComponent<Text>().text = "$" + ((int) player.CustomProperties["score"]).ToString();

                int numItems = ((int) player.CustomProperties["itemsStolen"]);
                totalItems += numItems;
                row.transform.Find("Items").gameObject.GetComponent<Text>().text = numItems.ToString();
                int numSpecial = ((int) player.CustomProperties["specialStolen"]);
                totalSpecial += numSpecial;
                row.transform.Find("Special").gameObject.GetComponent<Text>().text = numSpecial.ToString();
            }
            
            totalRow.transform.Find("Items").gameObject.GetComponent<Text>().text = totalItems.ToString();
            totalRow.transform.Find("Special").gameObject.GetComponent<Text>().text = totalSpecial.ToString();
            totalRow.transform.Find("Value").gameObject.GetComponent<Text>().text = "$" + ((int) PhotonNetwork.CurrentRoom.CustomProperties["score"]).ToString();
        } else {
            winScreen.SetActive(false);
            lossScreen.SetActive(true);
        }

    }

    public void QuitButton() {
        OnLeftRoom();
    }

    public void PlayAgainButton() {
        PhotonNetwork.LoadLevel("GameLobby 1");
    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
    }
}
