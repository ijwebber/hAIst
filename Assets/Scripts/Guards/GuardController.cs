using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    public Grid localGrid;
    private GuardMovement[] guardMovements;

    // Start is called before the first frame update
    void Start()
    {
        this.localGrid = new Grid(202,122,1f);
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
