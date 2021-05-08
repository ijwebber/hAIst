using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MuteController : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("pointer enter");
        gameObject.GetComponent<Image>().color = new Color(195, 36, 36);
            

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("pointer exit");

        gameObject.GetComponent<Image>().color = new Color(76, 7, 7);

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}