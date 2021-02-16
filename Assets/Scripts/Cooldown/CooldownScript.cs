using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CooldownScript : MonoBehaviour
{

    public int seconds;

    public float targetTime = 0;    // increase the target time by a certain amount i.e. 10s
    // Start is called before the first frame update

    public Text cooldownBox;
    void Start()
    {

        cooldownBox.text = "";
        
    }

    // Update is called once per frame
    void Update()
    {
        if(targetTime != 0 && !(seconds < 0)){      // perhaps change this so from the pick up script, you set target time to 0 if the time left is 0/ seconds
            targetTime -= Time.deltaTime;           // in float
            seconds = (int)(targetTime % 60);       // convert from float, updating the seconds variable
        }                                           // this chunk of code can be moved to the HUD script
        else if(targetTime < 0 || seconds < 0){
            seconds = 0;
            targetTime = 0;
        }
        else{
            seconds = 0;
            targetTime = 0;
        }


        if(seconds>0){cooldownBox.text = "Cooldown: " + seconds;}
        else{cooldownBox.text = "";}
    }
}
