using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WireManual : MonoBehaviour
{
    public Image wire;
    public Text colour;

    public int wiresID;

    public Color[] colours = { Color.red, Color.green, Color.blue, Color.yellow };
    public string[] strColour = { "RED", "GREEN", "BLUE", "YELLOW" };

    private void OnEnable()
    {
        Wires[] wiress = GameObject.FindObjectsOfType<Wires>();

        foreach (Wires wires in wiress)
        {
            if (wires.id == wiresID)
            {
                // Show correct wire colour
                wire.color = colours[wires.wire];
                colour.text = "To cut the power to the lasers cut the " + strColour[wires.wire] + " wire.";
            }
        }
    }
}
