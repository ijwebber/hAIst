using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerRevive : MonoBehaviour
{
    public LayerMask playerMask;
    public GameObject floatingTextPrefab;

    [HideInInspector]
    public List<KeyValuePair<GameObject, GameObject>> disabledInRangePlayers = new List<KeyValuePair<GameObject, GameObject>>();

    private GameObject playerReference;
    public float holdTime = 3.0f;
    public float startTime = 0f;

    private KeyValuePair<GameObject, GameObject> inProgressRess;
    private bool inProgress = false;


    private void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
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

                    GameObject tt = Instantiate(floatingTextPrefab, playerInView.transform.position + new Vector3(0, -2f, 0), Quaternion.identity, playerInView.transform);
                    tt.GetComponent<TextMesh>().text = "Hold E";
                    Destroy(tt, 0.2f);

                    if (Input.GetKey(KeyCode.E) && !inProgress)
                    {
                        inProgressRess = new KeyValuePair<GameObject, GameObject>(playerInView, tt);
                        inProgress = true;
                        startTime = Time.time;
                    }
                    if (Input.GetKey(KeyCode.E) && inProgress)
                    {   
                        if(playerInView.GetInstanceID() == inProgressRess.Key.GetInstanceID())
                        {
                            if(startTime + holdTime <= Time.time)
                            {
                                playerInView.GetComponent<PhotonView>().RPC("syncDisabled", RpcTarget.All, false);
                            }
                        }
                        
                    }

                    if (!Input.GetKey(KeyCode.E))
                    {
                        startTime = 0f;
                        inProgress = false;
                        
                    }

                }

                
            }

        }
        //if down then show "hold E"
        //check if E is held for 3 seconds
        //change down value to false in player being revived
        
    }

    

}
