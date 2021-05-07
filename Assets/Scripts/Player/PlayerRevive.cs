using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerRevive : MonoBehaviour
{
    public LayerMask playerMask;

    public TextMeshProUGUI downText;
    public GameObject textObject;

    

    private GameObject playerReference;
    private ProgressBarController progressBar;
    public PlayerController playerController;
    public float holdTime = 3.0f;
    public float startTime = 0f;

    private GameObject inProgressRessPlayer;
    private bool inProgress = false;
    private bool selfInProgress = false;
    private bool disabledPlayersInRange = false;


    void Start()
    {
        textObject = GameObject.Find("DisplayMessagePlayer");
        progressBar = GameObject.FindObjectOfType<ProgressBarController>();;
        StartCoroutine("FindReviveWithDelay", 0.01f);
    }

    void Update() {
        SelfRevive();
    }

    IEnumerator FindReviveWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            checkForRevive();
        }
    }

    public void checkForRevive()
    {
        progressBar = GameObject.FindObjectOfType<ProgressBarController>();
        //check circle radius of player
        Collider[] playersInView = Physics.OverlapSphere(transform.position, 3.0f, playerMask);
        //if another player there, check if down

        for (int i = 0; i < playersInView.Length; i++)
        {
            GameObject playerInView = playersInView[i].gameObject;

            if (playerInView.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            {
                //if in range and player is disabled then display E above their head
                if (playerInView.GetComponent<PlayerMovement>().disabled && GetComponent<PhotonView>().IsMine)
                {
                    if (!GetComponent<PlayerMovement>().disabled)
                    {
                        disabledPlayersInRange = true;
                        Debug.Log("Disabled");
                        playerInView.GetComponent<PlayerRevive>().downText.text = "Hold E";


                        if (Input.GetKey(KeyCode.E) && !inProgress)
                        {
                            inProgressRessPlayer = playerInView;
                            inProgress = true;
                            startTime = Time.time;
                            progressBar.Show();
                        }
                        if (Input.GetKey(KeyCode.E) && inProgress)
                        {
                            if (playerInView.GetInstanceID() == inProgressRessPlayer.GetInstanceID())
                            {
                                progressBar.UpdateBar(Time.time - startTime, 0, holdTime);

                                if (startTime + holdTime <= Time.time)
                                {
                                    playerInView.GetComponent<PhotonView>().RPC("syncDisabled", RpcTarget.All, false);
                                    playerInView.GetComponent<PlayerRevive>().downText.text = "";

                                    progressBar.Hide();
                                    progressBar.ResetBar();
                                }
                            }

                        }

                        if (!Input.GetKey(KeyCode.E))
                        {
                            startTime = 0f;
                            inProgress = false;

                            Debug.Log("Progress bar -- " + progressBar.name);
                            progressBar.Hide();
                            progressBar.ResetBar();
                        }
                    } else playerInView.GetComponent<PlayerRevive>().downText.text = "";

                }


            }

        }

        if (GetComponent<PlayerMovement>().disabled)
        {
            bool upPlayerInRange = false;
            for (int i = 0; i < playersInView.Length; i++)
            {
                GameObject playerInView = playersInView[i].gameObject;
                if (playerInView.gameObject.GetInstanceID() != this.gameObject.GetInstanceID() && !playerInView.GetComponent<PlayerMovement>().disabled)
                {
                    upPlayerInRange = true;
                }

            }
            if (!upPlayerInRange)
            {
                downText.text = "";
            }
        }
        else downText.text = "";
    }

    public void SelfRevive() {
        if (GetComponent<PhotonView>().IsMine) {
            playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
            if (playerController.self_revive && playerController.isDisabled) {
                textObject.GetComponent<Text>().text = "Hold E to use self revive";

                if (Input.GetKey(KeyCode.E) && !selfInProgress)
                {
                    selfInProgress = true;
                    startTime = Time.time;
                    progressBar.Show();
                }

                if (Input.GetKey(KeyCode.E) && selfInProgress)
                {
                    progressBar.UpdateBar(Time.time - startTime, 0, holdTime);

                    if (startTime + holdTime <= Time.time)
                    {
                        GetComponent<PhotonView>().RPC("syncDisabled", RpcTarget.All, false);
                        textObject.GetComponent<Text>().text = "";

                        playerController.self_revive = false;

                        progressBar.Hide();
                        progressBar.ResetBar();
                    }
                }

                if (!Input.GetKey(KeyCode.E))
                {
                    startTime = 0f;
                    selfInProgress = false;

                    progressBar.Hide();
                    progressBar.ResetBar();
                }

            } else {
                if (textObject.GetComponent<Text>().text == "Hold E to use self revive") {
                    //dear isaac. This line of code nearly made me cry. THanks, josh
                    textObject.GetComponent<Text>().text = "";
                }
            }
        }
    }
}
