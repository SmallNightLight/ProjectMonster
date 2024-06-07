using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

[ExecuteInEditMode()]
[RequireComponent(typeof(SplineContainer))]
public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private int _resolution;
    [SerializeField] private float _roadWidth = 2;

    private List<Vector3> _positions1;
    private List<Vector3> _positions2;

    [Header("Components")]
    private SplineContainer _splineContainer;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshCollider _meshCollider;

    [SerializeField] private bool _drawDebug;

    private void Start()
    {
        _splineContainer = GetComponent<SplineContainer>();

        UpdateSplines();
    }

    private void UpdateSplines()
    {
        _positions1 = new List<Vector3>();
        _positions2 = new List<Vector3>();

        float step = 1f / _resolution;

        if (_splineContainer.Spline == null)
            return;

        for (int i = 0; i < _splineContainer.Splines.Count; i++)
        {
            for (int j = 0; j < _resolution; j++)
            {
                AddRoadPositions(i, step * j);
            }

            AddRoadPositions(i, 1f);
        }
    }

    private void AddRoadPositions(int splineIndex, float step)
    {
        _splineContainer.Evaluate(splineIndex, step, out float3 position, out float3 forward, out float3 up);
        float3 right = Vector3.Cross(forward, up).normalized;
        _positions1.Add(right * _roadWidth + position);
        _positions2.Add(-right * _roadWidth + position);
    }

    public void GenerateMesh()
    {
        UpdateSplines();

        if (_positions1.Count == 0 || _positions2.Count == 0) return;

        Mesh newMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        float uvOffset = 0f;

        for (int i = 0; i < _splineContainer.Splines.Count; i++)
        {
            int splineOffset = _resolution * i + i;

            for(int j = 1; j < _resolution + 1; j++)
            {
                int vertexOffset = splineOffset + j;

                Vector3 position1 = _positions1[vertexOffset - 1] - transform.position;
                Vector3 position2 = _positions2[vertexOffset - 1] - transform.position;
                Vector3 position3 = _positions1[vertexOffset] - transform.position;
                Vector3 position4 = _positions2[vertexOffset] - transform.position;

                int offset = (4 * _resolution * i) + 4 * (j - 1);

                int triangle1 = offset + 0;
                int triangle2 = offset + 2;
                int triangle3 = offset + 3;
                int triangle4 = offset + 3;
                int triangle5 = offset + 1;
                int triangle6 = offset + 0;

                //Set verticles
                vertices.Add(position1);
                vertices.Add(position2);
                vertices.Add(position3);
                vertices.Add(position4);

                //Set triangles
                triangles.Add(triangle1);
                triangles.Add(triangle2);
                triangles.Add(triangle3);
                triangles.Add(triangle4);
                triangles.Add(triangle5);
                triangles.Add(triangle6);

                float distance = Vector3.Distance(position1, position3) / 4f;
                float uvDistance = uvOffset + distance;

                uvs.Add(new Vector2(1, uvOffset));
                uvs.Add(new Vector2(0, uvOffset));
                uvs.Add(new Vector2(1, uvDistance));
                uvs.Add(new Vector2(0, uvDistance));
                
                uvOffset += distance;
            }
        }

        newMesh.SetVertices(vertices);
        newMesh.SetTriangles(triangles, 0);
        newMesh.RecalculateNormals();
        newMesh.SetUVs(0, uvs);

        if (_meshFilter)
            _meshFilter.mesh = newMesh;

        if (_meshCollider)
            _meshCollider.sharedMesh = newMesh;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        GenerateMesh();
    }
#endif

    //Debug
    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        GenerateMesh();
    }

    private void OnDrawGizmosSelected()
    {
        if (!_drawDebug || _positions1 == null || _positions2 == null) return;

        for(int i = 0; i < _positions1.Count; i++)
        {
            Gizmos.DrawSphere(_positions1[i], 1);
        }

        for (int i = 0; i < _positions2.Count; i++)
        {
            Gizmos.DrawSphere(_positions2[i], 1);
        }
    }
}