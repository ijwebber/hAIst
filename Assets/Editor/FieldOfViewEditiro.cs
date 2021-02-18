using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditiro : Editor
{


    //This script allows you to see a visual for radius and viewing angle size and it draws lines to the objects that are in the field of view
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        //draws viewing radius and viewing angle.
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

        //draws a line to every target in visibleTargets in red using their positions
        Handles.color = Color.red;
        foreach(GameObject visibleTarget in fov.visibleTargets)
        {
            Handles.DrawLine(fov.transform.position, visibleTarget.transform.position);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
