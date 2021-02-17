using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixImageTask : MonoBehaviour
{
    bool inRange = false;

    bool canvasActive = false;

    bool buttonPressed = false;

    public Camera mainCam;  // define camera object

    public GameObject pictureTask;
    private Inventory inventory;

    // This script is to be used for the "rotate" task and should be applied to the object that is to be stolen
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;  //link camera object to main camera (that follows the player)
        mainCam.GetComponent<FollowPlayer>().seconds = 0;

        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    
    }

    // Update is called once per frame
    void Update()
    {
        
        int timeLeft = mainCam.GetComponent<FollowPlayer>().seconds;  // access the seconds variable from the mainCam class/ follow player script

        if(Input.GetKey(KeyCode.E) && inRange && canvasActive == false && timeLeft == 0){
            pictureTask.SetActive(true);
            canvasActive = true;
            buttonPressed = true;
        } 
        
        if(canvasActive == true && inRange == false){
            pictureTask.SetActive(false);
            canvasActive = false;
            buttonPressed = false;
        }

        writeMessage(timeLeft);

        bool pictureCorrect = pictureTask.GetComponent<RotateTask>().win;

        if(pictureCorrect && inRange && timeLeft == 0 && !inventory.isFull()){ // if both player is in range and the button E is pressed, then add points to the score, move this to its own method
            mainCam.GetComponent<FollowPlayer>().targetTime += 11;  // increase time duration
            
            inventory.Add(gameObject);
            pictureTask.SetActive(false);
            pictureCorrect = false;
            pictureTask.GetComponent<RotateTask>().win = false;
            
        }    
    }

    void writeMessage(int timeLeft){
        if(inRange && buttonPressed && timeLeft == 0){mainCam.GetComponent<Message>().messageD = "";}
        else if(Input.GetKey(KeyCode.E) && inRange && timeLeft != 0){mainCam.GetComponent<Message>().messageD = "Wait for the Cooldown";}
        else if(inRange){mainCam.GetComponent<Message>().messageD = "Press E to pick up";}
    }

    private void OnTriggerEnter(Collider other) {       // if player enters the box collider of the object, do etc...
        if((other.name == "Timmy")){
            inRange = true;
        }
        
    }

    private void OnTriggerExit(Collider other) {        // if player exits the box collider of the object, do etc...
        if((other.name == "Timmy")){
            inRange = false;
            mainCam.GetComponent<Message>().messageD = "";
        }
    }
}
