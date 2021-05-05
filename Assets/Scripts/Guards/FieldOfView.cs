using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;

    [Range(0,360)]
    public float viewAngle;
    public float meshResolution;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public NavMeshAgent agent;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    [HideInInspector]
    public List<GameObject> visibleTargets = new List<GameObject>();

    [HideInInspector]
    public List<GameObject> behindGuardTargets = new List<GameObject>();



    //calls FindVisibleTargets after every 'delay' seconds, this is started with a coroutine in start() method when guard object is instiated.
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    //finds visible targets within viewing radius + angle, visibleTarget list is cleared everytime it's run to avoid duplicates. This is called used a Enumerator with some delay.
    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        behindGuardTargets.Clear();
        Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //look through all the objects with target tag
        for(int i = 0; i<targetsInView.Length; i++)
        {
            GameObject target = targetsInView[i].gameObject;
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

            //if the object is within the viewangle
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                //we get the distance to the target
                float dstToTarget = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.transform.position.x, 0, target.transform.position.z));

                //checks if obstacle is in way
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask) && !target.GetComponent<PlayerMovement>().disabled)
                {


                    //we can see the target, we add this to our visibletargets list

                    visibleTargets.Add(target);

                }
            }
            else //if not in viewangle then target must be behind the guard, add to behindguardlist
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                //checks if obstacle is in way
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    behindGuardTargets.Add(target);
                }
            }
        }
    }

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
        MeshFilter ViewFilter = GameObject.FindGameObjectWithTag("ViewVis").GetComponent<MeshFilter>();
        Debug.Log("!!!" + ViewFilter.name);
        viewMeshFilter = GameObject.Instantiate<MeshFilter>(ViewFilter);
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetsWithDelay", 0.03f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        viewMeshFilter.transform.position = transform.position;
        viewMeshFilter.transform.rotation = transform.rotation;
        DrawFOV();
        
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
