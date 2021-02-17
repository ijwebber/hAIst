using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldDownTask : MonoBehaviour
{
    bool inRange = false;

    public Camera mainCam;  // define camera object


    private float startTime = 0f;
    private float timer = 0f;
    public float holdTime = 5.0f;
    private bool held = false;

    //public GameObject messageController;

    private Inventory inventory;

    //public GameObject newMessage;

    void Start()
    {
        mainCam = Camera.main;  //link camera object to main camera (that follows the player)
        mainCam.GetComponent<FollowPlayer>().seconds = 0;


        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        //newMessage = GameObject.FindGameObjectWithTag("displayMessage");
        
    }

    // Update is called once per frame
    void Update()
    {
        int timeLeft = mainCam.GetComponent<FollowPlayer>().seconds;  // access the seconds variable from the mainCam class/ follow player script

        if(Input.GetKeyDown(KeyCode.E)){    // if player is holding down E, start a timer
            startTime = Time.time;
            timer = startTime;
        }

        if(Input.GetKey(KeyCode.E) && held == false && timeLeft == 0 && !inventory.isFull()){   // for each time E is being held down, count/increment the timer and remove the onscreen text
            //message.SetActive(false);
            timer += Time.deltaTime;                   
            if(timer>(startTime + holdTime)){   // if the time reaches 5s, then set held to true, else set to false
                held = true;
            }
            else{
                held = false;
            }
        }

        writeMessage(timeLeft);
        
        if(Input.GetKey(KeyCode.E) == false){ // if key is not being pressed, just set held to false
            held = false;
        
        }

        if(held && inRange && timeLeft == 0 && !inventory.isFull()){ // if both player is in range and the button E is pressed for 5 secpmds, then add points to the score, move this to its own method
            inventory.Add(gameObject);
            mainCam.GetComponent<FollowPlayer>().targetTime += 11;  // increase time duration       
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
            mainCam.GetComponent<Message>().messageD = "";
        }
    }

    void writeMessage(int timeLeft){
        if(inRange && Input.GetKey(KeyCode.E) && timeLeft == 0){mainCam.GetComponent<Message>().messageD = "";}
        else if(Input.GetKey(KeyCode.E) && inRange && timeLeft != 0){mainCam.GetComponent<Message>().messageD = "Wait for the Cooldown";}
        else if(inRange){mainCam.GetComponent<Message>().messageD = "Hold E to pick up";}
    }
}
