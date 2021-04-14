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

        SetPaintingsLayer(default);
        guardShotReference = GameObject.Find("Guard3(Clone)");
        

        introSceneTrack.gameObject.transform.Find("CM Guard Cam").gameObject.GetComponent<CinemachineVirtualCamera>().Follow = guardShotReference.transform;
        introSceneTrack.gameObject.transform.Find("CM Guard Cam").gameObject.GetComponent<CinemachineVirtualCamera>().LookAt = guardShotReference.transform;

        SetLayerRecursively(guardShotReference, default);
      
    }

    // Update is called once per frame
    void Update()
    {
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
