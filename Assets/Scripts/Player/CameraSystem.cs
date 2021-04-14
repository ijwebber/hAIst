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

            SetLayerRecursively(guardShotReference, 10);
            SetPaintingsLayer(13);
        }
    }

    void introCutSceneSetup()
    {
        //Finding guard object for guard scene shot and setting the VC to follow and look at it
        guardShotReference = GameObject.Find("Guard3(Clone)");
        introSceneTrack.gameObject.transform.Find("CM Guard Cam").gameObject.GetComponent<CinemachineVirtualCamera>().Follow = guardShotReference.transform;
        introSceneTrack.gameObject.transform.Find("CM Guard Cam").gameObject.GetComponent<CinemachineVirtualCamera>().LookAt = guardShotReference.transform;

        //setting the layers for paintings and guard so they render
        SetPaintingsLayer(default);
        SetLayerRecursively(guardShotReference, default);
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
