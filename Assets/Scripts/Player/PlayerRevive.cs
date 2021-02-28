using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerRevive : MonoBehaviour
{
    public LayerMask playerMask;

    public TextMeshProUGUI downText;

    

    private GameObject playerReference;
    public ProgressBarController progressBar;
    public float holdTime = 3.0f;
    public float startTime = 0f;

    private GameObject inProgressRessPlayer;
    private bool inProgress = false;
    private bool disabledPlayersInRange = false;


    void Start()
    {
        StartCoroutine("FindReviveWithDelay", 0.2f);
    }
    
    void Update() {
        if (Input.GetKey(KeyCode.E) && inProgress) {
            progressBar.Show();
            progressBar.UpdateBar(Time.time - startTime, 0, holdTime);
        } else if (!Input.GetKey(KeyCode.E)) {
            progressBar.Hide();
            progressBar.ResetBar();
        }
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
        //check circle radius of player
        Collider[] playersInView = Physics.OverlapSphere(transform.position, 2.0f, playerMask);
        //if another player there, check if down

        for (int i = 0; i < playersInView.Length; i++)
        {
            GameObject playerInView = playersInView[i].gameObject;

            if (playerInView.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            {
                //if in range and player is disabled then display E above their head
                if (playerInView.GetComponent<PlayerMovement>().disabled && GetComponent<PhotonView>().IsMine)
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

                        progressBar.Hide();
                        progressBar.ResetBar();
                    }

                }


            }

        }

        if(GetComponent<PlayerMovement>().disabled && playersInView.Length == 1)
        {
            downText.text = "";
        }

        
    }

    

}
