using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTask : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] pictures;

    public bool win;
    private void OnEnable() {
        // set win to false
        win = false;

        // Initialise Rotations

        // implement a random number selector between the angles

        pictures[0].GetComponent<RectTransform>().Rotate(0f,0f,180f);
        pictures[1].GetComponent<RectTransform>().Rotate(0f,0f,90f);
        pictures[2].GetComponent<RectTransform>().Rotate(0f,0f,-180f);
        pictures[3].GetComponent<RectTransform>().Rotate(0f,0f,180f);
        pictures[4].GetComponent<RectTransform>().Rotate(0f,0f,90f);
        pictures[5].GetComponent<RectTransform>().Rotate(0f,0f,180f);

        
    }
 
    // Update is called once per frame
    void Update()
    {

        float picture_1 = pictures[0].GetComponent<RectTransform>().rotation.z; // get rotation values
        float picture_2 = pictures[1].GetComponent<RectTransform>().rotation.z;
        float picture_3 = pictures[2].GetComponent<RectTransform>().rotation.z;
        float picture_4 = pictures[3].GetComponent<RectTransform>().rotation.z;
        float picture_5 = pictures[4].GetComponent<RectTransform>().rotation.z;
        float picture_6 = pictures[5].GetComponent<RectTransform>().rotation.z;

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
