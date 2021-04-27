using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using System;

public enum State {
    normal      = 0,
    suspicious  = 1,
    chase       = 2,
    disabled    = 3
}

public class GuardMovement : MonoBehaviourPun
{
    

    public GuardController guardController;
    private PlayerController playerController;
    private GameController gameController;
    public NavMeshAgent agent;
    public SoundVisual soundVis;
    public AudioSource heySound;
    public List<GameObject> specials = new List<GameObject>();
    public List<Vector3> patrolPath = new List<Vector3> {new Vector3(-44.0f, 13.38f, 27.83f), new Vector3(-8.0f, 13.38f, 27.7f), new Vector3(-6.2f, 13.38f, 4.3f), new Vector3(-32.4f, 13.21f, 13.0f)};
    private int currDes = 0;
    public State state;
    public float chaseSpeed;
    public float walkSpeed;
    public GameObject chasedPlayer;
    private bool start = true;
    private float guardReach = 2f; //reach length of guards
    public bool guardDisabled = false;
    private GameObject player;
    private bool listening = true;
    private bool onTheWay = false;
    private float xDir = 0;
    private float yDir = 0;
    private FieldOfView fovScript;
    

    private bool timedOut = false;
    
    // public void setGrid(Grid grid) {
    //     this.grid = grid;
    // }

    private void Awake()
    {
        this.gameController = GameObject.FindObjectOfType<GameController>();
    }

    private void Start() {
        agent.SetDestination(patrolPath[currDes]);
        player = GameObject.Find("Timmy");
        this.state = State.normal;
        this.guardController = GameObject.FindObjectOfType<GuardController>();
        this.playerController = GameObject.FindObjectOfType<PlayerController>();
        fovScript = GetComponent<FieldOfView>();
        heySound = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (this.state == State.disabled || guardDisabled) //runs if guard is disabled
        {
            //check if the timer has already been started, if so don't start it again
            if (!timedOut)
            {
                agent.isStopped = true;
                timedOut = true;
                foreach (var spec in specials)
                {
                    spec.GetComponent<CollectableItem>().stolen = true;
                    spec.GetComponent<CollectableItem>().guardPoint = null;
                    playerController.Specials.Add(spec);
                }
                if(specials.Count > 0) {
                    gameController.gameState++;
                    gameController.regress = false;
                    this.GetComponent<PhotonView>().RPC("ClearSpecials", RpcTarget.MasterClient);
                    Debug.Log("Recaptured painting");
                }
                StartCoroutine(disableForTime(3.0f));
            }
        }
        else
        {
            //if a target is in fov then path to that target
            if (fovScript.visibleTargets.Count != 0 && this.state != State.disabled)
            {

                GameObject playerToFollow = fovScript.visibleTargets[0];

                foreach (GameObject g in fovScript.visibleTargets)
                {
                    
                    PlayerMovement moveScript = g.GetComponent<PlayerMovement>();

                    if (!moveScript.disabled)
                    {   
                        playerToFollow = g;
                        chasedPlayer = g;


                        if(this.state != State.chase && playerToFollow.GetComponent<PhotonView>().IsMine)
                        {
                            heySound.Play();
                        }
                        
                        agent.SetDestination(g.transform.position);
                        if(agent.speed != chaseSpeed)
                        {
                            agent.speed = chaseSpeed;
                        }
                        
                        this.state = State.chase;
                        
                        break;
                    }
                }
                PlayerMovement playerMoveScript = playerToFollow.GetComponent<PlayerMovement>();

                //if guard is next to player then disable his ass
                if (playerToFollow.GetComponent<PhotonView>().IsMine && Mathf.Abs(transform.position.x - playerToFollow.transform.position.x) <= guardReach && Mathf.Abs(transform.position.z - playerToFollow.transform.position.z) <= guardReach && !playerMoveScript.disabled && !guardDisabled)
                {
                    if (playerController.shield) {
                        playerController.disableShield();
                    } else {
                        if (playerController.invincibleFrames == 0) {
                            playerMoveScript.disabled = true;
                            this.state = State.normal;
                            agent.ResetPath();
                            playerToFollow.GetComponent<PhotonView>().RPC("syncDisabled", RpcTarget.All, true);
                            if (playerController.Specials.Count > 0) {
                                // specials = playerController.Specials;
                                gameController.gameState -= 1;
                                gameController.regress = true;
                                string serializedObjects = "";
                                int i = 0;
                                foreach (var spec in playerController.Specials)
                                {
                                    serializedObjects += spec.name + ",";
                                    spec.GetComponent<CollectableItem>().stolen = false; // point to guard;
                                    spec.GetComponent<CollectableItem>().guardPoint = this.gameObject; // point to guard;
                                    i++;
                                }
                                serializedObjects.TrimEnd(","[0]);
                                this.GetComponent<PhotonView>().RPC("updateGuardSpecials", RpcTarget.All, serializedObjects);
                                playerController.Specials.Clear();
                                Debug.Log("Guard has Captured painting");
                            }
                        }
                    }

                }
            }
            else
            {
                // check for sound
                if (guardController.localGrid.GetValue(transform.position) > 0 && this.state != State.disabled)
                {
                    Vector3 playerPosition = player.transform.position;
                    Debug.Log("I hear a who at // " + playerPosition);
                    this.photonView.RPC("snitch", RpcTarget.MasterClient, playerPosition.x, playerPosition.y, playerPosition.z);
                }
                else
                {

                    //if destination has been reached, the guard moves to the next cords in the patrol path
                    if (Mathf.Abs(transform.position.x - agent.destination.x) <= 1f && Mathf.Abs(transform.position.z - agent.destination.z) <= 1f)
                    {
                        state = State.normal;
                        agent.speed = walkSpeed;

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
                        PhotonView view = PhotonView.Get(this);
                        Player p = g.GetComponent<PhotonView>().Controller;

                        g.GetComponent<PhotonView>().RPC("KnockOut", p, view.ViewID);
                }
            }
        }        
    }

    [PunRPC]
    void updateGuardSpecials(string serializedObjects) {
        Debug.Log("Received special update with " + serializedObjects);
        string[] outObjs = serializedObjects.Split(","[0]);
        GameObject Stealables = GameObject.Find("StealItems");
        foreach (string ob in outObjs)
        {
            if (ob != "") {
                Debug.Log("!" + ob + "!");
                specials.Add(Stealables.transform.Find(ob).gameObject);
                Debug.Log(specials[0]);
            }
            // Debug.Log("Updated specs " + specials.ToString());
        }
    }

    [PunRPC]
    void ClearSpecials() {
        specials.Clear();
    }

    [PunRPC]
    void snitch(float x, float y, float z) {
        // receive new sound source and update local grid
        if (this.state == State.normal) {
            this.state = State.suspicious;
            agent.SetDestination(new Vector3(x,y,z));
        }
    }

    //timer coroutine
    IEnumerator disableForTime(float disableTime)
    {
        // if (this.specials.Count)
        yield return new WaitForSeconds(disableTime);
        guardDisabled = false;
        this.state = State.normal;
        timedOut = false;
        agent.isStopped = false;
    }
}
