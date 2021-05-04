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
    private Mesh viewMesh;
    private MeshFilter viewMeshFilter, objectMeshFilter;
    private PlayerController playerController;


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
        playerController = GameObject.FindObjectOfType<PlayerController>();

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

                //setting up mesh for caught cutscene
                if (SceneManager.GetActiveScene().name == "BuildScene" || SceneManager.GetActiveScene().name == "ArtLevel")
                {
                    MeshFilter ViewFilter = GameObject.FindGameObjectWithTag("POVObjectsCutScene").GetComponent<MeshFilter>();
                    MeshFilter ViewFilter3 = GameObject.FindGameObjectWithTag("POVGuardsCutScene").GetComponent<MeshFilter>();

                    // viewMeshFilter = ViewFilter;
                    viewMeshFilter = ViewFilter3;
                    objectMeshFilter = ViewFilter;
                    viewMesh = new Mesh();
                    viewMesh.name = "POV mesh";
                    viewMeshFilter.mesh = viewMesh;
                    objectMeshFilter.mesh = viewMesh;
                    
                }


                start = true;
            }

            if (isCaughtCutSceneHappening)
            {

                viewMeshFilter.transform.position = new Vector3(guardCaughtIn4k.LookAt.position.x, 16.5f, guardCaughtIn4k.LookAt.position.z);
                viewMeshFilter.transform.rotation = thisPlayer.transform.rotation;
                objectMeshFilter.transform.position = new Vector3(guardCaughtIn4k.LookAt.position.x, 16.5f, guardCaughtIn4k.LookAt.position.z);
                objectMeshFilter.transform.rotation = thisPlayer.transform.rotation;
                DrawFOV();
            }
            else
            {
                viewMeshFilter.mesh.Clear();
                objectMeshFilter.mesh.Clear();
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




        gameUIReference.GetComponent<CanvasGroup>().alpha = 0;
        caughtTargetGroup.GetComponent<CinemachineTargetGroup>().AddMember(guard.transform, 1.0f, 10.0f);
        caughtTargetGroup.GetComponent<CinemachineTargetGroup>().AddMember(PhotonView.Find(caughtPlayerID).gameObject.transform, 1.0f, 10.0f);

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



    
    void DrawFOV() {

        float meshResolution = 1f;

        int stepCount = Mathf.RoundToInt(360 * meshResolution);
        if (stepCount == 0) {
            stepCount = 360;
        }
        float stepAngleSize = 360 / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = thisPlayer.transform.eulerAngles.y - 360/2 + stepAngleSize*i;
            ViewCastInfo newViewCast = viewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count+1;
        Vector3[] vertices = new Vector3[vertexCount];
        int [] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount-1; i++)
        {
            vertices[i+1] = thisPlayer.transform.InverseTransformPoint(viewPoints[i]);

            if(i < vertexCount - 2){
                triangles[i*3] = 0;
                triangles[i*3+1] = i+1;
                triangles[i*3+2] = i+2;
            }

        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    
    ViewCastInfo viewCast(float globalAngle) {
        Vector3  dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        float viewRadius = 15f;

        if (Physics.Raycast(guardCaughtIn4k.LookAt.position, dir, out hit, viewRadius, playerController.ObMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        } else {
            return new ViewCastInfo(false, guardCaughtIn4k.LookAt.position + dir * viewRadius, viewRadius, globalAngle);
        }

    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += thisPlayer.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }
}

