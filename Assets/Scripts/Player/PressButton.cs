using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PressButton : MonoBehaviour
{
    // Start is called before the first frame update
    public bool triggered = true;
    public new Light light;
    public int id;
    public bool done = false;

    IEnumerator timer() {
        yield return new WaitForSeconds(3);
        this.triggered = false;
    }

    public void Update() {
        MeshRenderer buttonRenderer = GetComponentInChildren<MeshRenderer>(false);
        if (!this.triggered && !done) {
            light.color = Color.red;
            buttonRenderer.materials[0].color = Color.red;
        } else if (done) {
            buttonRenderer.materials[0].color = Color.green;
        }
    }

    [PunRPC]
    void ButtonPressed(int id) {  // do the following
        if (!done) {
            PressButton[] buttons = GameObject.FindObjectsOfType<PressButton>();
            MeshRenderer buttonRenderer = GetComponentInChildren<MeshRenderer>(false);
            this.triggered = true;
            light.color = Color.yellow;
            buttonRenderer.materials[0].color = Color.yellow;
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
                light.color = Color.green;
                buttonRenderer.materials[0].color = Color.green;
                foreach (PressButton button in buttons)
                { 
                    if(button.id == id) {
                        button.light.color = Color.green;
                        button.done = true;
                    }
                }
            }
        }
    }

}
