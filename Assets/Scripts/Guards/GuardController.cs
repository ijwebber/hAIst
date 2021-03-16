using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    public LayerMask obstacleMask;
    public Grid localGrid;
    public Sprite sus;
    public Sprite exclamation;
    private GuardMovement[] guardMovements;
    public PlayerController playerController;

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
    }
    
    void Update() {
        foreach (GuardMovement guard in guardMovements) {
            if (Physics.Raycast(guard.gameObject.transform.position, (playerController.player.transform.position - guard.gameObject.transform.position).normalized, playerController.viewRadius+2,obstacleMask)) {
                switch (guard.state)
                {
                    case State.normal:
                        guard.sprite.sprite = null;
                        break;
                    case State.suspicious:
                        guard.sprite.sprite = sus;
                        break;
                    case State.chase:
                        guard.sprite.sprite = exclamation;
                        break;
                }
            } else {
                guard.sprite.sprite = null;
            }
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


}
