using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class CameraControlPlayer : MonoBehaviourPun
{

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

                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {

                obstruction = hit.transform.gameObject;
                name = obstruction.name; // name of obstruction
                objectMaterial = obstruction.GetComponent<Renderer>().material; 
                
                if(obstruction.tag == "hideObject" &&  currentObject[0] == name){   // case where we are hidden behind a wall but we don't move to another wall
                    //obstruction.GetComponent<Renderer>().enabled = false;  
                    SetAlpha(0.5F);     
                }
                else if(obstruction.tag == "hideObject" &&  currentObject[0] == ""){ // case where we are hidden behind a wall but the array is "empty"
                    //obstruction.GetComponent<Renderer>().enabled = false;
                    SetAlpha(0.5F);  
                    currentObject[0] = name;
                }
                else if (obstruction.tag == "hideObject" &&  currentObject[0] != name){ // case where we are hidden behind a new wall to the one in the array
                    GameObject prev = GameObject.Find(currentObject[0]);   // change prev wall to visible
                    //prev.GetComponent<Renderer>().enabled = true; 
                    prevObjectMaterial = prev.GetComponent<Renderer>().material; 
                    SetAlpha1(1F); 
                    currentObject[0] = name; // replace with new name
                }
                else if (obstruction.name == "Timmy" && currentObject[0] != "") {  
                // set previous object back to visible, need to have a way to specifically change the visibility of the previous object
                    GameObject prev = GameObject.Find(currentObject[0]);   
                    //prev.GetComponent<Renderer>().enabled = true; 
                    prevObjectMaterial = prev.GetComponent<Renderer>().material; 
                    SetAlpha1(1F); 
                    currentObject[0] = "";
                }
            }





                Follow ();
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

}








