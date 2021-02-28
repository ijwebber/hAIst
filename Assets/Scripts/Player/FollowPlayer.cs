using UnityEngine;

public class FollowPlayer : MonoBehaviour
{   
    public Transform player;
    public Vector3 offset;

    public GameObject obstruction;  // global obstructions 

    public string currentName; // name of current object in view
    
    public Material objectMaterial;

    public Material prevObjectMaterial;

    public int points = 0;

    public string[] currentObject = {""};  // at most, there will probably be two walls in this array so initialise for two strings, but for now just do 1

    public float targetTime = 0;    // increase the target time by a certain amount i.e. 10s

    public int seconds; // convert to seconds


    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + offset;

        if(targetTime != 0 && !(seconds < 0)){      // perhaps change this so from the pick up script, you set target time to 0 if the time left is 0/ seconds
            targetTime -= Time.deltaTime;           // in float
            seconds = (int)(targetTime % 60);       // convert from float, updating the seconds variable
        }                                           // this chunk of code can be moved to the HUD script
        else if(targetTime < 0 || seconds < 0){
            seconds = 0;
            targetTime = 0;
        }
        else{
            seconds = 0;
            targetTime = 0;
        }
        
        
        Ray ray = GetComponent<Camera>().ViewportPointToRay(player.transform.position);
        RaycastHit hit;

        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1<<11)))
        {
            obstruction = hit.transform.gameObject;
            currentName = obstruction.name; // name of obstruction
            objectMaterial = obstruction.GetComponent<Renderer>().material; 
            
            if(obstruction.tag == "hideObject" &&  currentObject[0] == currentName){   // case where we are hidden behind a wall but we don't move to another wall
                //obstruction.GetComponent<Renderer>().enabled = false;  
                SetAlpha(0.5F);     
            }
            else if(obstruction.tag == "hideObject" &&  currentObject[0] == ""){ // case where we are hidden behind a wall but the array is "empty"
                //obstruction.GetComponent<Renderer>().enabled = false;
                SetAlpha(0.5F);  
                currentObject[0] = currentName;
            }
            else if (obstruction.tag == "hideObject" &&  currentObject[0] != currentName){ // case where we are hidden behind a new wall to the one in the array
                GameObject prev = GameObject.Find(currentObject[0]);   // change prev wall to visible
                //prev.GetComponent<Renderer>().enabled = true; 
                prevObjectMaterial = prev.GetComponent<Renderer>().material; 
                SetAlpha1(1F); 
                currentObject[0] = currentName; // replace with new name
            }
            if (obstruction.name == "Timmy" && currentObject[0] != "") {  
                // set previous object back to visible, need to have a way to specifically change the visibility of the previous object
                GameObject prev = GameObject.Find(currentObject[0]);   
                //prev.GetComponent<Renderer>().enabled = true; 
                prevObjectMaterial = prev.GetComponent<Renderer>().material; 
                SetAlpha1(1F); 
                currentObject[0] = "";
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

    private void OnGUI(){
        if(seconds !=0){GUI.Label(new Rect(10,40,100,20),"Cool Down : " + seconds); }
    }
}
