using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GuardController : MonoBehaviourPun
{
    public LayerMask obstacleMask;
    public Grid localGrid;
    public GuardMovement[] guardMovements;
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        // this.localGrid = new Grid(202,122,.5f);
        this.localGrid = new Grid(202,122,1);
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
        if (guardMovements.Length == 0) {
            guardMovements = GameObject.FindObjectsOfType<GuardMovement>();
        }
        localGrid.updateNodes();
    }
    
    public bool inChase() {
        foreach (GuardMovement guard in guardMovements) {
            if (guard.state == State.chase) {
                //return true;
            }
        }

        return false;
    }


}
