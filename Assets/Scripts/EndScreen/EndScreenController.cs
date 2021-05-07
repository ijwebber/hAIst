using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class EndScreenController : MonoBehaviourPunCallbacks
{

    // private DBControllerEnd dbController;
    public GameObject[] playerRows;
    public GameObject totalRow;

    public GameObject winScreen;
    public GameObject lossScreen;

    public Button PlayAgainWin;
    public Button PlayAgainLose;
    [SerializeField] DBControllerEnd dbController;

    [SerializeField] private EndScreenAudioController audioController;

    private void Awake()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            audioController.GetComponent<PhotonView>().RPC("StopAll", RpcTarget.All);
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
            return;
        }

        // dbController = GameObject.FindObjectOfType<DBControllerEnd>();
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
            Debug.Log("Is guest? " + PlayerPrefs.GetInt("isGuest", -1));
            if (PlayerPrefs.GetInt("isGuest", -1) == 0) {
                Debug.Log("New balance = " + PlayerPrefs.GetInt("PlayerBalance", 0) + ((int) PhotonNetwork.CurrentRoom.CustomProperties["score"]/PhotonNetwork.PlayerList.Length));
                dbController.EditCoinBalance(PhotonNetwork.NickName, (PlayerPrefs.GetInt("PlayerBalance", 0) + ((int) PhotonNetwork.CurrentRoom.CustomProperties["score"]/PhotonNetwork.PlayerList.Length)),0);
            }
        } else {
            winScreen.SetActive(false);
            lossScreen.SetActive(true);
        }

    }

    public void QuitButton()
    {
        PhotonNetwork.LeaveRoom();
    }

    //Go back to main meny when you leave game
    public override void OnLeftRoom()
    {
        Debug.Log("On left Room callback");

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        audioController.GetComponent<PhotonView>().RPC("StopAll", RpcTarget.All);
        PhotonNetwork.LoadLevel("GameLobby 1");
        Debug.Log("gamelobby 1 loaded");
    }

    public void PlayAgainButton() {
        audioController.GetComponent<PhotonView>().RPC("StopAll", RpcTarget.All);
        PhotonNetwork.LoadLevel("GameLobby 1");

    }

}
