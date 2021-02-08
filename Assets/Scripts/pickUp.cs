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
        mainCam.GetComponent<FollowPlayer>().seconds = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(inRange){print("INRANGE");}      // keep track of whether player is in range or not
        else{print("NOT IN RANGE");}

        int timeLeft = mainCam.GetComponent<FollowPlayer>().seconds;

        if(Input.GetKey(KeyCode.E) && inRange && timeLeft == 0){ // if both player is in range and the button E is pressed, then add points to the score
            mainCam.GetComponent<FollowPlayer>().points+=50;
            mainCam.GetComponent<FollowPlayer>().targetTime += 11;  // increase time duration
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
