using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Video;
using System.IO;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

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
    public GameObject exitBlowUpTrack;
    

    [Header("Camera references")]
    
    public CinemachineVirtualCamera guardCaughtIn4k;
    public CinemachineVirtualCamera explosionCam;
    public CinemachineVirtualCamera playerCam;
    
    public Camera mainCam;
    [SerializeField] private GameObject guardCam;


    [Header("CutScene object references")]
    public GameObject caughtTargetGroup;
    public GameObject sceneTransitionCanvas;
    public GameObject helicopterPrefab;
    public GameObject swatTeam1;
    public GameObject swatTeam2;
    public GameObject gameUIReference;
    public GameObject caughtPlayerObject;
    public GameObject preExplosionWall;
    public GameObject afterExplosionAsset;
    public GameObject C4;
    public GameObject explosionEffect;
    public GameObject skipCounterText;
    


    [Header("Flags and floats")]
    [Range(0.6f, 1.0f)]
    public float zoomMultiplier = 1.0f;
    public bool introDone = false;
    public bool isCutSceneHappening = true;
    public bool isCaughtCutSceneHappening = false;
    public bool isEndCutSceneHappening = false;
    public bool skippedOrNot = false;
    private bool playerCamActive = false;
    
    private GameObject guardShotReference;
    private GameObject thisPlayer;
    private GameObject securityCameraReference;
    private float startingHeight;
    private float startingDistance;
    public int skipCounter;
    public const byte skipCutSceneCounterCode = 1;
    private int cullingMask = 0;
    public bool disableCutScenes = false;

    [Header("Players")]
    public GameObject introPlayer;
    public GameObject swatPlayer;
    public GameObject explodeWallPlayer;

    public GameObject introRenderer;
    public GameObject swatRenderer;
    public GameObject explodeRenderer;

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
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        black = GameObject.Find("Black");
        thisPlayer = playerCam.Follow.gameObject;
        audioController = GameObject.FindObjectOfType<AudioController>();
        sceneTransitionCanvas.SetActive(true);

        startingHeight = playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;
        startingDistance = playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z;

        

        introPlayer.GetComponent<VideoPlayer>().url = Path.Combine(Application.streamingAssetsPath, "Intro.mp4");

        swatPlayer.GetComponent<VideoPlayer>().url = Path.Combine(Application.streamingAssetsPath, "Swat.mp4");


        explodeWallPlayer.GetComponent<VideoPlayer>().url = Path.Combine(Application.streamingAssetsPath, "Explode.mp4");


        gameUIReference.GetComponent<CanvasGroup>().alpha = 0;

        RenderTexture renderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.Create();

        introRenderer.GetComponent<RawImage>().texture = renderTexture;

        introPlayer.GetComponent<VideoPlayer>().targetTexture = renderTexture;

        introPlayer.GetComponent<VideoPlayer>().Prepare();


        cullingMask = mainCam.cullingMask;
        





    }

    // Update is called once per frame
    void Update()
    {
        
        if (guardShotReference != null && introPlayer.GetComponent<VideoPlayer>().isPrepared)
        {
            if (!start) {

                //setting up start cutScene
                introCutSceneSetup();

                


                start = true;
            }

           
            //for the first escape key press we want to end the cutscene and skip to the player cam
            if (!introDone && Input.GetKeyDown(KeyCode.Escape) && !skippedOrNot)
            {
                raiseEventSkipCounter();
                skippedOrNot = true;
                
                
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

            if (!guardShotReference)
            {
                guardShotReference = GameObject.Find("Guard3(Clone)");
            }
           
            
        }

        
        if(!introDone && PhotonNetwork.CurrentRoom != null && (skipCounter == PhotonNetwork.CurrentRoom.PlayerCount))
        {
            introEnd();
            introDone = true;
        }

        if(!introDone && (skipCounter>0))
        {
            skipCounterText.GetComponent<Text>().text = skipCounter + "/" + PhotonNetwork.CurrentRoom.PlayerCount;
        }

        if(Input.GetKeyDown(KeyCode.Comma))
        {
            disableCutScenes = true;
        }
        




    }
    public void raiseEventSkipCounter()
    {
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(skipCutSceneCounterCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == skipCutSceneCounterCode)
        {
            skipCounter++;
        }
    }

    
    public void introEnd()
    {
        //change layer of guard back to normal, fade ui back in, turn on black

        mainCam.cullingMask = -1;
        playerCamActive = true;
        mainCam.gameObject.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
        playerCamTrack.SetActive(true);


        introPlayer.GetComponent<VideoPlayer>().targetTexture.Release();
        introPlayer.SetActive(false);
        introSceneTrack.SetActive(false);

        black.SetActive(true);
        
        introRenderer.SetActive(false);

        StartCoroutine(disableAfterTime(playerCamTrack, 3f));
        mainCam.rect = new Rect(new Vector2(0, 0), new Vector2(1f, 1f));            
        thisPlayer.GetComponent<PlayerMovement>().paused = false;

        skipCounterText.GetComponent<Text>().text = "";




        isCutSceneHappening = false;

        SetLayerRecursively(guardShotReference, 10);
        SetPaintingsLayer(13);
        SetLayerRecursively(securityCameraReference, 10);

        
        

        GameController gameController = GameObject.FindObjectOfType<GameController>();
        gameController.gameStart();

        GameObject.FindObjectOfType<SoundController>().enableSound(true);
    }

    void introCutSceneSetup()
    {
        //Finding guard object for guard scene shot and setting the VC to follow and look at it
        guardCam.GetComponent<CinemachineVirtualCamera>().Follow = guardShotReference.transform;
        guardCam.GetComponent<CinemachineVirtualCamera>().LookAt = guardShotReference.transform;

        //find players and disable their control whilst cutscene plays
        mainCam.gameObject.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;
        thisPlayer.GetComponent<PlayerMovement>().paused = true;
        //sceneTransitionCanvas.SetActive(true);
        //find security cam
        securityCameraReference = GameObject.Find("Camera 2");
        
        //setting the layers for paintings and guard so they render
        SetPaintingsLayer(23);
        SetLayerRecursively(guardShotReference, default);
        SetLayerRecursively(securityCameraReference, default);

        gameUIReference.GetComponent<CanvasGroup>().alpha = 0;
        introRenderer.GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        


        black.SetActive(false);
        introSceneTrack.SetActive(true);
        mainCam.cullingMask = 0;
        introPlayer.GetComponent<VideoPlayer>().Play();
        
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

        GameObject.FindObjectOfType<SoundController>().enableSound(false);
        sceneTransitionCanvas.SetActive(true);
        
        
        playerCamFadeOutTrack.SetActive(true);
        
        RenderTexture renderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.Create();

        swatRenderer.GetComponent<RawImage>().texture = renderTexture;
        
        swatPlayer.GetComponent<VideoPlayer>().targetTexture = renderTexture;

        swatPlayer.GetComponent<VideoPlayer>().Prepare();

        yield return new WaitForSeconds(1f);
        
        black.SetActive(false);
        
        swatRenderer.GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        

        while (!swatPlayer.GetComponent<VideoPlayer>().isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
        }



        
        swatCamTrack.SetActive(true);
        playerCamFadeOutTrack.SetActive(false);
        gameUIReference.GetComponent<CanvasGroup>().alpha = 0;
        mainCam.cullingMask = 0;
        swatPlayer.GetComponent<VideoPlayer>().Play();
        
        
        
        
        

        //ending

        double swatTime = swatCamTrack.GetComponent<PlayableDirector>().duration;
        yield return new WaitForSeconds(8f);


        mainCam.cullingMask = -1;
        playerCamTrack.SetActive(true);
        
        swatCamTrack.SetActive(false);
        renderTexture.Release();
        swatRenderer.SetActive(false);
        black.SetActive(true);
        
        
        
        

        
        
        yield return new WaitForSeconds(2f);

        thisPlayer.GetComponent<PlayerMovement>().paused = false;
        GuardController.Instance.disableAllguards(false);




        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateRoomObject(swatTeam1.name, new Vector3(-28.7f, 13.56f, 20.6f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject(swatTeam1.name, new Vector3(-28.7f, 13.56f, 25f), Quaternion.identity).GetComponent<GuardMovement>().patrolPath.Reverse();
        }
        GameObject.FindObjectOfType<GuardController>().swat();
        GameObject.FindObjectOfType<SoundController>().enableSound(true);

        yield return new WaitForSeconds(1f);
        playerCamTrack.SetActive(false);
        sceneTransitionCanvas.SetActive(false);
    }
   


    private IEnumerator disableAfterTime(GameObject g, float time)
    {
        yield return new WaitForSeconds(time);
        
        
        
        g.SetActive(false);
        sceneTransitionCanvas.SetActive(false);
    }

    


    public void caughtCutScene(int guardViewID, int caughtPlayerID, string message)
    {
        GameObject.FindObjectOfType<SoundController>().enableSound(false);
        GameObject guard = PhotonView.Find(guardViewID).gameObject;



        caughtPlayerObject = PhotonView.Find(caughtPlayerID).gameObject;
        gameUIReference.GetComponent<CanvasGroup>().alpha = 0;
        caughtTargetGroup.GetComponent<CinemachineTargetGroup>().AddMember(guard.transform, 1.0f, 10.0f);
        caughtTargetGroup.GetComponent<CinemachineTargetGroup>().AddMember(caughtPlayerObject.transform, 1.0f, 10.0f);

        guardCaughtIn4k.Follow = caughtTargetGroup.transform;
        guardCaughtIn4k.LookAt = caughtTargetGroup.transform;

        guardCaughtIn4k.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.m_InputAxisValue = 360;

        

        guardCaughtIn4k.Priority = 11;

        audioController.PlayIntenseTheme();

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
        yield return new WaitForSeconds(3.5f);


        guardCaughtIn4k.Priority = 9;
        BarController.Instance.HideBars();

        yield return new WaitForSeconds(2f);
        SetLayerRecursively(guard, 10);
        thisPlayer.GetComponent<PlayerMovement>().paused = false;
        gameUIReference.GetComponent<CanvasGroup>().alpha = 1;

        GuardController.Instance.disableAllguards(false);
        GameObject.FindObjectOfType<SoundController>().enableSound(true);
        isCaughtCutSceneHappening = false;
    }


    public IEnumerator explodeExitCutScene()
    {

        GameObject.FindObjectOfType<SoundController>().enableSound(false);
        thisPlayer.GetComponent<PlayerMovement>().paused = true;
        GuardController.Instance.disableAllguards(true);
        sceneTransitionCanvas.SetActive(true);
        playerCamFadeOutTrack.SetActive(true);

        RenderTexture renderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.Create();

        explodeRenderer.GetComponent<RawImage>().texture = renderTexture;

        explodeWallPlayer.GetComponent<VideoPlayer>().targetTexture = renderTexture;

        explodeWallPlayer.GetComponent<VideoPlayer>().Prepare();

        while (!explodeWallPlayer.GetComponent<VideoPlayer>().isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1.5f);
        explodeRenderer.GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);

        black.SetActive(false);

        // Additional music plays here
        audioController.EnableAdditional();

        
        exitBlowUpTrack.SetActive(true);
        mainCam.cullingMask = 0;
        explodeWallPlayer.GetComponent<VideoPlayer>().Play();
        playerCamFadeOutTrack.SetActive(false);
        gameUIReference.GetComponent<CanvasGroup>().alpha = 0;


        //ending

        double trackTime = exitBlowUpTrack.GetComponent<PlayableDirector>().duration;

        yield return new WaitForSeconds(8f);
        mainCam.cullingMask = -1;
        playerCamTrack.SetActive(true);
        exitBlowUpTrack.SetActive(false);
        black.SetActive(true);
        renderTexture.Release();
        explodeRenderer.SetActive(false);
        

        

        yield return new WaitForSeconds(2.5f);
        playerCamTrack.SetActive(false);
        sceneTransitionCanvas.SetActive(false);
        thisPlayer.GetComponent<PlayerMovement>().paused = false;
        GameObject.FindObjectOfType<SoundController>().enableSound(true);
        GuardController.Instance.disableAllguards(false);

    }

    public void playExplosionEffect()
    {
        explosionEffect.SetActive(true);
        
    }

    public void changeWallsForExplosion()
    {
        preExplosionWall.SetActive(false);
        afterExplosionAsset.SetActive(true);
        C4.SetActive(false);
    }


    
   
}

