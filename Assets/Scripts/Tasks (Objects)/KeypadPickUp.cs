using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class KeypadPickUp : MonoBehaviour
{
    bool inRange = false;

    bool canvasActive = false;


    public GameObject cooldown;
    public GameObject keycodeTask;
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        int timeLeft = cooldown.GetComponent<CooldownScript>().seconds;  // access the seconds variable from the mainCam class/ follow player script

        if(Input.GetKey(KeyCode.E) && inRange && canvasActive == false && timeLeft == 0){
            keycodeTask.SetActive(true);
            canvasActive = true;
        } 
        
        if(canvasActive == true && inRange == false){   // check if canvas is true but player is out of range, then disable the canvas
            keycodeTask.SetActive(false);
            canvasActive = false;
        }

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
