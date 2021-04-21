using System.Collections;
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
    public GameObject caughtTargetGroup;
    public GameObject sceneTransitionCanvas;
    

    [Range(0.6f, 1.0f)]
    public float zoomMultiplier = 1.0f;
    public GameObject gameUIReference;
    public bool introDone = false;
    public bool isCutSceneHappening = true;
    private bool playerCamActive = false;
    
    private GameObject guardShotReference;
    private GameObject thisPlayer;
    private GameObject securityCameraReference;
    private float startingHeight;
    private float startingDistance;

    [SerializeField] private AudioController audioController;
    [SerializeField] private GameObject guardCam;

    private GameObject black;


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
        black = GameObject.Find("Black");
        thisPlayer = playerCam.Follow.gameObject;

        startingHeight = playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;
        startingDistance = playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z;
        introCutSceneSetup();
        
        audioController = GameObject.FindObjectOfType<AudioController>();
    }

    // Update is called once per frame
    void Update()
    {   
        //for the first escape key press we want to end the cutscene and skip to the player cam
        if(!introDone && Input.GetKeyDown(KeyCode.Escape))
        {
            introEnd();
        }

        //zooming functionality
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
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

            
            thisPlayer.GetComponent<PlayerMovement>().paused = false;

            isCutSceneHappening = false;

            SetLayerRecursively(guardShotReference, 10);
            SetPaintingsLayer(13);
            SetLayerRecursively(securityCameraReference, 10);

            black.SetActive(true);
        }
    }

    void introCutSceneSetup()
    {
        //Finding guard object for guard scene shot and setting the VC to follow and look at it
        guardShotReference = GameObject.Find("Guard3(Clone)");
        Debug.Log("***");
        guardCam.GetComponent<CinemachineVirtualCamera>().Follow = guardShotReference.transform;
        guardCam.GetComponent<CinemachineVirtualCamera>().LookAt = guardShotReference.transform;

        //find players and disable their control whilst cutscene plays
        
        thisPlayer.GetComponent<PlayerMovement>().paused = true;
        sceneTransitionCanvas.SetActive(true);
        //find security cam
        securityCameraReference = GameObject.Find("Camera 1");

        //setting the layers for paintings and guard so they render
        SetPaintingsLayer(default);
        SetLayerRecursively(guardShotReference, default);
        SetLayerRecursively(securityCameraReference, default);

        gameUIReference.GetComponent<CanvasGroup>().alpha = 0;

        black.SetActive(false);
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


    public void caughtCutScene(int guardViewID, int caughtPlayerID, string message)
    {
        isCutSceneHappening = true;
        GameObject guard = PhotonView.Find(guardViewID).gameObject;




        gameUIReference.GetComponent<CanvasGroup>().alpha = 0;
        caughtTargetGroup.GetComponent<CinemachineTargetGroup>().AddMember(guard.transform, 1.0f, 10.0f);
        caughtTargetGroup.GetComponent<CinemachineTargetGroup>().AddMember(PhotonView.Find(caughtPlayerID).gameObject.transform, 1.0f, 10.0f);

        guardCaughtIn4k.Follow = caughtTargetGroup.transform;
        guardCaughtIn4k.LookAt = caughtTargetGroup.transform;

        guardCaughtIn4k.Priority = 11;

        audioController.PlayIntenseTheme();

        SetLayerRecursively(guard, default);
        BarController.Instance.SetText(message);
        BarController.Instance.ShowBars();

        StartCoroutine(endCaughtCutScene(guard));

        
        
        



    }

    // public static CinemachineBrain.BrainEvent CameraUpdatedEvent() {

    // }

    private IEnumerator endCaughtCutScene(GameObject guard)
    {
        yield return new WaitForSeconds(3f);


        guardCaughtIn4k.Priority = 9;
        BarController.Instance.HideBars();

        yield return new WaitForSeconds(2f);
        SetLayerRecursively(guard, 10);
        thisPlayer.GetComponent<PlayerMovement>().paused = false;
        gameUIReference.GetComponent<CanvasGroup>().alpha = 1;

        GuardController.Instance.disableAllguards(false);

        isCutSceneHappening = false;
    }


    

}

