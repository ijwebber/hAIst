﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public class LaserDown : MonoBehaviourPun
{

    private LineRenderer lr;

    [SerializeField]
    private Transform startPoint; 

    GameObject character;

    public AudioController song;


    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0,startPoint.position);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.right,out hit)){
            if(hit.collider){
                lr.SetPosition(1,hit.point);

            }
            if(hit.transform.tag == "Player"){
                character = hit.collider.gameObject;

                PlayerMovement playerMoveScript = character.GetComponent<PlayerMovement>();
                Hashtable setSpotted = new Hashtable() { { "spotted", true }, { "spottingGuardLocation", null }, { "cutSceneDone", true } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(setSpotted); 
                song.PlayIntenseTheme(); 
                
            }
        }

        else{
            lr.SetPosition(1,-transform.right * 5000);
        }
        
    }
}
