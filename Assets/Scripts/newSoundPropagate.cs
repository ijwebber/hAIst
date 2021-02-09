using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newSoundPropagate : MonoBehaviour
{

    public Transform player;
    private List<GameObject> guards;
    // Start is called before the first frame update
    void Start()
    {
        guards = getGuards();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown("p")) {
            // determime max range of sound travel
            float distance = soundDistance(60,20,1);
            // raycast to guards

        }

        // soft sound
        if (Input.GetKeyDown("o")) {
            // determine max range of sound travel
            float distance = soundDistance(50,20,1);
            RaycastHit hit;
            // raycast to guards
            int LayerMask = (1 << 8);
            print(LayerMask);
            foreach (GameObject g in guards)
            {
                if (!Physics.Linecast(player.transform.position, g.transform.position, out hit, LayerMask, QueryTriggerInteraction.UseGlobal) && Vector3.Distance(player.position, g.transform.position) < distance)
                {
                    Debug.DrawLine(player.position, g.transform.position);
                    print("Guard " + g.transform.position + " alerted");
                }
            }
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

    List<GameObject> getGuards() {
        List<GameObject> guards = new List<GameObject>();
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
        foreach(GameObject go in gos)
        {
            if(go.layer==10)
            {
                guards.Add(go);
            }
        } 
        return guards;
    }
}
