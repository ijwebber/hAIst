using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HoldDownTask : MonoBehaviour
{
    bool inRange = false;

    public GameObject cooldown;
    private float startTime = 0f;
    private float timer = 0f;
    public float holdTime = 5.0f;
    private bool held = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int timeLeft = cooldown.GetComponent<CooldownScript>().seconds;  // access the seconds variable from the mainCam class/ follow player script

        if(Input.GetKeyDown(KeyCode.E)){    // if player is holding down E, start a timer
            startTime = Time.time;
            timer = startTime;
        }

        if(Input.GetKey(KeyCode.E) && held == false && timeLeft == 0){   // for each time E is being held down, count/increment the timer and remove the onscreen text
            //message.SetActive(false);
            timer += Time.deltaTime;                   
            if(timer>(startTime + holdTime)){   // if the time reaches 5s, then set held to true, else set to false
                held = true;
            }
            else{
                held = false;
            }
        }
        
        if(Input.GetKey(KeyCode.E) == false){ // if key is not being pressed, just set held to false
            held = false;
        
        }

        if(held && inRange && timeLeft == 0){ // if both player is in range and the button E is pressed for 5 secpmds, then add points to the score, move this to its own method
            //inventory.Add(gameObject);
            cooldown.GetComponent<CooldownScript>().targetTime += 11;
            gameObject.GetComponent<PhotonView>().RPC("destroyObject",RpcTarget.All);   
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


    [PunRPC]
    void destroyObject(){
        Destroy(gameObject);
    }
}
