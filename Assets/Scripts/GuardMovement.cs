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

        if (fovScript.visibleTargets.Count != 0)
        {
            agent.SetDestination(fovScript.visibleTargets[0].position);
        }
        
        if (start)
        {
            agent.SetDestination(patrolPath[currDes]);
            start = false;
        }

        Debug.Log(agent.pathStatus);
        Vector3 pos = transform.position;
        //Debug.Log(pos.z.ToString());
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
