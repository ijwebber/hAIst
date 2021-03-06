using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public class Laser : MonoBehaviourPun
{

    private LineRenderer lr;

    [SerializeField]
    private Transform startPoint; 
    [SerializeField] private Argon argon;
    [SerializeField] private SoundController soundController;

    GameObject character;

    [SerializeField] private AudioController audioController;

    public int wiresID;
    public bool disabled = false;
    public RaycastHit hit;
    GuardController guardController;
    
    void Start()
    {
        lr = GetComponent<LineRenderer>();        
        guardController = GameObject.FindObjectOfType<GuardController>();
    }
    
    void Update()
    {
        if (!disabled)
        {
            // Display laser beam up to first collision
            lr.SetPosition(0, startPoint.position);
            if (Physics.Raycast(transform.position, -transform.right, out hit))
            {
                if (hit.collider)
                {
                    lr.SetPosition(1, hit.point);

                }
                if (hit.transform.tag == "Player")
                {

                    character = hit.collider.gameObject;

                    Hashtable setSpotted = new Hashtable() { { "spotted", true }, { "spottingGuardLocation", null }, { "cutSceneDone", true } };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(setSpotted);
                    guardController.MoveClosestGuard(hit.transform.position);

                    if (!argon.tripped) {
                        argon.gameObject.SetActive(true);
                        argon.fillArgon();
                        soundController.grid.updateWalls();
                    }
                }
            }

            else
            {
                lr.SetPosition(1, -transform.right * 5000);
            }
        }
        
    }

    // Disables this laser for everyone
    [PunRPC]
    void disableLaser()
    {
        this.disabled = true;
        this.GetComponent<LineRenderer>().enabled = false;
    }

    // Enables this laser for everyone
    [PunRPC]
    void enableLaser()
    {
        this.disabled = false;
        this.GetComponent<LineRenderer>().enabled = true;
    }
}
