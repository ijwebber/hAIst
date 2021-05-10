using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using UnityEngine.SceneManagement;

public class CameraControlPlayer : MonoBehaviourPunCallbacks
{

    public GameObject player;
    public GameObject obstruction;  // global obstructions 

    public string objInView; // name of current object in view
    
    public Material objectMaterial;

    public Material prevObjectMaterial;

    public string[] currentObject = {""};

    public AudioSource audioSource;



    [SerializeField]
    private float distance = 0.0f;
        
    [SerializeField]
    private float height = 12.0f;
        

    [SerializeField]
    private bool followOnStart = false;


    // cached transform of the target
    Transform cameraTransform;

    // maintain a flag internally to reconnect if target is lost or camera is switched
    public bool isFollowing;
    bool isCutScene = false;
    private Vector3 guardCamPos;
    private Vector3 spottingGuardLocation;
    bool cutSceneDone = false;
    bool obsecured = false;
    private float rotateCounter = 0;
    private MeshFilter viewMeshFilter, objectMeshFilter;
    private float viewRadius;
    private bool cutTime = false;
    private Mesh viewMesh;
    private bool start = false;
    public PlayerController playerController;
    public Vector3 cutscenePosition;

    
        
    // Cache for camera offset
    Vector3 cameraOffset = Vector3.zero;

    public float meshResolution = 1;

    AudioController audioController;
    /*
    void DrawFOV() {
        int stepCount = Mathf.RoundToInt(360 * meshResolution);
        if (stepCount == 0) {
            stepCount = 360;
        }
        float stepAngleSize = 360 / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = player.transform.eulerAngles.y - 360/2 + stepAngleSize*i;
            ViewCastInfo newViewCast = viewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count+1;
        Vector3[] vertices = new Vector3[vertexCount];
        int [] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount-1; i++)
        {
            vertices[i+1] = player.transform.InverseTransformPoint(viewPoints[i]);

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
    }*/

    /*
    ViewCastInfo viewCast(float globalAngle) {
        Vector3  dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(cutscenePosition, dir, out hit, viewRadius, playerController.ObMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        } else {
            return new ViewCastInfo(false, cutscenePosition + dir * viewRadius, viewRadius, globalAngle);
        }

    }*/

    //takes in an angle and gives its direction 
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += player.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }
    // Start is called before the first frame update
    void Start()
        {
            // Start following the target if wanted.
            if (followOnStart)
            {
                OnStartFollowing();
            }
            player = GameObject.Find("Timmy");
            playerController = GameObject.FindObjectOfType<PlayerController>();
            audioController = GameObject.FindObjectOfType<AudioController>();
            
        }


    public void OnStartFollowing()
        {         
            cameraTransform = Camera.main.transform;
            isFollowing = true;
            // we don't smooth anything, we go straight to the right camera shot
            //Cut();
        }

    // Update is called once per frame
    void Update()
    {   
        /*
        if (SceneManager.GetActiveScene().name == "BuildScene" || SceneManager.GetActiveScene().name == "ArtLevel" && !start) {
            MeshFilter ViewFilter = GameObject.FindGameObjectWithTag("POVObjectsCutScene").GetComponent<MeshFilter>();
            MeshFilter ViewFilter3 = GameObject.FindGameObjectWithTag("POVGuardsCutScene").GetComponent<MeshFilter>();

            // viewMeshFilter = ViewFilter;
            viewMeshFilter = ViewFilter3;
            objectMeshFilter = ViewFilter;
            viewMesh = new Mesh();
            viewMesh.name = "POV mesh";
            viewMeshFilter.mesh = viewMesh;
            objectMeshFilter.mesh = viewMesh;
            start = true;
        }
        if (start && cutTime) {
            viewMeshFilter.transform.position = new Vector3(cutscenePosition.x, 16.5f, cutscenePosition.z);
            viewMeshFilter.transform.rotation = player.transform.rotation;
            objectMeshFilter.transform.position = new Vector3(cutscenePosition.x, 16.5f, cutscenePosition.z);
            objectMeshFilter.transform.rotation = player.transform.rotation;
            DrawFOV();
        } else {
            viewMeshFilter.mesh.Clear();
            objectMeshFilter.mesh.Clear();
        }
        */
        if (cameraTransform == null && isFollowing)
            {
                OnStartFollowing();
            }

            // only follow is explicitly declared
            if (isFollowing) {

                // Ray ray = Camera.main.ViewportPointToRay(player.transform.position);
                // Ray ray = Physics.Linecast(player.transform.position, cameraTransform.position,(1<<))
                RaycastHit hit;

                if (Physics.Linecast(player.transform.position, cameraTransform.position, out hit,(1<<8),QueryTriggerInteraction.UseGlobal))
                {

                obstruction = hit.transform.gameObject;
                objInView = obstruction.name; // name of obstruction
                objectMaterial = obstruction.GetComponent<Renderer>().material; 
                
                if(obstruction.tag == "hideObject" &&  currentObject[0] == objInView){   // case where we are hidden behind a wall but we don't move to another wall
                    //obstruction.GetComponent<Renderer>().enabled = false;  
                    // SetAlpha(0.5F);     
                }
                else if(obstruction.tag == "hideObject" &&  currentObject[0] == ""){ // case where we are hidden behind a wall but the array is "empty"
                    //obstruction.GetComponent<Renderer>().enabled = false;
                    // SetAlpha(0.5F);  
                    currentObject[0] = objInView;
                }
                else if (obstruction.tag == "hideObject" &&  currentObject[0] != objInView){ // case where we are hidden behind a new wall to the one in the array
                    GameObject prev = GameObject.Find(currentObject[0]);   // change prev wall to visible
                    //prev.GetComponent<Renderer>().enabled = true; 
                    prevObjectMaterial = prev.GetComponent<Renderer>().material; 
                    // SetAlpha1(1F); 
                    currentObject[0] = objInView; // replace with new name
                }
                if (obstruction.name == "Timmy" && currentObject[0] != "") {  
                    Debug.Log(currentObject[0]);
                    // set previous object back to visible, need to have a way to specifically change the visibility of the previous object
                    GameObject prev = GameObject.Find(currentObject[0]);   
                    //prev.GetComponent<Renderer>().enabled = true; 
                    prevObjectMaterial = prev.GetComponent<Renderer>().material; 
                    // SetAlpha1(1F); 
                    currentObject[0] = "";
                }
            }





                Follow ();
            }
            /*
        if(isCutScene && !isFollowing)
        {
            if (BarController.Instance != null)
            {
                cutScene();
            } else
            {
                isCutScene = false;
                cutTime = false;
                isFollowing = true;
            }
        }*/
    }

    void Follow() 
    {
        cameraOffset.z = distance;
        cameraOffset.y = height;
        cameraTransform.position = this.transform.position + cameraOffset;
       
    }

    void SetAlpha(float alpha){            // need to merge these together by creating maybe just one object material class?
        Color color = objectMaterial.color;
        color.a = alpha;
        objectMaterial.color = color;
    }

    void SetAlpha1(float alpha){
        Color color = prevObjectMaterial.color;
        color.a = alpha;
        prevObjectMaterial.color = color;
    }


    //Recieves CutScene data
    [PunRPC]
    void RpcCutScene(int guardPhotonID, int caughtPlayerPhotonID, string customMessage, int code)
    {   
        
        
        //stop camera following player
        //isFollowing = false;

     
        //freeze player
        this.GetComponent<PlayerMovement>().paused = true;

       
        //freeze guards, will only work if player is master
        GuardController.Instance.disableAllguards(true);

        CameraSystem.Instance.caughtCutScene(guardPhotonID, caughtPlayerPhotonID, "The Police have been alerted");

        //this starts the incremental camera updates to the desired location (the cutscene)
        //StartCoroutine(cameraCutSceneUpdates(0.03f, location, distanceOffset, heightOffset, cameraRotation, customMessage, code));

    }

    [PunRPC]
    void updateSkipCounter(bool skip)
    {
        CameraSystem.Instance.skipCounter++;
    }

    //This calls the incremental update function, 0.03 works for 30 frames per second
    IEnumerator cameraCutSceneUpdates(float delay, object location, int distanceOffset, int heightOffset, object cameraRotation, string customMessage, int code)
    {
        

        while (!isFollowing)
        {
            yield return new WaitForSeconds(delay);
            cutSceneUpdate((Vector3)location, distanceOffset, heightOffset, (Vector3)cameraRotation, customMessage, code);
        }

        cutSceneDone = false;
    }

    //Updates camera positioning and rotation incrementally, the location and cameraRotation parameters will be the end location/rotation for the camera.
    void cutSceneUpdate(Vector3 location, int distanceOffset, int heightOffset, Vector3 cameraRotation, string customMessage, int code)
    {
        //We show the cutScene Bars and set the text to the desired message
        BarController.Instance.ShowBars();
        BarController.Instance.SetText(customMessage);


        //if the camera has reached the desired location, we set the cutSceneDone flag to true and start bringing the camera back to the original position
        if (Math.Abs(cameraTransform.position.x - location.x) < 1.5 && Math.Abs(cameraTransform.position.z - (location.z + distanceOffset)) < 1.5 && !cutSceneDone)
        {
            cutSceneDone = true;
        }
        else if (cutSceneDone)
        {
            //player cam info, we will bring camera back to here
            Vector3 playerCamPos = new Vector3(this.transform.position.x, this.transform.position.y + height, this.transform.position.z + distance);
            

            //update using linear interpolation between current cam pos and player cam pos.
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, playerCamPos, Time.deltaTime * 2);

            //updating rotation back to old one (53.883, 0, 0)
            float angleX = Mathf.LerpAngle(52.883f, cameraRotation.x, rotateCounter);
            float angleY = Mathf.LerpAngle(0f, cameraRotation.y, rotateCounter);
            float angleZ = Mathf.LerpAngle(0f, cameraRotation.z, rotateCounter);
            cameraTransform.eulerAngles = new Vector3(angleX, angleY, angleZ);
            //counter decreases
            rotateCounter -= 0.1f;


            //if old cam location and rotation have been reached end the cutScene
            if (Math.Abs(cameraTransform.position.x - playerCamPos.x) < 1 && Math.Abs(cameraTransform.position.z - playerCamPos.z) < 1 && cameraTransform.rotation.x < 54)
            {
                //unpause player, turn on cam following player, unpause guards and hide blackbars

                this.GetComponent<PlayerMovement>().paused = false;
                isFollowing = true;
                GuardController.Instance.disableAllguards(false);
                BarController.Instance.HideBars();


            }

        }
        else
        {
            //update using linear interpolation between current cam pos and the desired end location for the scene
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, new Vector3(location.x, location.y+heightOffset, location.z+distanceOffset), Time.deltaTime * 1);

            //updating rotation to desired rotation, counter increases
            float angleX = Mathf.LerpAngle(52.883f, cameraRotation.x, rotateCounter);
            float angleY = Mathf.LerpAngle(0f, cameraRotation.y, rotateCounter);
            float angleZ = Mathf.LerpAngle(0f, cameraRotation.z, rotateCounter);
            cameraTransform.eulerAngles = new Vector3(angleX, angleY, angleZ);
            //counter increases
            rotateCounter += 0.05f;
        }
    }



    /*
    public override void OnRoomPropertiesUpdate(Hashtable setSpotted)
    {
        base.OnRoomPropertiesUpdate(setSpotted);

        if(setSpotted["spotted"] != null)
        {
            if ((bool)setSpotted["spotted"] && !(bool)setSpotted["cutSceneDone"])
            {
                if (this.photonView.IsMine)
                {   
                    //freeze local player so they can't move during cutscene
                    this.GetComponent<PlayerMovement>().paused = true;

                    //start cutScene corountine
                    isCutScene = true;
                    this.GetComponent<PhotonView>().RPC("setCut", RpcTarget.All);
                    spottingGuardLocation = (Vector3)setSpotted["spottingGuardLocation"];
                    cutscenePosition = spottingGuardLocation;
                    guardCamPos = (Vector3)setSpotted["spottingGuardLocation"];
                    guardCamPos.z = guardCamPos.z - 4;
                    guardCamPos.y = guardCamPos.y + height-4;
                    isFollowing = false;

                    //cameraTransform.Rotate(0, 30, 0, Space.World);
                    
                    // Play the intense music track
                    audioController.PlayIntenseTheme();
                }
            }
            
        }
    }
    
    [PunRPC]
    void setCut() {
        viewRadius = 15;
        cutTime = true;
    }*/





    /*
    public void cutScene()
    {

        //once corountine is finished free local player, bring camera back and free local player
        //if master client set cutscene to finished and free guards.


        BarController.Instance.ShowBars();

        Vector3 playerCamPos = this.transform.position;
        playerCamPos.z = playerCamPos.z + distance;
        playerCamPos.y = playerCamPos.y + height;


        

        
        if (Math.Abs(cameraTransform.position.x - guardCamPos.x) < 1.5 && Math.Abs(cameraTransform.position.z - guardCamPos.z) < 1.5 && !cutSceneDone)
        {
            cutSceneDone = true;
        }
        else if (cutSceneDone)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, playerCamPos, Time.deltaTime * 2);

            float angle = Mathf.LerpAngle(52.883f, 75, rotateCounter);
            cameraTransform.eulerAngles = new Vector3(angle, 0, 0);
            rotateCounter -= 0.1f;


            if (Math.Abs(cameraTransform.position.x - playerCamPos.x) < 1 && Math.Abs(cameraTransform.position.z - playerCamPos.z) < 1 && cameraTransform.rotation.x < 54)
            {
                
                Hashtable setSpotted = new Hashtable() { { "spotted", true }, { "spottingGuardLocation", null}, { "cutSceneDone", true } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(setSpotted);

                this.GetComponent<PlayerMovement>().paused = false;
                isCutScene = false;
                cutTime = false;
                viewRadius = 0;
                isFollowing = true;
                //renable guards
                GuardController.Instance.disableAllguards(false);
                BarController.Instance.HideBars();

               
            }

        }
        else
        {
            
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, guardCamPos, Time.deltaTime * 1);

            float angle = Mathf.LerpAngle(52.883f, 75, rotateCounter);
            cameraTransform.eulerAngles = new Vector3(angle, 0, 0);
            rotateCounter += 0.05f;
        }
    }
    public struct ViewCastInfo {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle) {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }*/
}