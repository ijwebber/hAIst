using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WireManual : MonoBehaviour
{
    public Image wire;

    public int wiresID;

    public Color[] colours = { Color.red, Color.green, Color.blue, Color.yellow };
    
    private void OnEnable()
    {
        Wires[] wiress = GameObject.FindObjectsOfType<Wires>();

        foreach (Wires wires in wiress)
        {
            if (wires.id == wiresID)
            {
                wire.color = colours[wires.wire];
            }
        }
    }
}
