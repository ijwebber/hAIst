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

       
        guardShotReference = GameObject.Find("Guard3(Clone)");
        

        introSceneTrack.gameObject.transform.Find("CM Guard Cam").gameObject.GetComponent<CinemachineVirtualCamera>().Follow = guardShotReference.transform;
        introSceneTrack.gameObject.transform.Find("CM Guard Cam").gameObject.GetComponent<CinemachineVirtualCamera>().LookAt = guardShotReference.transform;
      


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
        }
    }
}
