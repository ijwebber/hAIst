// DEPRECATED SCRIPT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    public float maxRange;
    public bool achieved;
    public float currentRange;
    public float intensity;
    private float[,] coords = new float[51, 2];
    public LineRenderer line;
    public int segments = 50;
    public int[] segs = new int[51];
    public bool finished = false;

    // initiate sound pulse
    public static Pulse CreatePulse(GameObject where, Vector3 position, LineRenderer line, float maxRange, float currentRange, float intensity)
    {
        if (line == null)
        {
            Debug.Log("Line is null");
        }
        LineRenderer clone = Instantiate(line, position, Quaternion.identity);
        Pulse p = new GameObject().AddComponent<Pulse>();
        p.line = clone;
        p.achieved = false;
        p.maxRange = maxRange;
        p.currentRange = currentRange;
        p.intensity = intensity;
        for (int i = 0; i < 51; i++)
        {
            p.segs[i] = 1;
        }

        return p;
    }

    // propagate sound on GAME update (changed this from frame update for smoother results, also stops game
    // from freezing on too many sound pulses)
    public void FixedUpdate()
    {
        float x = 0;
        float y = 0;

        float angle = 20f;

        // if sound pulse has travelled to its max distance, or all segments have stopped moving, destroy object (save memory)
        // in the future, possibly have a maximum sound pulse limit?
        if (this.currentRange >= this.maxRange || this.achieved)
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
                // update previous coordinates
                currx = this.coords[i, 0];
                curry = this.coords[i, 1];
                // calculate new coordinates
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * this.currentRange;
                y = Mathf.Cos(Mathf.Deg2Rad * angle) * this.currentRange;
                // if the new coordinate is intersected by a collider, keep segment as is
                RaycastHit hit;
                if (Physics.Linecast(this.line.transform.position, this.line.transform.position + new Vector3(x, 0, y), out hit, ~(1 << 9), QueryTriggerInteraction.UseGlobal))
                {

                    //Debug.Log("INTERSECTED");
                    if (this.segs[i] == 0) {
                        //start new pulse
                        float newIntensity = this.intensity - 6 * Mathf.Log(currentRange, 2);
                        float distance = soundDistance(newIntensity,20,hit.distance);
                        // Pulse newPulse = Pulse.CreatePulse(gameObject, hit.point, line, distance, 0, newIntensity);
                    }
                    bool done = true;
                    foreach (int s in segs)
                    {
                        if (s == 1) {
                            done = false;
                        } 
                    }
                    this.achieved = done;
                    segs[i] = 0;
                    x = currx;
                    y = curry;
                }
                else
                {
                    this.coords[i, 0] = x;
                    this.coords[i, 1] = y;
                }

                // draw sound pulse
                this.line.SetPosition(i, new Vector3(x, 0, y));

                angle += (360f / this.segments);
            }
            
            //increment range
            //this.currentRange += 343/150;
            this.currentRange += 0.5f;
        }
    }
    private float soundDistance(float dB, float threshold, float startDistance) {
        float distance = startDistance;
        // double distance = reduce sound by 6dB
        while (dB > threshold)
        {
            dB -= 6;
            distance*=2;
        }
        return distance;
    }

}

public class emitSound : MonoBehaviour
{
    void Start() {

        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = (segments + 1);
        line.useWorldSpace = false;
    }

    // public variables
    public bool soundPulse = false;
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
            // determime max range of sound travel
            Debug.Log("p button down");
            float distance = soundDistance(60,20,1);
            Pulse newPulse = Pulse.CreatePulse(gameObject, player.position, line, distance, 0, 60);
        }

        // soft sound
        if (Input.GetKeyDown("o")) {
            // determime max range of sound travel
            float distance = soundDistance(40,20,1);
            Pulse newPulse = Pulse.CreatePulse(gameObject, player.position, line, distance, 0, 30);
        }
    }
    float soundDistance(float dB, float threshold, float startDistance) {
        float distance = startDistance;
        // double distance = reduce sound by 6dB
        while (dB > threshold)
        {
            dB -= 6;
            distance*=2;
        }
        return distance;
    }
}
