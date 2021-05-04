﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class POVMesh : MonoBehaviourPun
{
    public PlayerController playerController;
    public float meshResolution;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private Scene scene;
    public NavMeshAgent agent;
    private MeshFilter viewMeshFilter, objectMeshFilter;
    private Mesh viewMesh;
    private bool start = false;

    private CameraControlPlayer cameraControlPlayer;
    private CameraSystem cameraSystem;

    [HideInInspector]
    public List<GameObject> behindGuardTargets = new List<GameObject>();
    GameObject player;



    void DrawFOV(Vector3 fromPoint) {
        int stepCount = Mathf.RoundToInt(playerController.viewAngle * meshResolution);
        float stepAngleSize = playerController.viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        Debug.Log("!!!! 1 " + stepCount + " //" + stepAngleSize);
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = this.transform.eulerAngles.y - playerController.viewAngle/2 + stepAngleSize*i;
            ViewCastInfo newViewCast = viewCast(angle, fromPoint);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count+1;
        Vector3[] vertices = new Vector3[vertexCount];
        int [] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount-1; i++)
        {
            vertices[i+1] = this.transform.InverseTransformPoint(viewPoints[i]);

            if(i < vertexCount - 2){
                triangles[i*3] = 0;
                triangles[i*3+1] = i+1;
                triangles[i*3+2] = i+2;
            }

        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
    ViewCastInfo viewCast(float globalAngle, Vector3 fromPoint) {
        Vector3  dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(fromPoint, dir, out hit, playerController.viewRadius, obstacleMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        } else {
            return new ViewCastInfo(false, fromPoint + dir * playerController.viewRadius, playerController.viewRadius, globalAngle);
        }

    }

    //takes in an angle and gives its direction 
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += player.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindObjectOfType<PlayerController>();
        player = playerController.getPlayer();
        var cameraControlPlayers = GameObject.FindObjectsOfType<CameraControlPlayer>();
        cameraSystem = GameObject.FindObjectOfType<CameraSystem>();
        foreach (var ccm in cameraControlPlayers)
        {
            if (ccm.photonView.IsMine) {
                cameraControlPlayer = ccm;
            }
        }
        // cameraControlPlayer = GameObject.FindObjectsOfType<CameraControlPlayer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (this.photonView.IsMine) {
            if (SceneManager.GetActiveScene().name == "BuildScene" || SceneManager.GetActiveScene().name == "ArtLevel" && !start) {
                MeshFilter ViewFilter = GameObject.FindGameObjectWithTag("POVObjects").GetComponent<MeshFilter>();
                MeshFilter ViewFilter3 = GameObject.FindGameObjectWithTag("POVGuards").GetComponent<MeshFilter>();

                // viewMeshFilter = ViewFilter;
                viewMeshFilter = ViewFilter3;
                objectMeshFilter = ViewFilter;
                viewMesh = new Mesh();
                viewMesh.name = "POV mesh";
                viewMeshFilter.mesh = viewMesh;
                objectMeshFilter.mesh = viewMesh;
                start = true;
            }
            if (start) {
                Vector3 fromPoint = player.transform.position;
                if (cameraSystem.isCaughtCutSceneHappening) {
                    fromPoint = cameraSystem.guardCaughtIn4k.LookAt.position;
                } else {
                    viewMeshFilter.transform.position = new Vector3(fromPoint.x, 16.5f, fromPoint.z);
                    viewMeshFilter.transform.rotation = player.transform.rotation;
                    objectMeshFilter.transform.position = new Vector3(fromPoint.x, 16.5f, fromPoint.z);
                    objectMeshFilter.transform.rotation = player.transform.rotation;
                }
                DrawFOV(fromPoint);
            }
        }
    }

    public struct ViewCastInfo {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle) {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }
}
