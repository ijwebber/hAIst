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
    [SerializeField] private string[] colors;
    [SerializeField] private GameObject deadWeight;
    [SerializeField] private GameObject GA;
    [SerializeField] private GameObject moneyBags;
    [SerializeField] private GameObject loudMouth;
    public TextMeshProUGUI totalText;
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
            audioController.PlayWin();

            winScreen.SetActive(true);
            lossScreen.SetActive(false);

            for (int i = 0; i < playerRows.Length; i++) {
                GameObject row = playerRows[i];
                GameObject seg = playerSegs[i];
                row.SetActive(false);
                seg.SetActive(false);
            }

            int noPlayers = PhotonNetwork.PlayerList.Length;
            List<int> moneyPlayers = new List<int>();
            int moneyValue = 0;
            List<int> GAPlayers = new List<int>();
            int GAValue = 0;
            List<int> loudPlayers = new List<int>();
            int loudValue = 0;
            List<int> deadPlayers = new List<int>();
            int deadValue = 0;

            //populate rows and segments
            for (int i = 0; i < noPlayers; i++)
            {
                GameObject row = playerRows[i];
                row.SetActive(true);

                Player player = PhotonNetwork.PlayerList[i];
                row.transform.Find("Player").gameObject.GetComponent<Text>().text = player.NickName;
                int specs = 0;
                if (PhotonNetwork.PlayerList[i].CustomProperties["specialStolen"] != null) {
                    specs = (int)PhotonNetwork.PlayerList[i].CustomProperties["specialStolen"];
                }
                row.transform.Find("Specials").gameObject.GetComponent<Text>().text = specs.ToString();
                if ((int)player.CustomProperties["score"] >= moneyValue)  {
                    moneyPlayers.Add(i);
                    moneyValue = (int)player.CustomProperties["score"];
                    moneyBags.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "Got the biggest haul for the gang ($" + ((int)player.CustomProperties["score"]).ToString() + ")";
                } else {
                    Debug.Log("Null score");
                }
                if (player.CustomProperties["downs"] != null) {
                    if ((int)player.CustomProperties["downs"] >= deadValue)  {
                        deadPlayers.Add(i);
                        deadValue =  (int)player.CustomProperties["downs"];
                        Debug.Log("Player " + i + " got downed " + (int)player.CustomProperties["downs"]);
                        deadWeight.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "let down the side with Most downs (" + ((int)player.CustomProperties["downs"]).ToString() + ")";
                    }
                } else {
                    Debug.Log("Null downs");
                }
                if (player.CustomProperties["revives"] != null)  {
                    if ((int)player.CustomProperties["revives"] >= GAValue)  {
                        GAPlayers.Add(i);
                        GAValue = (int)player.CustomProperties["revives"];
                        GA.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "Had everyone's backs with most saves (" + ((int)player.CustomProperties["revives"]).ToString() + ")";
                    }
                }
                if (player.CustomProperties["alerts"] != null)  {
                    if ((int)player.CustomProperties["alerts"] >= loudValue)  {
                        loudPlayers.Add(i);
                        loudValue = (int)player.CustomProperties["alerts"];
                        loudMouth.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "Couldn't shut up and alerted the most guards (" + ((int)player.CustomProperties["alerts"]).ToString() + ")" ;
                    }
                }
            }
            int finalEarnings = 0;
            float startingCut = (1f/(float)noPlayers);
            float[] cuts = {startingCut, startingCut, startingCut, startingCut};
            for (int i = 0; i < noPlayers; i++)
            {
                float cut = startingCut;
                if (moneyPlayers.Count < noPlayers) {
                    if (moneyPlayers.Contains(i)) {
                        cuts[i] += .1f/moneyPlayers.Count;
                        if (moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().text == "Player1") {
                            moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "<#" + colors[i] + ">" + PhotonNetwork.PlayerList[i].NickName + "</color>";
                        } else {
                            moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().text += " + " + "<#" + colors[i] + ">" + PhotonNetwork.PlayerList[i].NickName + "</color>";
                        }
                    } else {
                        if (moneyPlayers.Count > 0) {
                            cuts[i] -= .1f/(noPlayers-moneyPlayers.Count);
                        }
                    }
                }
                if (loudPlayers.Count < noPlayers) {
                    if (loudPlayers.Contains(i)) {
                        cuts[i] -= .05f/loudPlayers.Count;
                        if (loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().text == "Player1") {
                            loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "<#" + colors[i] + ">" + PhotonNetwork.PlayerList[i].NickName + "</color>";
                        } else {
                            loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().text += " + " + "<#" + colors[i] + ">" + PhotonNetwork.PlayerList[i].NickName + "</color>";
                        }
                    } else {
                        if (loudPlayers.Count > 0) {
                            cuts[i] += .05f/(noPlayers-loudPlayers.Count);
                        }
                    }
                }
                if (GAPlayers.Count < noPlayers) {
                    if (GAPlayers.Contains(i)) {
                        cuts[i] += .05f/GAPlayers.Count;
                        if (GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().text == "Player1") {
                            GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "<#" + colors[i] + ">" + PhotonNetwork.PlayerList[i].NickName + "</color>";
                        } else {
                            GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().text += " + " + "<#" + colors[i] + ">" + PhotonNetwork.PlayerList[i].NickName + "</color>";
                        }
                    } else {
                        if (GAPlayers.Count > 0) {
                            cuts[i] -= .05f/(noPlayers-GAPlayers.Count);
                        }

                    }
                }
                if (deadPlayers.Count < noPlayers) {
                    if (deadPlayers.Contains(i)) {
                        cuts[i] -= .1f/deadPlayers.Count;
                        if (deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().text == "Player1") {
                            deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "<#" + colors[i] + ">" + PhotonNetwork.PlayerList[i].NickName + "</color>";
                        } else {
                            deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().text += " + " + "<#" + colors[i] + ">" + PhotonNetwork.PlayerList[i].NickName + "</color>";
                        }
                    } else {
                        if (deadPlayers.Count > 0) {
                            cuts[i] += .1f/(noPlayers-deadPlayers.Count);
                        }
                    }
                }
            }
            if (moneyPlayers.Count == 0) {
                moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "No-one";
                moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.gray;
                moneyBags.transform.Find("ExtraPercent").gameObject.SetActive(false);

            } else if (moneyPlayers.Count == noPlayers) {
                moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "Everyone!";
                moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.white;
                moneyBags.transform.Find("ExtraPercent").gameObject.SetActive(false); // maybe give a flat bonus
            }
            if (loudPlayers.Count == 0) {
                loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "No-one";
                loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.gray;
                loudMouth.transform.Find("ExtraPercent").gameObject.SetActive(false);
            } else if (loudPlayers.Count == noPlayers) {
                loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "Everyone!";
                loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.white;
                loudMouth.transform.Find("ExtraPercent").gameObject.SetActive(false); // maybe give a flat bonus
            }
            if (GAPlayers.Count == 0) {
                GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "No-one";
                GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.gray;
                GA.transform.Find("ExtraPercent").gameObject.SetActive(false);
            } else if (GAPlayers.Count == noPlayers) {
                GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "Everyone!";
                GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.white;
                GA.transform.Find("ExtraPercent").gameObject.SetActive(false); // maybe give a flat bonus
            }
            if (deadPlayers.Count == 0) {
                deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "No-one";
                deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.gray;
                deadWeight.transform.Find("ExtraPercent").gameObject.SetActive(false);
            } else if (deadPlayers.Count == noPlayers) {
                deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "Everyone!";
                deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.white;
                deadWeight.transform.Find("ExtraPercent").gameObject.SetActive(false); // maybe give a flat bonus
            }
            float startRot = 0;
            for (int i = 0; i < noPlayers; i++)
            {
                GameObject row = playerRows[i];
                GameObject seg = playerSegs[i];
                seg.SetActive(true);
                seg.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,startRot);
                seg.GetComponent<Image>().fillAmount = cuts[i];
                row.transform.Find("Cut").gameObject.GetComponent<Text>().text = (cuts[i]*100).ToString("0.#") + "%";
                if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[i]) {
                    finalEarnings = (int)Mathf.Floor((int) (PhotonNetwork.CurrentRoom.CustomProperties["score"]) * cuts[i]);
                }
                row.transform.Find("Earnings").gameObject.GetComponent<Text>().text = "$" + (Mathf.Floor((int) (PhotonNetwork.CurrentRoom.CustomProperties["score"]) * cuts[i])).ToString();
                startRot -= 360f*cuts[i];
                Debug.Log("END // " + startRot);
            }
            
            //update total score
            totalText.text = "$" + ((int)PhotonNetwork.CurrentRoom.CustomProperties["score"]).ToString();

            //update database
            if (PlayerPrefs.GetInt("isGuest", -1) == 0) {
                Debug.Log("New balance = " + PlayerPrefs.GetInt("PlayerBalance", 0) + (finalEarnings));
                dbController.EditCoinBalance(PhotonNetwork.NickName, (PlayerPrefs.GetInt("PlayerBalance", 0) + finalEarnings),0);
            }
        } else {
            audioController.PlayLoss();

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
