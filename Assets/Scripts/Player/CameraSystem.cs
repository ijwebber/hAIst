﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class CameraSystem : MonoBehaviour
{
    // Start is called before the first frame update

    public static CameraSystem Instance { get; private set; }

    public GameObject introSceneTrack;
    public GameObject playerCamTrack;
    public CinemachineBrain brain;
    public CinemachineVirtualCamera guardCaughtIn4k;
    public CinemachineVirtualCamera playerCam;

    [Range(0.6f, 1.0f)]
    public float zoomMultiplier = 1.0f;
    public GameObject gameUIReference;
    public bool introDone = false;
    public bool isCutSceneHappening = true;
    private bool playerCamActive = false;
    
    private GameObject guardShotReference;
    private GameObject player;
    private GameObject securityCameraReference;
    private float startingHeight;
    private float startingDistance;


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
        introCutSceneSetup();
        startingHeight = playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;
        startingDistance = playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z;



    }

    // Update is called once per frame
    void Update()
    {   
        //for the first escape key press we want to end the cutscene and skip to the player cam
        if(!introDone && Input.GetKeyDown(KeyCode.Escape))
        {
            introEnd();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
        {
            zoomMultiplier += Input.GetAxis("Mouse ScrollWheel")/-5;
            zoomMultiplier = Mathf.Clamp(zoomMultiplier, 0.6f, 1.0f);
            playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = startingHeight * zoomMultiplier;
            playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = startingDistance * zoomMultiplier;
        }
        


    }

    public void introEnd()
    {   
        //change layer of guard back to normal, fade ui back in, turn on black
        if (!introDone)
        {
            introDone = true;
            playerCamActive = true;
            
            playerCamTrack.SetActive(true);
            introSceneTrack.SetActive(false);
            StartCoroutine(disableAfterTime(playerCamTrack, 2f));

            
            player.GetComponent<PlayerMovement>().paused = false;

            isCutSceneHappening = false;

            SetLayerRecursively(guardShotReference, 10);
            SetPaintingsLayer(13);
            SetLayerRecursively(securityCameraReference, 10);
        }
    }

    void introCutSceneSetup()
    {
        //Finding guard object for guard scene shot and setting the VC to follow and look at it
        guardShotReference = GameObject.Find("Guard3(Clone)");
        introSceneTrack.gameObject.transform.Find("CM Guard Cam").gameObject.GetComponent<CinemachineVirtualCamera>().Follow = guardShotReference.transform;
        introSceneTrack.gameObject.transform.Find("CM Guard Cam").gameObject.GetComponent<CinemachineVirtualCamera>().LookAt = guardShotReference.transform;

        //find players and disable their control whilst cutscene plays
        player = GameObject.Find("Timmy");
        player.GetComponent<PlayerMovement>().paused = true;

        //find security cam
        securityCameraReference = GameObject.Find("Camera 1");

        //setting the layers for paintings and guard so they render
        SetPaintingsLayer(default);
        SetLayerRecursively(guardShotReference, default);
        SetLayerRecursively(securityCameraReference, default);
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }
       
        obj.layer = newLayer;
       
        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void SetPaintingsLayer(int layer){
        var stealItems = GameObject.FindGameObjectsWithTag("steal");
        foreach(var items in stealItems){items.layer = layer;}
    }

    private IEnumerator disableAfterTime(GameObject g, float time)
    {
        yield return new WaitForSeconds(time);
        g.SetActive(false);
    }


    public void caughtCutScene(int guardViewID, string message, GameObject player)
    {
        isCutSceneHappening = true;
        GameObject guard = PhotonView.Find(guardViewID).gameObject;
        guardCaughtIn4k.Follow = guard.transform;
        guardCaughtIn4k.LookAt = guard.transform;
        guardCaughtIn4k.Priority = 11;

        SetLayerRecursively(guard, default);
        BarController.Instance.SetText(message);
        BarController.Instance.ShowBars();

        StartCoroutine(endCaughtCutScene(guard));

        
        
        



    }

    private IEnumerator endCaughtCutScene(GameObject guard)
    {
        yield return new WaitForSeconds(3f);


        guardCaughtIn4k.Priority = 9;
        BarController.Instance.HideBars();

        yield return new WaitForSeconds(2f);
        SetLayerRecursively(guard, 10);
        player.GetComponent<PlayerMovement>().paused = false;
        
        GuardController.Instance.disableAllguards(false);

        isCutSceneHappening = false;
    }


    

}

