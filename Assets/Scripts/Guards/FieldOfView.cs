using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;

    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public NavMeshAgent agent;

    [HideInInspector]
    public List<GameObject> visibleTargets = new List<GameObject>();

    //calls FindVisibleTargets after every 'delay' seconds, this is started with a coroutine in start() method when guard object is instiated.
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    //finds visible targets within viewing radius + angle, visibleTarget list is cleared everytime it's run to avoid duplicates. This is called used a Enumerator with some delay.
    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //look through all the objects with target tag
        for(int i = 0; i<targetsInView.Length; i++)
        {
            GameObject target = targetsInView[i].gameObject;
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

            //if the object is within the viewangle
            if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {   
                //we get the distance to the target
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                //checks if obstacle is in way
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    
                    
                    //we can see the target, we add this to our visibletargets list

                    visibleTargets.Add(target);
                    
                }
            }
        }
    }

    //takes in an angle and gives its direction 
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
