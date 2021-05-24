using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

// movement script for swat guards
public class SwatMovement : MonoBehaviour
{   

    public NavMeshAgent agent;

    private PlayerMovement[] playerList;

    private PlayerMovement target;
    private int counter;
    // Start is called before the first frame update
    void Start()
    {
        playerList = GameObject.FindObjectsOfType<PlayerMovement>();
        target = playerList[0];
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!target.disabled)
        {
            agent.SetDestination(target.transform.position);
        }

        if(Vector3.Distance(target.transform.position, this.transform.position) <= 3f && !target.disabled)
        {
            target.disabled = true;
            target.gameObject.GetComponent<PhotonView>().RPC("syncDisabled", RpcTarget.All, true);
        }

        if (target.disabled)
        {
            foreach(PlayerMovement p in playerList)
            {
                if (!p.disabled)
                {
                    target = p;
                }
            }
            
        }
    }
}
