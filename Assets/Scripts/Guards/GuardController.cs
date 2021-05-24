using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

public class GuardController : MonoBehaviourPun
{
    public LayerMask obstacleMask;
    public Grid localGrid;
    public Sprite sus;
    public Sprite exclamation;
    public SoundController soundController;
    public GuardMovement[] guardMovements;
    public PlayerController playerController;
    public bool playersSpotted = false;
    private float baseMoveSpeed, baseRadius;

    public static GuardController Instance { get; private set; }
    [SerializeField] private MoveSpotLight[] Spotlights;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // this.localGrid = new Grid(202,122,.5f);
        // set new grid for keeping track of local sound instances
        this.localGrid = new Grid(202,122,1);
        guardMovements = GameObject.FindObjectsOfType<GuardMovement>();
    }
    
    // when swat guards are spawned in, update guard list
    public void swat() {
        this.photonView.RPC("swatUpdate", RpcTarget.All);
        guardMovements = GameObject.FindObjectsOfType<GuardMovement>();
    }

    [PunRPC]
    //update swat guards for everyone
    void swatUpdate() {
        guardMovements = GameObject.FindObjectsOfType<GuardMovement>();
        foreach (var guard in guardMovements)
        {
            if (guard.Swat) {
                guard.walkSpeed = baseMoveSpeed*1.8f;
                guard.chaseSpeed = baseMoveSpeed*1.8f*1.8f;
                guard.GetComponent<FieldOfView>().viewRadius = baseRadius*1.5f;
            } else {
                guard.walkSpeed = baseMoveSpeed;
                guard.chaseSpeed = baseMoveSpeed*2;
                guard.GetComponent<FieldOfView>().viewRadius = baseRadius;
            }
        }
    }

    // sound emission set
    public void setValue(Vector3 position, float intensity) {
        localGrid.SetValue(position, (int)intensity);
    }

    public double getValue(Vector3 worldPosition) {
        localGrid.getXY(worldPosition, out int x, out int y);
        return localGrid.GetValue(x, y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        localGrid.updateNodes();

        //we only want to run this once, every client shouldn't run this otherwise we would get multiple rpc calls
        if (PhotonNetwork.LocalPlayer.IsMasterClient && !playersSpotted)
        {
            cutSceneIfSpotted();
        }  
    }

    [PunRPC]
    // sync guard settings accross clients
    void guardUpdate() {
        guardMovements = GameObject.FindObjectsOfType<GuardMovement>();
        baseMoveSpeed = (float)PhotonNetwork.CurrentRoom.CustomProperties["movespeedSetting"];
        baseRadius = (int)PhotonNetwork.CurrentRoom.CustomProperties["radiusSetting"];
        foreach (var guard in guardMovements)
        {
            if (!guard.Swat) {
                guard.walkSpeed = baseMoveSpeed;
                guard.chaseSpeed = baseMoveSpeed*2;
                guard.GetComponent<FieldOfView>().viewRadius = baseRadius;
            }
        }
    }

    public void setGuards() {
        guardMovements = GameObject.FindObjectsOfType<GuardMovement>();
        baseMoveSpeed = (float)PhotonNetwork.CurrentRoom.CustomProperties["movespeedSetting"];
        baseRadius = (int)PhotonNetwork.CurrentRoom.CustomProperties["radiusSetting"];
        foreach (var guard in guardMovements)
        {
            if (!guard.Swat) {
                guard.walkSpeed = baseMoveSpeed;
                guard.chaseSpeed = baseMoveSpeed*2;
                guard.GetComponent<FieldOfView>().viewRadius = baseRadius;
            }
        }
        this.photonView.RPC("guardUpdate", RpcTarget.All);
    }
    
    void Update() {
        // keep velocities consistent with main grid
        localGrid.velocities = soundController.grid.velocities;
    }

    public bool inChase() {
        foreach (GuardMovement guard in guardMovements) {
            if (guard.state == State.chase) {
                //return true;
            }
        }

        return false;
    }

    public bool getSpotted()
    {
        return playersSpotted;
    }

    public bool MoveAgent(NavMeshAgent agent, Vector3 position) {
        return agent.SetDestination(position);
    }
    public void MoveClosestGuard(Vector3 targetPosition) {
        this.GetComponent<PhotonView>().RPC("GetClosestGuard", RpcTarget.MasterClient, targetPosition.x, targetPosition.y, targetPosition.z);
    }

    [PunRPC]
    void GetClosestGuard(float x, float y, float z) {
    // Returns closest guard to a position
        Vector3 targetPosition = new Vector3(x, y, z);
        GuardMovement closestGuard = null;
        float closestDistance = 100;
        foreach (GuardMovement guard in guardMovements) {
            Vector3 agentPos = guard.agent.transform.position;

            float distance = Vector3.Distance(agentPos, targetPosition);
            if (distance < closestDistance && (guard.state == State.normal || guard.state == State.suspicious) && !guard.sleeping) {
                closestDistance = distance;
                closestGuard = guard;
            }
        }
        if (closestGuard != null) {
            closestGuard.state = State.suspicious;
            closestGuard.agent.SetDestination(targetPosition);
        }
    }

    public void cutSceneIfSpotted()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GuardMovement guard in guardMovements)
            {
                if (guard.state == State.chase)
                {

                    //this is the cutscene location
                    Vector3 guardPos = guard.gameObject.transform.position;

                    //get list of players
                    CameraControlPlayer[] players = GameObject.FindObjectsOfType<CameraControlPlayer>();


                    //each player needs to run the cutscene code on their own 'CameraControl' script, so we send a targeted rpc to each individual player and the target photonview is owned by that specfic player.
                    foreach (CameraControlPlayer g in players)
                    {
                        PhotonView v = g.gameObject.GetComponent<PhotonView>();

                        v.RPC("RpcCutScene", v.Controller, guard.photonView.ViewID, guard.GetComponent<GuardMovement>().chasedPlayer.GetComponent<PhotonView>().ViewID, "The Police have been alerted!", 1);
                    }


                    //set to true so it doesn't run again when guards spot players
                    playersSpotted = true;

                    Hashtable endTriggered = new Hashtable() { { "triggered", true } };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(endTriggered);

                    break;
                }
            }
        }

        
    }
    public void cutsceneSpotlight(GameObject SpotLight, GameObject other) {
        CameraControlPlayer[] players = GameObject.FindObjectsOfType<CameraControlPlayer>();

        //each player needs to run the cutscene code on their own 'CameraControl' script, so we send a targeted rpc to each individual player and the target photonview is owned by that specfic player.
        foreach(CameraControlPlayer g in players)
        {
            PhotonView v = g.gameObject.GetComponent<PhotonView>();
            
            v.RPC("RpcCutScene", v.Controller, SpotLight.GetComponent<PhotonView>().ViewID, other.gameObject.GetComponent<PhotonView>().ViewID, "The Police have been alerted!", 1);
        }
        
        //set to true so it doesn't run again when guards spot players
        playersSpotted = true;

        Hashtable endTriggered = new Hashtable() { { "triggered", true } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(endTriggered);
    }


    public void disableAllguards(bool value)
    {   
        //refresh array

        guardMovements = GameObject.FindObjectsOfType<GuardMovement>();

        foreach (GuardMovement guard in guardMovements) {
            guard.agent.isStopped = value;
        }
        foreach(MoveSpotLight spot in Spotlights) {
        if (value) {
            spot.speed = 0;
        } else {
            spot.speed = 1;
        }
        }
    }




}
