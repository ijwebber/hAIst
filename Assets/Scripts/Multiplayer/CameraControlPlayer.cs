using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class CameraControlPlayer : MonoBehaviourPunCallbacks
{

    public GameObject player;
    public GameObject obstruction;  // global obstructions 

    public string name; // name of current object in view
    
    public Material objectMaterial;

    public Material prevObjectMaterial;

    public string[] currentObject = {""}; 



    [SerializeField]
    private float distance = 0.0f;
        
    [SerializeField]
    private float height = 12.0f;
        

    [SerializeField]
    private bool followOnStart = false;


    // cached transform of the target
    Transform cameraTransform;

    // maintain a flag internally to reconnect if target is lost or camera is switched
    bool isFollowing;
    bool isCutScene = false;
    private Vector3 guardCamPos;
    private Vector3 spottingGuardLocation;
    bool cutSceneDone = false;
    bool obsecured = false;
    private float rotateCounter = 0;

    
        
    // Cache for camera offset
    Vector3 cameraOffset = Vector3.zero;



    // Start is called before the first frame update
    void Start()
        {
            // Start following the target if wanted.
            if (followOnStart)
            {
                OnStartFollowing();
            }
            player = GameObject.Find("Timmy");
            
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
                name = obstruction.name; // name of obstruction
                objectMaterial = obstruction.GetComponent<Renderer>().material; 
                
                if(obstruction.tag == "hideObject" &&  currentObject[0] == name){   // case where we are hidden behind a wall but we don't move to another wall
                    //obstruction.GetComponent<Renderer>().enabled = false;  
                    // SetAlpha(0.5F);     
                }
                else if(obstruction.tag == "hideObject" &&  currentObject[0] == ""){ // case where we are hidden behind a wall but the array is "empty"
                    //obstruction.GetComponent<Renderer>().enabled = false;
                    // SetAlpha(0.5F);  
                    currentObject[0] = name;
                }
                else if (obstruction.tag == "hideObject" &&  currentObject[0] != name){ // case where we are hidden behind a new wall to the one in the array
                    GameObject prev = GameObject.Find(currentObject[0]);   // change prev wall to visible
                    //prev.GetComponent<Renderer>().enabled = true; 
                    prevObjectMaterial = prev.GetComponent<Renderer>().material; 
                    // SetAlpha1(1F); 
                    currentObject[0] = name; // replace with new name
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

        if(isCutScene && !isFollowing)
        {
            if (BarController.Instance != null)
            {
                cutScene();
            } else
            {
                isCutScene = false;
                isFollowing = true;
            }
        }
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
                    spottingGuardLocation = (Vector3)setSpotted["spottingGuardLocation"];
                    guardCamPos = (Vector3)setSpotted["spottingGuardLocation"];
                    //guardCamPos.z = guardCamPos.z + distance-4;
                    guardCamPos.y = guardCamPos.y + height-5;
                    isFollowing = false;

                    //cameraTransform.Rotate(0, 30, 0, Space.World);
                    

                    


                }

            }
            
        }
    }


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


            if (Math.Abs(cameraTransform.position.x - playerCamPos.x) < 1 && Math.Abs(cameraTransform.position.z - playerCamPos.z) < 1)
            {
                
                Hashtable setSpotted = new Hashtable() { { "spotted", true }, { "spottingGuardLocation", null}, { "cutSceneDone", true } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(setSpotted);

                this.GetComponent<PlayerMovement>().paused = false;
                isCutScene = false;
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
    

}








