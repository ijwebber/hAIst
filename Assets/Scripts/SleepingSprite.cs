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
            s.color = new Color(s.color.r, s.color.g, s.color.b, 1);
        } else s.color = new Color(s.color.r, s.color.g, s.color.b, 0);
    }
}
