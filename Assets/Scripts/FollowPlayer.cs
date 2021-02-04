using UnityEngine;

public class FollowPlayer : MonoBehaviour
{   
    public Transform player;
    public Vector3 offset;

    public GameObject obstruction;  // global obstructions 

    public string name; // name of current object in view
    
    public Material objectMaterial;

    public Material prevObjectMaterial;

    public string[] currentObject = {""};  // at most, there will probably be two walls in this array so initialise for two strings, but for now just do 1


    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + offset;
        
        
        Ray ray = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        
        if (Physics.Raycast(ray, out hit))
        {
            print("I'm looking at " + hit.transform.name);

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
            print("I'm looking at nothing ");
            
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
