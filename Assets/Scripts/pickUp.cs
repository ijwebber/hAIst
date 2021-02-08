using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class pickUp : MonoBehaviour
{
    bool inRange = false;

    public Camera mainCam;  // define camera object

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;  //link camera object to main camera (that follows the player)
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(inRange){print("INRANGE");}      // keep track of whether player is in range or not
        else{print("NOT IN RANGE");}

        if(Input.GetKey(KeyCode.E) && inRange){ // if both player is in range and the button E is pressed, then add points to the score
            mainCam.GetComponent<FollowPlayer>().points+=50; 
            Destroy(gameObject);
        }
        
        
    }

    private void OnTriggerEnter(Collider other) {       // if player enters the box collider of the object, do etc...
        if((other.name == "Timmy")){
            inRange = true;
        }
        
    }

    private void OnTriggerExit(Collider other) {        // if player exits the box collider of the object, do etc...
        if((other.name == "Timmy")){
            inRange = false;
        }
    }
}
