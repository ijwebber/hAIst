using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (CameraFOV))]
public class CameraFOVEditor : Editor
{
    //This script allows you to see a visual for radius and viewing angle size and it draws lines to the objects that are in the field of view
    private void OnSceneGUI()
    {
        CameraFOV fov = (CameraFOV) target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.GetFOVPosition(), Vector3.up, Vector3.forward, 360, fov.viewRadius);
        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        //draws viewing radius and viewing angle.
        Handles.DrawLine(fov.GetFOVPosition(), fov.GetFOVPosition() + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.GetFOVPosition(), fov.GetFOVPosition() + viewAngleB * fov.viewRadius);

        //draws a line to every target in visibleTargets in red using their positions
        Handles.color = Color.red;
        foreach(Transform visibleTarget in fov.visibleTargets)
        {
            Handles.DrawLine(fov.GetFOVPosition(), visibleTarget.position);
        }
    }
}
