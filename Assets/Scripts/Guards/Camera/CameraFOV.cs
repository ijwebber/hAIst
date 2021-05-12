using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFOV : MonoBehaviour {

	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;
    public float shift = 0;

	public LayerMask targetMask;
	public LayerMask obstacleMask;
    public MeshFilter viewMeshFilter;
    public float meshResolution;
    public State cameraState = State.normal;
    Mesh viewMesh;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

    GuardController guardController;

	void Start() {
        guardController = GameObject.FindObjectOfType<GuardController>();

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetsWithDelay", 0.2f);
	}

    void Update() {
        if (!GetComponent<CameraProps>().disabled) {
            DrawFOV();
        } else {
            viewMesh.Clear();
        }
    }


	IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);
            if (!GetComponent<CameraProps>().disabled) {
                GetVisibleTargets();
            }
		}
	}

	void GetVisibleTargets() {
		visibleTargets.Clear();
        
        Vector3 newPos = GetFOVPosition();

        // Gets all targets within the sphere
		Collider[] targetsInViewRadius = Physics.OverlapSphere(newPos, viewRadius, targetMask);
		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - newPos).normalized;

            // Checks if within view angle 
			if (Vector3.Angle (-transform.forward, dirToTarget) < viewAngle / 2) {
				float dstToTarget = Vector3.Distance(newPos, target.position);

				if (!Physics.Raycast (newPos, dirToTarget, dstToTarget, obstacleMask)) {
					visibleTargets.Add(target);
				}
			}
		}

        if (visibleTargets.Count > 0) {
            cameraState = State.suspicious;
            Debug.Log("Suspicious cameera");
        } else {
            cameraState = State.normal;
        }
        foreach (Transform target in visibleTargets) {
            if (!target.gameObject.GetComponent<PlayerPickUp>().down){
                guardController.MoveClosestGuard(target.position);
            }
        }
	}


	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}

        float rotatedAngle = angleInDegrees + 180;
		return new Vector3(Mathf.Sin(rotatedAngle * Mathf.Deg2Rad), 0, Mathf.Cos(rotatedAngle * Mathf.Deg2Rad));
	}

    // Returns where the fov of the camera actually is after shifting it
    public Vector3 GetFOVPosition() {
        Vector3 pos = transform.position;
        Vector3 newPos = new Vector3(pos.x, pos.y - shift, pos.z);
        return newPos;
    }

    void DrawFOV() {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        
        for (int i = 0; i < vertexCount-1; i++)
        {
            vertices[i+1] = transform.InverseTransformPoint(viewPoints[i]);

            if(i < vertexCount - 2) {
                triangles[i*3] = 0;
                triangles[i*3+1] = i+1;
                triangles[i*3+2] = i+2;
            }

        }

        // Fixing the y coord of the 0 vector
        // vertices[0].y = vertices[1].y;

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle) {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(GetFOVPosition(), dir, out hit, viewRadius, obstacleMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        } else {
            return new ViewCastInfo(false, GetFOVPosition() + dir * viewRadius, viewRadius, globalAngle);
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