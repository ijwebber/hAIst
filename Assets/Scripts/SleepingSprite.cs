using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingSprite : MonoBehaviour
{
    public SpriteRenderer s;
    public GuardMovement m;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(m.state == State.normal && m.agent.velocity.magnitude == 0)
        {
            s.enabled = true;
        } else s.enabled = false;
    }
}
