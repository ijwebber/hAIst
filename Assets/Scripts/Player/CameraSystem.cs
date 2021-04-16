using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject introSceneTrack;
    public GameObject playerCamTrack;
    public CinemachineBrain brain;

    public GameObject gameUIReference;
    public bool introDone = false;
    private bool playerCamActive = false;
    
    private GameObject guardShotReference;
    private GameObject player;
    private GameObject securityCameraReference;

    void Start()
    {
        introCutSceneSetup();

        
      
    }

    // Update is called once per frame
    void Update()
    {   
        //for the first escape key press we want to end the cutscene and skip to the player cam
        if(!introDone && Input.GetKeyDown(KeyCode.Escape))
        {
            introEnd();
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

            
            player.GetComponent<PlayerMovement>().paused = false;

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
}
