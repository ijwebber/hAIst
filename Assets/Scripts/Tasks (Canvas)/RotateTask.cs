using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RotateTask : MonoBehaviour
{
    [SerializeField]
    private Image[] pictures;           // define image containers

    [SerializeField]
    private Sprite[] spriteSetLS;

    [SerializeField]
    private Sprite[] spriteSetSN;


    public bool win;
    private void OnEnable() {
        // set win to false
        win = false;

        int randomNum = Random.Range(0,2);

        //int randomNum = 1;

        if(randomNum == 0){
            pictures[0].sprite = spriteSetLS[0];
            pictures[1].sprite = spriteSetLS[1];
            pictures[2].sprite = spriteSetLS[2];
            pictures[3].sprite = spriteSetLS[3];
            pictures[4].sprite = spriteSetLS[4];
            pictures[5].sprite = spriteSetLS[5];
        }
        else if(randomNum == 1){
            pictures[0].sprite = spriteSetSN[0];
            pictures[1].sprite = spriteSetSN[1];
            pictures[2].sprite = spriteSetSN[2];
            pictures[3].sprite = spriteSetSN[3];
            pictures[4].sprite = spriteSetSN[4];
            pictures[5].sprite = spriteSetSN[5];
        }

        // Initialise Rotations

        // implement a random number selector between the angles
        // make rotation random

        pictures[0].GetComponent<Transform>().Rotate(0f,0f,180f);
        pictures[1].GetComponent<Transform>().Rotate(0f,0f,-90f);
        pictures[2].GetComponent<Transform>().Rotate(0f,0f,180f);
        pictures[3].GetComponent<Transform>().Rotate(0f,0f,90f);
        pictures[4].GetComponent<Transform>().Rotate(0f,0f,90f);
        pictures[5].GetComponent<Transform>().Rotate(0f,0f,180f);

        
    }
 
    // Update is called once per frame
    void Update()
    {

        float picture_1 = pictures[0].GetComponent<Transform>().rotation.z; // get rotation values
        float picture_2 = pictures[1].GetComponent<Transform>().rotation.z;
        float picture_3 = pictures[2].GetComponent<Transform>().rotation.z;
        float picture_4 = pictures[3].GetComponent<Transform>().rotation.z;
        float picture_5 = pictures[4].GetComponent<Transform>().rotation.z;
        float picture_6 = pictures[5].GetComponent<Transform>().rotation.z;

        int rotation_1 = (Mathf.RoundToInt(picture_1));
        int rotation_2 = (Mathf.RoundToInt(picture_2));
        int rotation_3 = (Mathf.RoundToInt(picture_3));
        int rotation_4 = (Mathf.RoundToInt(picture_4));
        int rotation_5 = (Mathf.RoundToInt(picture_5)); // convert all the rotations
        int rotation_6 = (Mathf.RoundToInt(picture_6)); 

        if(rotation_1 == 0              // check to see if each image is the right way up
        && rotation_2 == 0
        && rotation_3 == 0
        && rotation_4  == 0
        && rotation_5 == 0
        && rotation_6 == 0){

            win = true;
        }

        /*if(pictures[0].rotation.z == 0 
        && pictures[1].rotation.z == 0
        &&pictures[2].rotation.z == 0
        &&pictures[3].rotation.z == 0
        &&pictures[4].rotation.z == 0
        &&pictures[5].rotation.z == 0){

            win = true;
            print("WINNER");

        }*/  
        
    }
}
