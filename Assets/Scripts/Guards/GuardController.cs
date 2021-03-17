using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GuardController : MonoBehaviour
{
    public Grid localGrid;
    public static GuardController Instance { get; private set; }
    private GuardMovement[] guardMovements;
    
    //this is an event that only happens once per round
    private bool playersSpotted = false;

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

    public bool inChase() {
        foreach (GuardMovement guard in guardMovements) {
            if (guard.state == State.chase) {
                //return true;
            }
        }

        return false;
    }

    public void cutSceneIfSpotted()
    {
        foreach (GuardMovement guard in guardMovements)
        {
            if (guard.state == State.chase)
            {   
                
                //update room properties with the guard that found a player so we know where to cutscene to
                Vector3 guardPos = guard.gameObject.transform.position;
                Hashtable setSpotted = new Hashtable() { { "spotted", true }, { "spottingGuardLocation", new Vector3 (guardPos.x, guardPos.y, guardPos.z) }, { "cutSceneDone", false} };
                PhotonNetwork.CurrentRoom.SetCustomProperties(setSpotted);
                
                //freeze all guards
                disableAllguards(true);

                playersSpotted = true;
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
