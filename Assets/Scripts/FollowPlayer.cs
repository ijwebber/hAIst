using UnityEngine;

public class FollowPlayer : MonoBehaviour
{   
    public Transform player;
    public Vector3 offset;

    public GameObject obstruction;  // global obstructions 

    public string name;
    

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
            

            if(obstruction.tag == "hideObject"){
                
                //objectHit.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                name = obstruction.name; // change name to previous wall 
            
                obstruction.GetComponent<Renderer>().enabled = false;
            }


            else if (obstruction.name == "Cube") {  
            // set previous object back to visible, need to have a way to specifically change the visibility of the previous object

                GameObject prev = GameObject.Find("wall-corner (9)");   // needs to be changed
                prev.GetComponent<Renderer>().enabled = true; 
                

            }
        }


        else
        {
            print("I'm looking at nothing ");
            
        }
        
    }
}
