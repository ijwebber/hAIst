using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public bool isDisabled = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Timmy");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
