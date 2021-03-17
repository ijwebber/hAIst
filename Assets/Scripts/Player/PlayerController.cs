using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public LayerMask ObMask;
    public float viewRadius;

    [Range(0,360)]
    public float viewAngle;
    public bool isDisabled = false;
    // Start is called before the first frame update

    public bool isInView(Vector3 targetObject)  {
        return (!Physics.Raycast(targetObject, (player.transform.position - targetObject).normalized, Mathf.Min(viewRadius, Vector3.Distance(player.transform.position, targetObject)), ObMask, QueryTriggerInteraction.Ignore) && Vector3.Distance(player.transform.position, targetObject) <= viewRadius);
    }
    void Start()
    {
        player = GameObject.Find("Timmy");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
