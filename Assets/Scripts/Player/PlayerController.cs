using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public float viewRadius;

    [Range(0,360)]
    public float viewAngle;
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
