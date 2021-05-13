using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

public class GuardController : MonoBehaviour
{
    public LayerMask obstacleMask;
    public Grid localGrid;
    public Sprite sus;
    public Sprite exclamation;
    public SoundController soundController;
    public GuardMovement[] guardMovements;
    public PlayerController playerController;
    public bool playersSpotted = false;

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
    // Start is called before the first frame update
    void Start()
    {
        // this.localGrid = new Grid(202,122,.5f);
        this.localGrid = new Grid(202,122,1);
        guardMovements = GameObject.FindObjectsOfType<GuardMovement>();
    }
    
    public void swat() {
        guardMovements = GameObject.FindObjectsOfType<GuardMovement>();
    }

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
    
    void Update() {
        localGrid.velocities = soundController.grid.velocities;
        if (guardMovements.Length == 0) {
            guardMovements = GameObject.FindObjectsOfType<GuardMovement>();
        }
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
        Debug.Log("Move closest guard here");
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
        foreach (GuardMovement guard in guardMovements)
        {
            if (guard.state == State.chase)
            {   
                
                //this is the cutscene location
                Vector3 guardPos = guard.gameObject.transform.position;

                //get list of players
                CameraControlPlayer[] players = GameObject.FindObjectsOfType<CameraControlPlayer>();

                
                //each player needs to run the cutscene code on their own 'CameraControl' script, so we send a targeted rpc to each individual player and the target photonview is owned by that specfic player.
                foreach(CameraControlPlayer g in players)
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
