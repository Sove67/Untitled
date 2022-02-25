using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewAngle;
    public float viewRadius;
    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDistThreshold;
    private MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    public LayerMask obstacleMask;
    public GameObject body;

    // Start is called before the first frame update
    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter = GetComponent<MeshFilter>();
        viewMeshFilter.mesh = viewMesh;
    }

    // Update is called once per frame
    void Update()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector2> viewPoints = new List<Vector2>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < stepCount; i++)
        {
            float angle = body.transform.eulerAngles.z + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDistThresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDistThreshold;
                if (oldViewCast.hit != newViewCast.hit || oldViewCast.hit && newViewCast.hit && edgeDistThresholdExceeded)
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector2.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector2.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertecies = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertecies[0] = Vector3.zero;

        for (int i = 1; i < vertexCount; i++)
        {
            vertecies[i] = transform.InverseTransformPoint(viewPoints[i-1]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertecies;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector2 minPoint = Vector2.zero;
        Vector2 maxPoint = Vector2.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistThresholdExceeded = Mathf.Abs(minViewCast.dist - newViewCast.dist) > edgeDistThreshold;

            if (newViewCast.hit == minViewCast.hit && !edgeDistThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            } else {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        float radian = degree * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector2 dir = DegreeToVector2(globalAngle);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

        if (hit.collider != null) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        } else {
            return new ViewCastInfo(false, (Vector2)transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector2 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector2 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector2 pointA;
        public Vector2 pointB;

        public EdgeInfo(Vector2 _pointA, Vector2 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
