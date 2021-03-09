using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PressButton : MonoBehaviour
{
    // Start is called before the first frame update
    public bool triggered = true;
    public GameObject gameObject;
    public int id;
    public bool done = false;

    IEnumerator timer() {
        yield return new WaitForSeconds(3);
        this.triggered = false;
    }

    public void Update() {
        if (!this.triggered && !done) {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    [PunRPC]
    void ButtonPressed(int id) {  // do the following
        if (!done) {
            PressButton[] buttons = GameObject.FindObjectsOfType<PressButton>();
            this.triggered = true;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            StartCoroutine(timer());
            bool allTriggered = true;
            foreach (PressButton button in buttons)
            { 
                if(button.id == id) {
                    if (!button.triggered) {
                        allTriggered = false;
                    }
                }
            }
            if (allTriggered) {
                print("hey, all buttons have been triggered");
                gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
                foreach (PressButton button in buttons)
                { 
                    if(button.id == id) {
                        button.GetComponent<MeshRenderer>().material.color = Color.cyan;
                        button.done = true;
                    }
                }
            }
        }
    }

}
