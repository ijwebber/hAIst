using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    public float maxRange;
    public float currentRange;
    public float intensity;
    private float[,] coords = new float[51, 2];
    public LineRenderer line;
    public int segments = 50;
    public bool finished = false;

    public static Pulse CreatePulse(GameObject where, Transform player, LineRenderer line, float maxRange, float currentRange, float intensity)
    {
        if (line == null)
        {
            Debug.Log("Line is null");
        }
        LineRenderer clone = Instantiate(line, player.position, Quaternion.identity);
        Pulse p = new GameObject().AddComponent<Pulse>();
        p.line = clone;
        p.maxRange = maxRange;
        p.currentRange = currentRange;
        p.intensity = intensity;

        return p;
    }

    public void Update()
    {
        float x = 0;
        float y = 0;
        float z = 0;

        float angle = 20f;

        LayerMask layermask = LayerMask.GetMask("Player");
        //Debug.Log(layermask);

        if (this.currentRange >= this.maxRange)
        {
            Destroy(this.line.gameObject);
            Destroy(this);
        }
        else
        {
            for (int i = 0; i < (this.segments + 1); i++)
            {
                float currx = 0;
                float curry = 0;
                currx = this.coords[i, 0];
                curry = this.coords[i, 1];
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * this.currentRange;
                y = Mathf.Cos(Mathf.Deg2Rad * angle) * this.currentRange;
                // Debug.Log((player.position) + " // " + (player.position + new Vector3(x,1,y)));
                if (Physics.Linecast(this.line.transform.position, this.line.transform.position + new Vector3(x, 0, y), ~(1 << 9)))
                {
                    //Debug.Log("INTERSECTED");
                    x = currx;
                    y = curry;
                }
                else
                {
                    this.coords[i, 0] = x;
                    this.coords[i, 1] = y;
                }

                this.line.SetPosition(i, new Vector3(x, 0, y));

                angle += (360f / this.segments);
            }
            
            this.currentRange += 1f;
        }
    }

}

public class emitSound : MonoBehaviour
{
    void Start() {

        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = (segments + 1);
        line.useWorldSpace = false;
    }

    public bool soundPulse = false;
    public List<Pulse> pulses = new List<Pulse>();

    // Update is called once per frame
    public Transform player;
    public LineRenderer line;
    [Range(0,50)]
    public int segments = 50;
    [Range(0,5)]
    public float xradius = 5;
    [Range(0,5)]
    public float yradius = 5;
    public float[,] coords = new float[51, 2];
    void Update()
    {

        // normal sound
        if (Input.GetKeyDown("p")) {
            // set position of line renderer to players current position
            //currentRange = 0;

            //line.transform.position = player.position + new Vector3(0,1,0);

            // for (int i = 0; i < (segments + 1); i++) {
            //     coords[i,0] = 0;
            //     coords[i,1] = 0;
            // }

            //LineRenderer clone = Instantiate(line, player.position, Quaternion.identity);

            // determime max range of sound travel
            float distance = soundDistance(60,20);
            Pulse newPulse = Pulse.CreatePulse(gameObject, player, line, distance, 0, 60);
        }

        // soft sound
        if (Input.GetKeyDown("o")) {
            // set position of line renderer to players current position
            //currentRange = 0;
            //line.transform.position = player.position;

            // determime max range of sound travel
            float distance = soundDistance(60,20);
            Pulse newPulse = Pulse.CreatePulse(gameObject, player, line, distance*2, 0, 60);
        }
    }


    float soundDistance(float dB, float threshold) {
        float distance = 0;
        while (dB > threshold)
        {
            dB -= 6;
            distance++;
        }
        return distance;
    }
}
