using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum State {
    normal      = 0,
    suspicious  = 1,
    chase       = 2,
    disabled    = 3
}

public class GuardMovement : MonoBehaviourPun, IPunObservable
{
    

    public GuardController guardController;
    private PlayerController playerController;
    private GameController gameController;
    private AudioController audioController;
    public NavMeshAgent agent;
    public SoundVisual soundVis;
    public AudioSource heySound;
    public List<GameObject> specials = new List<GameObject>();
    public List<Vector3> patrolPath = new List<Vector3> {new Vector3(-44.0f, 13.38f, 27.83f), new Vector3(-8.0f, 13.38f, 27.7f), new Vector3(-6.2f, 13.38f, 4.3f), new Vector3(-32.4f, 13.21f, 13.0f)};
    private int currDes = 0;
    public State state;
    private State previousState;
    public float chaseSpeed;
    public bool sleepy;
    public bool sleeping;
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
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private new Rigidbody rigidbody;
    public bool Swat;
    

    private bool timedOut = false;
    
    private void Awake()
    {
        this.gameController = GameObject.FindObjectOfType<GameController>();
        audioController = GameObject.FindObjectOfType<AudioController>();
    }

    private void Start() {
        // initialise guards
        if (PhotonNetwork.IsMasterClient) {
            agent.SetDestination(patrolPath[currDes]);
        }
        player = GameObject.Find("Timmy");
        this.state = State.normal;
        this.guardController = GameObject.FindObjectOfType<GuardController>();
        this.playerController = GameObject.FindObjectOfType<PlayerController>();
        fovScript = GetComponent<FieldOfView>();
        heySound = GetComponent<AudioSource>();
        this.rigidbody = this.GetComponent<Rigidbody>();
    }
  
  // lag compensation
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        stream.SendNext(rigidbody.position);
        stream.SendNext(rigidbody.rotation);
        stream.SendNext(rigidbody.velocity);
    }
    else
    {
        networkPosition = (Vector3) stream.ReceiveNext();
        networkRotation = (Quaternion) stream.ReceiveNext();
        if (rigidbody != null) {
            rigidbody.velocity = (Vector3) stream.ReceiveNext();

            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
            networkPosition += (rigidbody.velocity * lag);
        }

    }
}

    // when guard is knocked out with specials, remove all carried specials
    public void removeSpecials() {
        if(specials.Count > 0) {
            int currSpec = 0;
            if (PhotonNetwork.CurrentRoom.CustomProperties["roomSpecial"] != null) {
                currSpec = (int)PhotonNetwork.CurrentRoom.CustomProperties["roomSpecial"];
            }
            Hashtable hash = new Hashtable();
            currSpec+=specials.Count;
            gameController.gameState+=specials.Count;
            string localMessage = "You've recaptured ";
            string remoteMessage = PhotonNetwork.NickName + " has recaptured ";
            if (specials.Count == 1) {
                localMessage += specials[0].GetComponent<CollectableItem>().itemName + "!";
                remoteMessage += specials[0].GetComponent<CollectableItem>().itemName + "!";
            } else {
                localMessage += specials.Count + " artifacts!";
                remoteMessage += specials.Count + " artifacts!";
            }
            int currentPlayerScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["score"];
            gameController.playerUpdates.updateDisplay(localMessage);
            gameController.updateDisp(remoteMessage);
            foreach (var spec in specials)
            {
                currentPlayerScore += spec.GetComponent<CollectableItem>().value;
                spec.GetComponent<CollectableItem>().stolen = true;
                spec.GetComponent<CollectableItem>().syncStolen(true, null);
                spec.GetComponent<CollectableItem>().guardPoint = null;
                // add specials to player who knocked the guard down
                playerController.Specials.Add(spec);
            }
            // update score
            Hashtable playerHash = new Hashtable();
            playerHash.Add("score", currentPlayerScore);
            hash.Add("roomSpecial", currSpec);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);
            int specCount = (int) PhotonNetwork.LocalPlayer.CustomProperties["specialStolen"];
            playerHash.Add("specialStolen", specCount + specials.Count);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);
            gameController.regress = false;
            this.GetComponent<PhotonView>().RPC("ClearSpecials", RpcTarget.All);
            Debug.Log("Recaptured painting");
        }
    }

    void FixedUpdate() {
        if (!photonView.IsMine) {
            //lag compensation
            rigidbody.position = Vector3.MoveTowards(rigidbody.position, networkPosition, Time.fixedDeltaTime);
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        }
    }

    void Update()
    {
        if (previousState != state && PhotonNetwork.IsMasterClient) {
            previousState = state;
            //sync state
            this.photonView.RPC("SyncState", RpcTarget.Others, state);
        }
        if (this.state == State.disabled || guardDisabled) //runs if guard is disabled
        {
            state = State.disabled;
            //check if the timer has already been started, if so don't start it again
            if (!timedOut)
            {
                agent.isStopped = true;
                timedOut = true;
                StartCoroutine(disableForTime(3.0f));
            }
        }
        else
        {
            //if a target is in fov then path to that target
            if (fovScript.visibleTargets.Count != 0 && this.state != State.disabled && !sleeping)
            {
                GameObject playerToFollow = fovScript.visibleTargets[0];
                foreach (GameObject g in fovScript.visibleTargets)
                {
                    PlayerMovement moveScript = g.GetComponent<PlayerMovement>();

                    if (!moveScript.disabled)
                    {
                        playerToFollow = g;
                        chasedPlayer = g;

                        if (this.state != State.chase && playerToFollow.GetComponent<PhotonView>().IsMine)
                        {
                            audioController.PlayGuardHey();
                        }

                        if (PhotonNetwork.IsMasterClient) {
                            agent.SetDestination(g.transform.position);
                        }
                        if (agent.speed != chaseSpeed)
                        {
                            agent.speed = chaseSpeed;
                        }

                        if (PhotonNetwork.IsMasterClient) {
                            this.state = State.chase;
                        }

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
                            // if player is not in invulnerable state disable
                            playerMoveScript.disabled = true;
                            audioController.PlayPlayerOof();
                            this.state = State.normal;
                            agent.ResetPath();
                            playerToFollow.GetComponent<PhotonView>().RPC("syncDisabled", RpcTarget.All, true);
                            if (playerController.Specials.Count > 0) {  
                                // specials = playerController.Specials;
                                gameController.regress = true;
                                string serializedObjects = "";
                                int i = 0;
                                Hashtable specHash = new Hashtable();
                                // set special stolen to 0
                                specHash.Add("specialStolen", 0);
                                PhotonNetwork.LocalPlayer.SetCustomProperties(specHash);
                                int currSpec = 0;
                                if (PhotonNetwork.CurrentRoom.CustomProperties["roomSpecial"] != null) {
                                    currSpec = (int)PhotonNetwork.CurrentRoom.CustomProperties["roomSpecial"];
                                }
                                Hashtable hash = new Hashtable();
                                string localMessage = "You've been knocked down and lost ";
                                string remoteMessage = PhotonNetwork.NickName + " has been knocked down and lost ";
                                // output info message to bottom left of screen
                                if (playerController.Specials.Count == 1) {
                                    localMessage += playerController.Specials[0].GetComponent<CollectableItem>().itemName + "!";
                                    remoteMessage += playerController.Specials[0].GetComponent<CollectableItem>().itemName + "!";
                                } else {
                                    localMessage += playerController.Specials.Count + " artifacts!";
                                    remoteMessage += playerController.Specials.Count + " artifacts!";
                                }
                                gameController.playerUpdates.updateDisplay(localMessage);
                                gameController.updateDisp(remoteMessage);
                                gameController.playerUpdates.updateDisplay("Knock out the guard to reclaim the artifacts");
                                gameController.updateDisp("Knock out the guard to reclaim the artifacts");
                                Hashtable specProps = PhotonNetwork.LocalPlayer.CustomProperties;
                                int currentPlayerScore = (int) specProps["score"]; 
                                // update game state and pointers
                                foreach (var spec in playerController.Specials)
                                {
                                    currSpec--;
                                    gameController.gameState -= 1;
                                    serializedObjects += spec.name + ",";
                                    spec.GetComponent<CollectableItem>().stolen = false; // point to guard;
                                    spec.GetComponent<CollectableItem>().syncStolen(false, this.gameObject);
                                    spec.GetComponent<CollectableItem>().guardPoint = this.gameObject; // point to guard;
                                    i++;
                                    // Increase the current score by the value
                                    currentPlayerScore -= spec.GetComponent<CollectableItem>().value;
                                    
                                }
                                // Create a hashtable entry with the new score
                                Hashtable newHash = new Hashtable();
                                newHash.Add("score", currentPlayerScore);
                                PhotonNetwork.LocalPlayer.SetCustomProperties(newHash);
                                hash.Add("roomSpecial", currSpec);
                                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
                                serializedObjects.TrimEnd(","[0]);
                                this.GetComponent<PhotonView>().RPC("updateGuardSpecials", RpcTarget.All, serializedObjects);
                                playerController.Specials.Clear();
                                Debug.Log("Guard has Captured painting");
                            } else {
                                gameController.playerUpdates.updateDisplay("You have been knocked down! Wait for your crew to help you back up!");
                                gameController.updateDisp(PhotonNetwork.NickName + " has been knocked down!");
                            }
                            Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
                            int currentDowns = 0;
                            if (props["downs"] != null) {
                                currentDowns = (int) props["downs"];
                            }
                            currentDowns++;
                            Hashtable playerHash = new Hashtable();
                            playerHash.Add("downs", currentDowns);
                            PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);
                        }
                    }
                }
            }
            else
            {
                // check for sound
                if (guardController.localGrid.GetValue(transform.position) > 0 && this.state != State.disabled)
                {
                    Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
                    int currentAlerts = 0;
                    if (props["alerts"] != null) {
                        currentAlerts = (int) props["alerts"];
                    }
                    currentAlerts++;
                    Hashtable playerHash = new Hashtable();
                    playerHash.Add("alerts", currentAlerts);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);
                    Vector3 playerPosition = player.transform.position;
                    this.photonView.RPC("snitch", RpcTarget.MasterClient, playerPosition.x, playerPosition.y, playerPosition.z);
                }
                else
                {

                    //if destination has been reached, the guard moves to the next cords in the patrol path
                    if (Mathf.Abs(transform.position.x - agent.destination.x) <= 1f && Mathf.Abs(transform.position.z - agent.destination.z) <= 1f)
                    {
                        agent.speed = walkSpeed;

                        if (currDes == patrolPath.Count - 1)
                        {
                            currDes = 0;
                        }
                        else currDes++;

                        if (sleepy && sleeping)
                        {
                            Quaternion target = Quaternion.Euler(0, -90, 0);
                            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 3f);
                        }
                        //Debug.Log(currDes);

                        if (PhotonNetwork.IsMasterClient) {
                            
                            state = State.normal;
                            agent.SetDestination(patrolPath[currDes]);
                        }
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

            if (PhotonNetwork.IsMasterClient)
            {
                if (Mathf.Abs(transform.position.x - patrolPath[0].x) <= 1f && Mathf.Abs(transform.position.z - patrolPath[0].z) <= 1f && sleepy && agent.velocity.magnitude == 0)
                {
                    sleeping = true;
                    this.photonView.RPC("SyncSleeping", RpcTarget.All, true);
                }
                else if (agent.velocity.magnitude > 0 && sleepy)
                {
                    sleeping = false;
                    this.photonView.RPC("SyncSleeping", RpcTarget.All, false);
                }
            }
        }        
    }

    [PunRPC]
    void SyncState(State state) {
        this.state = state;
    }

    [PunRPC]
    void SyncSleeping(bool state)
    {
        sleeping = state;
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
        if (this.state == State.normal || this.state == State.suspicious) {
            this.state = State.suspicious;
            agent.SetDestination(new Vector3(x,y,z));
        } else {
            // if chasing somebody, move someone else (maybe don't)
            // guardController.MoveClosestGuard(new Vector3(x,y,z));
        }
    }

    //timer coroutine
    IEnumerator disableForTime(float disableTime)
    {
        // if (this.specials.Count)
        
        yield return new WaitForSeconds(disableTime);

        guardDisabled = false;
        if (PhotonNetwork.IsMasterClient) {
            this.state = State.normal;
        }
        timedOut = false;
        agent.isStopped = false;
    }

    [PunRPC]
    void syncGuardDisabled(bool value)
    {
        if (value)
        {
            this.state = State.disabled;
        }
    }
}
