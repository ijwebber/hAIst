using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class GuardMovement : MonoBehaviour
{
    

    public NavMeshAgent agent;


    public List<Vector3> patrolPath = new List<Vector3> {new Vector3(-44.0f, 13.38f, 27.83f), new Vector3(-8.0f, 13.38f, 27.7f), new Vector3(-6.2f, 13.38f, 4.3f), new Vector3(-32.4f, 13.21f, 13.0f)};
    private int currDes = 0;
    private bool start = true;
    

    private void Awake()
    {
        
    }
    void Update()
    {

        FieldOfView fovScript = GetComponent<FieldOfView>();

        //if a target is in fov then path to that target
        if (fovScript.visibleTargets.Count != 0)
        {

            GameObject playerToFollow = fovScript.visibleTargets[0];

            foreach (GameObject g in fovScript.visibleTargets)
            {
                PlayerMovement moveScript = g.GetComponent<PlayerMovement>();

                if (!moveScript.disabled)
                {
                    playerToFollow = g;
                    agent.SetDestination(g.transform.position);
                    break;

                }
            }
            PlayerMovement playerMoveScript = playerToFollow.GetComponent<PlayerMovement>();

            //if guard is next to player then disable his ass
            if (Mathf.Abs(transform.position.x - playerToFollow.transform.position.x) <= 1f && Mathf.Abs(transform.position.z - playerToFollow.transform.position.z) <= 1f && !playerMoveScript.disabled)
            {
                
         
                playerMoveScript.disabled = true;


            }

            

                
        }
        
        //below loops through patrol path
        if (start)
        {
            agent.SetDestination(patrolPath[currDes]);
            start = false;
        }

        
        
        //if destination has been reached, the guard moves to the next cords in the patrol path
        if (Mathf.Abs(transform.position.x - agent.destination.x) <= 1f && Mathf.Abs(transform.position.z - agent.destination.z) <= 1f)
        {
            
            if (currDes == patrolPath.Count - 1)
            {
                currDes = 0;
            }
            else currDes++;

            //Debug.Log(currDes);
            
            agent.SetDestination(patrolPath[currDes]);
        }

       
        
    }
    void Start() {
        
    }
   
    
   
}
