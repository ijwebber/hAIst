using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class KeypadPickUp : MonoBehaviour
{
    bool inRange = false;
    bool canvasActive = false;

    public Camera mainCam;  // define camera object

    public GameObject cooldown;
    public GameObject keycodeTask;
    
    public bool buttonPressed = false;
    private Inventory inventory;

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
        int timeLeft = cooldown.GetComponent<CooldownScript>().seconds;  // access the seconds variable from the mainCam class/ follow player script

        if(Input.GetKey(KeyCode.E) && inRange && canvasActive == false && timeLeft == 0){
            keycodeTask.SetActive(true);
            buttonPressed = true;
            canvasActive = true;
        } 
        
        if(canvasActive == true && inRange == false){   // check if canvas is true but player is out of range, then disable the canvas
            keycodeTask.SetActive(false);
            canvasActive = false;
            buttonPressed = false;
        }

        writeMessage(timeLeft);

        bool keyCorrect = keycodeTask.GetComponent<KeycodeTask>().codeCorrect;  // boolean which just checks if the code is correct



        if(keyCorrect && inRange && timeLeft == 0){ // if both player is in range and the button E is pressed, then add points to the score, move this to its own method         
            cooldown.GetComponent<CooldownScript>().targetTime += 11;  // increase time duration
           
            //inventory.Add(gameObject); // add object to inventory
            keycodeTask.SetActive(false);
            keyCorrect = false;
            keycodeTask.GetComponent<KeycodeTask>().codeCorrect = false;
            
            gameObject.GetComponent<PhotonView>().RPC("destroyObject",RpcTarget.All);

            //photonView.RPC("destroyObject")
            //Destroy(gameObject);
        }    
    }

    [PunRPC]
    void destroyObject(){
        Destroy(gameObject);
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
