using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpotLight : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform[] targets;
    public float speed;
    public int start;
    
    private int current;

    void Awake() {
        current = start;
    }
    void Update()
    {
        if (transform.position != targets[current].position) {
            Vector3 pos = Vector3.MoveTowards(transform.position, targets[current].position, speed);
            transform.position = (pos);
        } else {
            current = (current+1)%targets.Length;
        }

    }
}
