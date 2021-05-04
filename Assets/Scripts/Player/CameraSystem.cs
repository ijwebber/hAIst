using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class CameraSystem : MonoBehaviour
{
    // Start is called before the first frame update

    public static CameraSystem Instance { get; private set; }

    public CinemachineBrain brain;

    [Header("Tracks")]
    public GameObject introSceneTrack;
    public GameObject playerCamTrack;
    public GameObject swatCamTrack;
    public GameObject playerCamFadeOutTrack;

    [Header("Camera references")]
    
    public CinemachineVirtualCamera guardCaughtIn4k;
    public CinemachineVirtualCamera playerCam;
    [SerializeField] private GameObject guardCam;


    [Header("CutScene object references")]
    public GameObject caughtTargetGroup;
    public GameObject sceneTransitionCanvas;
    public GameObject helicopterPrefab;
    public GameObject swatTeam1;
    public GameObject swatTeam2;
    public GameObject gameUIReference;
    public GameObject caughtPlayerObject;

    [Header("Flags and floats")]
    [Range(0.6f, 1.0f)]
    public float zoomMultiplier = 1.0f;
    public bool introDone = false;
    public bool isCutSceneHappening = true;
    public bool isCaughtCutSceneHappening = false;
    private bool playerCamActive = false;
    
    private GameObject guardShotReference;
    private GameObject thisPlayer;
    private GameObject securityCameraReference;
    private float startingHeight;
    private float startingDistance;
    
    

    [Header("Other Stuff")]
    [SerializeField] private AudioController audioController;
    
    private GameObject black;
    private bool start = false;
    


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
        audioController = GameObject.FindObjectOfType<AudioController>();
        

        startingHeight = playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;
        startingDistance = playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z;
        
    }

    // Update is called once per frame
    void Update()
    {   
        if (guardShotReference != null)
        {
            if (!start) {

                //setting up start cutScene
                introCutSceneSetup();

                


                start = true;
            }

           
            //for the first escape key press we want to end the cutscene and skip to the player cam
            if (!introDone && Input.GetKeyDown(KeyCode.Escape))
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
        } else {
            gameUIReference.GetComponent<CanvasGroup>().alpha = 0;
            guardShotReference = GameObject.Find("Guard3(Clone)");
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
        guardCam.GetComponent<CinemachineVirtualCamera>().Follow = guardShotReference.transform;
        guardCam.GetComponent<CinemachineVirtualCamera>().LookAt = guardShotReference.transform;
    
        //find players and disable their control whilst cutscene plays
        
        thisPlayer.GetComponent<PlayerMovement>().paused = true;
        sceneTransitionCanvas.SetActive(true);
        //find security cam
        securityCameraReference = GameObject.Find("Camera 2");

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

    public IEnumerator playSwatScene(){
        thisPlayer.GetComponent<PlayerMovement>().paused = true;
        GuardController.Instance.disableAllguards(true);
        playerCamFadeOutTrack.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        
        swatCamTrack.SetActive(true);
        playerCamFadeOutTrack.SetActive(false);
        gameUIReference.GetComponent<CanvasGroup>().alpha = 0;
        
        
        BarController.Instance.SetText("Backup has arrived");
        BarController.Instance.ShowBars();
        
        

        //ending

        double swatTime = swatCamTrack.GetComponent<PlayableDirector>().duration;

        yield return new WaitForSeconds((float)swatTime);
        swatCamTrack.SetActive(false);
        yield return new WaitForSeconds(2f);

        thisPlayer.GetComponent<PlayerMovement>().paused = false;
        GuardController.Instance.disableAllguards(false);
        gameUIReference.GetComponent<CanvasGroup>().alpha = 1;
        BarController.Instance.HideBars();

        PhotonNetwork.InstantiateRoomObject(swatTeam1.name, new Vector3(-28.7f, 13.56f, 20.6f), Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject(swatTeam1.name, new Vector3(-28.7f, 13.56f, 25f), Quaternion.identity).GetComponent<GuardMovement>().patrolPath.Reverse();

        
    }
   


    private IEnumerator disableAfterTime(GameObject g, float time)
    {
        yield return new WaitForSeconds(time);
        g.SetActive(false);
    }

    


    public void caughtCutScene(int guardViewID, int caughtPlayerID, string message)
    {
        
        GameObject guard = PhotonView.Find(guardViewID).gameObject;



        caughtPlayerObject = PhotonView.Find(caughtPlayerID).gameObject;
        gameUIReference.GetComponent<CanvasGroup>().alpha = 0;
        caughtTargetGroup.GetComponent<CinemachineTargetGroup>().AddMember(guard.transform, 1.0f, 10.0f);
        caughtTargetGroup.GetComponent<CinemachineTargetGroup>().AddMember(caughtPlayerObject.transform, 1.0f, 10.0f);

        guardCaughtIn4k.Follow = caughtTargetGroup.transform;
        guardCaughtIn4k.LookAt = caughtTargetGroup.transform;

        

        guardCaughtIn4k.Priority = 11;

        //audioController.PlayIntenseTheme();

        SetLayerRecursively(guard, default);
        BarController.Instance.SetText(message);
        BarController.Instance.ShowBars();
        
        isCaughtCutSceneHappening = true;

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

        isCaughtCutSceneHappening = false;
    }



    
   
}

