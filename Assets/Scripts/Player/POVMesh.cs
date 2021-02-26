using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class POVMesh : MonoBehaviour
{
    public float viewRadius;

    [Range(0,360)]
    public float viewAngle;
    public float meshResolution;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private Scene scene;
    public NavMeshAgent agent;
    private MeshFilter viewMeshFilter, objectMeshFilter;
    private Mesh viewMesh;
    private bool start = false;

    [HideInInspector]
    public List<GameObject> visibleTargets = new List<GameObject>();

    [HideInInspector]
    public List<GameObject> behindGuardTargets = new List<GameObject>();
    GameObject player;



    void DrawFOV() {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle/2 + stepAngleSize*i;
            ViewCastInfo newViewCast = viewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count+1;
        Vector3[] vertices = new Vector3[vertexCount];
        int [] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount-1; i++)
        {
            vertices[i+1] = transform.InverseTransformPoint(viewPoints[i]);

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
    ViewCastInfo viewCast(float globalAngle) {
        Vector3  dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        } else {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }

    }

    //takes in an angle and gives its direction 
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Timmy");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "BuildScene" && !start) {
            MeshFilter[] filters = GameObject.FindObjectsOfType<MeshFilter>();
            MeshFilter ViewFilter3 = new MeshFilter();
            MeshFilter ViewFilter = new MeshFilter();
            foreach (MeshFilter filter in filters)
            {
                if(filter.name == "POVGuards") {
                    Debug.Log(filter.name);
                    ViewFilter3 = filter;
                }
                if(filter.name == "POVObjects") {
                    ViewFilter = filter;
                }
            }
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
            viewMeshFilter.transform.position = new Vector3(player.transform.position.x, 16.5f, player.transform.position.z);
            viewMeshFilter.transform.rotation = player.transform.rotation;
            objectMeshFilter.transform.position = new Vector3(player.transform.position.x, 16.5f, player.transform.position.z);
            objectMeshFilter.transform.rotation = player.transform.rotation;
            DrawFOV();
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
