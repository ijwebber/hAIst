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

    public bool introDone = false;
    private bool playerCamActive = false;

    void Start()
    {
        
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
        if (!introDone)
        {
            introDone = true;
            playerCamActive = true;

            playerCamTrack.SetActive(true);
            introSceneTrack.SetActive(false);
        }
    }
}
