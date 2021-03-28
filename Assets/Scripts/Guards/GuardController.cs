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
    private bool playersSpotted = false;
    public static GuardController Instance { get; private set; }

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

        if (PhotonNetwork.LocalPlayer.IsMasterClient && !playersSpotted)
        {
            cutSceneIfSpotted();
        }  
    }
    
    void Update() {
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
    public void GetClosestGuard(float x, float y, float z) {
    // Returns closest guard to a position
        Vector3 targetPosition = new Vector3(x, y, z);
        NavMeshAgent closestGuard = null;
        float closestDistance = 1000;
        foreach (GuardMovement guard in guardMovements) {
            Vector3 agentPos = guard.agent.transform.position;

            float distance = Vector3.Distance(agentPos, targetPosition);
            if (distance < closestDistance && guard.state != State.chase) {
                closestDistance = distance;
            }
                closestGuard = guard.agent;
        }

        closestGuard.SetDestination(targetPosition);
    }

    public void cutSceneIfSpotted()
    {
        foreach (GuardMovement guard in guardMovements)
        {
            if (guard.state == State.chase)
            {   
                
                //update room properties with the guard that found a player so we know where to cutscene to
                Vector3 guardPos = guard.gameObject.transform.position;

                CameraControlPlayer[] players = GameObject.FindObjectsOfType<CameraControlPlayer>();
                
                
                foreach(CameraControlPlayer g in players)
                {
                    PhotonView v = g.gameObject.GetComponent<PhotonView>();
                    
                    v.RPC("RpcCutScene", v.Controller, (object)guardPos, -4, 8, (object)(new Vector3(75, 0, 0)), "The Police have been alerted!");
                }

                //g.GetComponent<PhotonView>().RPC("RpcCutScene", g.GetComponent<PhotonView>().Controller, (object)guardPos, -4, 8, (object)(new Vector3(75, 0, 0)), "The Police have been alerted");

                playersSpotted = true;
                break;
            }
        }

        
    }


    public void disableAllguards(bool value)
    {
        foreach(GuardMovement guard in guardMovements) {
            guard.agent.isStopped = value;
        }
    }




}
