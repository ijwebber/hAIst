using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class GuardMovement : MonoBehaviour
{
    

    public Grid grid;
    public NavMeshAgent agent;
    public SoundVisual soundVis;


    public List<Vector3> patrolPath = new List<Vector3> {new Vector3(-44.0f, 13.38f, 27.83f), new Vector3(-8.0f, 13.38f, 27.7f), new Vector3(-6.2f, 13.38f, 4.3f), new Vector3(-32.4f, 13.21f, 13.0f)};
    private int currDes = 0;
    private bool start = true;
    public bool guardDisabled = false;
    private bool onTheWay = false;
    private float xDir = 0;
    private float yDir = 0;
    

    private bool timedOut = false;
    
    // public void setGrid(Grid grid) {
    //     this.grid = grid;
    // }

    private void Awake()
    {
        
    }

    private void Start() {
        agent.SetDestination(patrolPath[currDes]);
        this.soundVis = GameObject.FindObjectOfType<SoundVisual>();
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
            if (Mathf.Abs(transform.position.x - playerToFollow.transform.position.x) <= 1f && Mathf.Abs(transform.position.z - playerToFollow.transform.position.z) <= 1f && !playerMoveScript.disabled && !guardDisabled)
            {
                playerMoveScript.disabled = true;
            }
        } else {
            // check for sound
            if(soundVis.grid.GetValue(transform.position) > 3 && !onTheWay) {
                onTheWay = true;
                Debug.Log("I hear a who");
                soundVis.grid.getXY(transform.position, out int x, out int y);
                double currentMax = 0;
                for (int xx = -1; xx < +1; xx++)
                {
                    for (int yy = -1; yy < +1; yy++) {
                        if(xx != 0 && yy != 0) {
                            double temp = soundVis.grid.GetValue(x+xx, y+yy);
                            if (temp > currentMax) {
                                currentMax = temp;
                                Debug.Log("new max at" + transform.position.x + " " + transform.position.z);
                                xDir = xx;
                                yDir = yy;
                            }
                        }
                    }
                    Debug.Log("max value = "+ xDir + " " + yDir);
                    Physics.Raycast(transform.position, new Vector3(xDir, 0, yDir) ,out RaycastHit hitinfo , Mathf.Infinity, (1 << 8), QueryTriggerInteraction.UseGlobal);
                    bool ok = agent.SetDestination(hitinfo.point);
                    Debug.DrawLine(transform.position, hitinfo.point, Color.white, 10);
                    Debug.Log("going to " + hitinfo.point);
                    Debug.Log("confirmed? " + ok);
                    xDir = yDir = 0;
                }
            } else {

                //if destination has been reached, the guard moves to the next cords in the patrol path
                if (Mathf.Abs(transform.position.x - agent.destination.x) <= 1f && Mathf.Abs(transform.position.z - agent.destination.z) <= 1f)
                {
                    onTheWay = false;

                    if (currDes == patrolPath.Count - 1)
                    {
                        currDes = 0;
                    }
                    else currDes++;

                    //Debug.Log(currDes);

                    agent.SetDestination(patrolPath[currDes]);
                }
            }
        }

        //check if any players are behind this guard, if they are then notify the player via RPC that they can knock out this guard
        if (fovScript.behindGuardTargets.Count != 0)
        {
            foreach (GameObject g in fovScript.behindGuardTargets)
            {
                if (Mathf.Abs(g.transform.position.x - transform.position.x) <= 1.5f && Mathf.Abs(g.transform.position.z - transform.position.z) <= 1.5f)
                {


                    PhotonView view = PhotonView.Get(this);
                    Player p = g.GetComponent<PhotonView>().Controller;

                    g.GetComponent<PhotonView>().RPC("KnockOut", p, view.ViewID);


                }
            }
        }

        if(guardDisabled) //runs if guard is disabled
        {
            
            //check if the timer has already been started, if so don't start it again
            if (!timedOut)
            {
                agent.isStopped = true;
                timedOut = true;
                StartCoroutine(disableForTime(3.0f));
            }
        }

       
        
    }

    //timer coroutine
    IEnumerator disableForTime(float disableTime)
    {
     
        yield return new WaitForSeconds(disableTime);
        guardDisabled = false;
        timedOut = false;
        agent.isStopped = false;
        
    }
}
