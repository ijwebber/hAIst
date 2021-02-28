using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RotateImage : MonoBehaviour
{
   public Image image;
   public void rotateImage(){
        image.GetComponent<RectTransform>().Rotate(0f,0f,-90f);      // rotate each image by 90 degrees
    }                                                               // this is assigned to each image but can be integrated into the rotatetask script
  
}
