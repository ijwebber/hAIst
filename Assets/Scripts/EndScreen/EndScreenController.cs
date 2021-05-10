using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class EndScreenController : MonoBehaviourPunCallbacks
{

    // private DBControllerEnd dbController;
    public GameObject[] playerRows;
    public GameObject[] playerSegs;
    [SerializeField] private TextMeshProUGUI deadDesc;
    [SerializeField] private TextMeshProUGUI GADesc;
    [SerializeField] private TextMeshProUGUI moneyDesc;
    [SerializeField] private TextMeshProUGUI loudDesc;
    public TextMeshProUGUI totalText;
    public GameObject winScreen;
    public GameObject lossScreen;

    public Button PlayAgainWin;
    public Button PlayAgainLose;
    [SerializeField] DBControllerEnd dbController;

    private void Awake()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {

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

            for (int i = 0; i < playerRows.Length; i++) {
                GameObject row = playerRows[i];
                GameObject seg = playerSegs[i];
                row.SetActive(false);
                seg.SetActive(false);
            }

            int noPlayers = PhotonNetwork.PlayerList.Length;
            (List<int>,int) moneyBags = (new List<int>(),0);
            (List<int>,int) GA = (new List<int>(),0);
            (List<int>,int) loud = (new List<int>(),0);
            (List<int>,int) dead = (new List<int>(),0);

            //populate rows and segments
            for (int i = 0; i < noPlayers; i++)
            {
                GameObject row = playerRows[i];
                GameObject seg = playerSegs[i];
                row.SetActive(true);
                seg.GetComponent<Image>().fillAmount = 1/noPlayers;
                seg.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,(360/noPlayers)*i);

                Player player = PhotonNetwork.PlayerList[i];
                row.transform.Find("Player").gameObject.GetComponent<Text>().text = player.NickName;
                row.transform.Find("Cut").gameObject.GetComponent<Text>().text = ((int)100/noPlayers).ToString() + "%";
                row.transform.Find("Earnings").gameObject.GetComponent<Text>().text = "$" + (Mathf.Floor((int) (PhotonNetwork.CurrentRoom.CustomProperties["score"])/noPlayers)).ToString();
                if ((int)player.CustomProperties["score"] >= moneyBags.Item2)  {
                    if ((int)player.CustomProperties["score"] == moneyBags.Item2) {
                        moneyBags.Item1.Add(i);
                    } else {
                        moneyBags = (new List<int>(i), (int)player.CustomProperties["score"]);
                    }
                    moneyDesc.text = "Got the biggest haul for the gang ($" + ((int)player.CustomProperties["score"]).ToString() + ")";
                }
                if (player.CustomProperties["downs"] != null) {
                    if ((int)player.CustomProperties["downs"] >= dead.Item2)  {
                        if ((int)player.CustomProperties["downs"] == dead.Item2) {
                            dead.Item1.Add(i);
                        } else {
                            dead = (new List<int>(i), (int)player.CustomProperties["downs"]);
                        }
                        deadDesc.text = "Letting down the side with Most downs (" + ((int)player.CustomProperties["downs"]).ToString() + ")";
                    }
                }
                if (player.CustomProperties["revives"] != null)  {
                    if ((int)player.CustomProperties["revives"] >= GA.Item2)  {
                        if ((int)player.CustomProperties["revives"] == GA.Item2) {
                            GA.Item1.Add(i);
                        } else {
                            GA = (new List<int>(i), (int)player.CustomProperties["revives"]);
                        }
                        GADesc.text = "Had everyone's backs with most saves (" + ((int)player.CustomProperties["revives"]).ToString() + ")";
                    }
                }
                if (player.CustomProperties["alerts"] != null)  {
                    if ((int)player.CustomProperties["alerts"] >= loud.Item2)  {
                        if ((int)player.CustomProperties["alerts"] == loud.Item2) {
                            loud.Item1.Add(i);
                        } else {
                            loud = (new List<int>(i), (int)player.CustomProperties["alerts"]);
                        }
                        loudDesc.text = "Couldn't shut up and alerted the most guards (" + ((int)player.CustomProperties["alerts"]).ToString() + ")" ;
                    }
                }
            }
            Debug.Log("END money " + PhotonNetwork.PlayerList[moneyBags.Item1[0]] + " // " + moneyBags.Item2);
            Debug.Log("END loud " + PhotonNetwork.PlayerList[loud.Item1[0]] + " // " + loud.Item2);
            Debug.Log("END ga " + PhotonNetwork.PlayerList[GA.Item1[0]] + " // " + GA.Item2);
            Debug.Log("END downs " + PhotonNetwork.PlayerList[dead.Item1[0]] + " // " + dead.Item2);
            
            //update total score
            totalText.text = ((int)PhotonNetwork.CurrentRoom.CustomProperties["score"]).ToString();

            //update database
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
        PhotonNetwork.LoadLevel("GameLobby 1");
        Debug.Log("gamelobby 1 loaded");
    }

    public void PlayAgainButton() {
        PhotonNetwork.LoadLevel("GameLobby 1");

    }

}
