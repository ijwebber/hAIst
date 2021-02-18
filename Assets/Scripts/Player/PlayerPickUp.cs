using UnityEngine;
using Photon.Pun;

public class PlayerPickUp : MonoBehaviourPun
{

    bool stealObject = false;

    public GameObject keycodeGame;

    public GameObject fixPaintingGame;

    public bool keyCorrect;

    public bool paintingCorrect;

    int gameSelection;


    // Update is called once per frame
    void Update()
    {

        keyCorrect = keycodeGame.GetComponent<KeycodeTask>().codeCorrect;   //dont forget to reset these values
        paintingCorrect = fixPaintingGame.GetComponent<RotateTask>().win;
        
    }


    private void OnCollisionEnter(Collision other) {    // what to do once player enters

        if(other.gameObject.tag == "steal"){            // check to see if item is stealable
                                                        // pick random number here on first collision 
            stealObject = true;         

            System.Random r = new System.Random();
            gameSelection = r.Next(0,2);
        }                                              

    }

    private void OnCollisionStay(Collision other) {    // what to do whilst players are in range of object

        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            
            Inventory inventory = GetComponent<Inventory>();

            if (other.gameObject.tag == "steal") {
                if (gameSelection == 0) {
                    if(Input.GetKey(KeyCode.E) && !inventory.isFull()) {
                        // whip out mini game
                        keycodeGame.SetActive(true);
                    } 
                    else if (keyCorrect) {   
                        inventory.Add(other.gameObject);

                        other.gameObject.SetActive(false);
                        keycodeGame.SetActive(false);
                    } 
                    else if (inventory.isFull()) {
                        // message to show that inventory is full
                    }
                    else if (!Input.GetKey(KeyCode.E)){
                        // out put message
                        // check cool down
                    }

                }

                else if (gameSelection == 1) {
                    if (Input.GetKey(KeyCode.E) && !inventory.isFull()) {
                        // whip out mini game
                        fixPaintingGame.SetActive(true);
                    }
                    else if (paintingCorrect) {

                        inventory.Add(other.gameObject);

                        other.gameObject.SetActive(false);
                        fixPaintingGame.SetActive(false);
                    }
                    else if (inventory.isFull()) {
                        // message to show that inventory is full
                    }
                    else if (!Input.GetKey(KeyCode.E)) {
                        // out put message
                        // check cool down
                    }

                }
            }
        }      
    }

    private void OnCollisionExit(Collision other) {    // what to do once player leaves


        keycodeGame.SetActive(false);
        fixPaintingGame.SetActive(false);

        keycodeGame.GetComponent<KeycodeTask>().codeCorrect = false;
        fixPaintingGame.GetComponent<RotateTask>().win = false;

        //stealObject = false;
        
    }
}
