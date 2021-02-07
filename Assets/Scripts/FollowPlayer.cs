using UnityEngine;
using Photon;
using Photon.Pun;


public class FollowPlayer : MonoBehaviourPun
{   
    public Transform player;
    public Vector3 offset;

    public GameObject obstruction;  // global obstructions 

    public string name; // name of current object in view
    
    public Material objectMaterial;

    public Material prevObjectMaterial;

    public string[] currentObject = {""};  // at most, there will probably be two walls in this array so initialise for two strings, but for now just do 1




 [Tooltip("The distance in the local x-z plane to the target")]
        [SerializeField]
        private float distance = 7.0f;
        
        [Tooltip("The height we want the camera to be above the target")]
        [SerializeField]
        private float height = 3.0f;
        
        [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
        [SerializeField]
        private Vector3 centerOffset = Vector3.zero;

        [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
        [SerializeField]
        private bool followOnStart = false;

        [Tooltip("The Smoothing for the camera to follow the target")]
        [SerializeField]
        private float smoothSpeed = 0.125f;

        // cached transform of the target
        Transform cameraTransform;

        // maintain a flag internally to reconnect if target is lost or camera is switched
        bool isFollowing;
        
        // Cache for camera offset
        Vector3 cameraOffset = Vector3.zero;





    void Start()
        {
            // Start following the target if wanted.
            if (followOnStart)
            {
                OnStartFollowing();
            }
            if (photonView.IsMine)
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
        if (isFollowing) {

            transform.position = player.position + offset;
            
            
            Ray ray = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;

            
            if (Physics.Raycast(ray, out hit))
            {
                //print("I'm looking at " + hit.transform.name);

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
            else
            {
                //print("I'm looking at nothing ");
                
            }
        }
        
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
