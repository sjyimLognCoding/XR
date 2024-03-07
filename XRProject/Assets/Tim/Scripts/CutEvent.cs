using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutEvent : MonoBehaviour
{
    private bool edgeSet = false;
    private Vector3 edgeVertex = Vector3.zero;
    private Vector2 edgeUV = Vector2.zero;
    private Plane edgePlane = new Plane();

    GameObject paper;

    GameObject entry, exit, middle;

    public GameObject temp;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DestroyMesh(temp);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        float x = 0, y = 0, z = 0;
        foreach (var ct in other.contacts)
        {
            x += ct.point.x;
            y += ct.point.y;
            z += ct.point.z;
        }

        x /= 4;
        y /= 4;
        z /= 4;

        entry = new GameObject();
        entry.transform.position = new Vector3(x, y, z);

        // if (other.transform.CompareTag("Paper"))
        // {
        //     paper = other.gameObject;
        //     print(paper.name);
        // }
    }

    ContactPoint[] contacts;

    private void OnCollisionStay(Collision other)
    {
        contacts = other.contacts;
    }

    private void OnCollisionExit(Collision other)
    {
        float x = 0, y = 0, z = 0;
        foreach (var ct in contacts)
        {
            x += ct.point.x;
            y += ct.point.y;
            z += ct.point.z;
        }

        x /= 4;
        y /= 4;
        z /= 4;

        exit = new GameObject();
        exit.transform.position = new Vector3(x, y, z);

        middle = new GameObject();
        middle.transform.position = (entry.transform.position + exit.transform.position) / 2;
        middle.transform.position += transform.forward / 3;

        // print(other.GetContacts(contacts));
        // GameObject go = new GameObject();

        // go.transform.position = other.contacts[other.contactCount - 1].point + new Vector3(0, GetComponent<Collider>().bounds.extents.y,
        // other.gameObject.GetComponent<Collider>().bounds.extents.z);
    }

    private void DestroyMesh(GameObject target)
    {
        Mesh originalMesh = target.GetComponent<MeshFilter>().mesh;
        originalMesh.RecalculateBounds();

        List<PartMesh> parts = new List<PartMesh>();
        List<PartMesh> subParts = new List<PartMesh>();

        PartMesh mainPart = new PartMesh()
        {
            UV = originalMesh.uv,
            Vertices = originalMesh.vertices,
            Normals = originalMesh.normals,
            Triangles = new int[originalMesh.subMeshCount][],
            bounds = originalMesh.bounds
        };

        for (int i = 0; i < originalMesh.subMeshCount; i++)
        {
            mainPart.Triangles[i] = originalMesh.GetTriangles(i);
        }

        parts.Add(mainPart);

        Plane plane = new Plane(entry.transform.position, middle.transform.position, exit.transform.position);

        subParts.Add(GenerateMesh(parts[0], plane, true));
        subParts.Add(GenerateMesh(parts[0], plane, false));

        parts = new List<PartMesh>(subParts);
        // subParts.Clear();

        for (int i = 0; i < subParts.Count; i++)
        {
            subParts[i].MakeGameObject(target);
        }

        Destroy(target);
    }

    private PartMesh GenerateMesh(PartMesh original, Plane plane, bool left)
    {
        PartMesh partMesh = new PartMesh() { };
        Ray ray1 = new Ray();
        Ray ray2 = new Ray();

        for (int i = 0; i < original.Triangles.Length; i++)
        {
            int[] triangles = original.Triangles[i];
            edgeSet = false;

            for (int j = 0; j < triangles.Length; j = j + 3)
            {
                bool SideA = plane.GetSide(original.Vertices[triangles[j]]) == left;
                bool SideB = plane.GetSide(original.Vertices[triangles[j + 1]]) == left;
                bool SideC = plane.GetSide(original.Vertices[triangles[j + 2]]) == left;

                int sideCount = (SideA ? 1 : 0) + (SideB ? 1 : 0) + (SideC ? 1 : 0);

                if (sideCount == 0) continue;

                if (sideCount == 3)
                {
                    partMesh.AddTriangles(
                        i,
                        original.Vertices[triangles[j]], original.Vertices[triangles[j + 1]], original.Vertices[triangles[j + 2]],
                        original.Normals[triangles[j]], original.Normals[triangles[j + 1]], original.Normals[triangles[j + 2]],
                        original.UV[triangles[j]], original.UV[triangles[j + 1]], original.UV[triangles[j + 2]]
                        );

                    continue;
                }

                int index = SideB == SideC ? 0 : SideA == SideC ? 1 : 2;

                ray1.origin = original.Vertices[triangles[j + index]];
                Vector3 direction1 = original.Vertices[triangles[j + ((index + 1) % 3)]]
                                        - original.Vertices[triangles[j + index]];
                ray1.direction = direction1;
                plane.Raycast(ray1, out float entry1);
                float lerp1 = entry1 / direction1.magnitude;

                ray2.origin = original.Vertices[triangles[j + index]];
                Vector3 direction2 = original.Vertices[triangles[j + ((index + 2) % 3)]]
                                        - original.Vertices[triangles[j + index]];
                ray2.direction = direction2;
                plane.Raycast(ray1, out float entry2);
                float lerp2 = entry2 / direction2.magnitude;

                AddEdge(i, partMesh,
                left ? plane.normal * -1f : plane.normal,
                ray1.origin + ray1.direction.normalized * entry1,
                ray2.origin + ray2.direction.normalized * entry2,
                Vector2.Lerp(original.UV[triangles[j + index]], original.UV[triangles[j + ((index + 1) % 3)]], lerp1),
                Vector2.Lerp(original.UV[triangles[j + index]], original.UV[triangles[j + ((index + 2) % 3)]], lerp2));

                if (sideCount == 1)
                {
                    partMesh.AddTriangles(
                        i,
                        original.Vertices[triangles[j + index]],
                        ray1.origin + ray1.direction.normalized * entry1,
                        ray2.origin + ray2.direction.normalized * entry2,

                        original.Normals[triangles[j + index]],
                        Vector3.Lerp(original.Normals[triangles[j + index]], original.Normals[triangles[j + ((index + 1) % 3)]], lerp1),
                        Vector3.Lerp(original.Normals[triangles[j + index]], original.Normals[triangles[j + ((index + 2) % 3)]], lerp2),

                        original.UV[triangles[j + index]],
                        Vector2.Lerp(original.UV[triangles[j + index]], original.UV[triangles[j + ((index + 1) % 3)]], lerp1),
                        Vector2.Lerp(original.UV[triangles[j + index]], original.UV[triangles[j + ((index + 2) % 3)]], lerp2)
                        );

                    continue;
                }

                if (sideCount == 2)
                {
                    partMesh.AddTriangles(i,
                    ray1.origin + ray1.direction.normalized * entry1,
                    original.Vertices[triangles[j + ((index + 1) % 3)]],
                    original.Vertices[triangles[j + ((index + 2) % 3)]],

                    Vector3.Lerp(original.Normals[triangles[j + index]], original.Normals[triangles[j + (index + 1) % 3]], lerp1),
                    original.Normals[triangles[j + (index + 1) % 3]],
                    original.Normals[triangles[j + (index + 2) % 3]],

                    Vector2.Lerp(original.UV[triangles[j + index]], original.UV[triangles[j + (index + 1) % 3]], lerp1),
                    original.UV[triangles[j + (index + 1) % 3]],
                    original.UV[triangles[j + (index + 2) % 3]]
                    );

                    partMesh.AddTriangles(i,
                    ray1.origin + ray1.direction.normalized * entry1,
                    original.Vertices[triangles[j + ((index + 2) % 3)]],
                    ray2.origin + ray2.direction.normalized * entry2,

                    Vector3.Lerp(original.Normals[triangles[j + index]], original.Normals[triangles[j + (index + 1) % 3]], lerp1),
                    original.Normals[triangles[j + (index + 2) % 3]],
                    Vector3.Lerp(original.Normals[triangles[j + index]], original.Normals[triangles[j + (index + 2) % 3]], lerp2),

                    Vector2.Lerp(original.UV[triangles[j + index]], original.UV[triangles[j + (index + 1) % 3]], lerp1),
                    original.UV[triangles[j + (index + 2) % 3]],
                    Vector2.Lerp(original.UV[triangles[j + index]], original.UV[triangles[j + (index + 2) % 3]], lerp2)
                    );

                    continue;
                }
            }
        }
        partMesh.FillArrays();

        return partMesh;
    }

    private void AddEdge(int submesh, PartMesh partMesh,
    Vector3 normal, Vector3 vert1, Vector3 vert2,
    Vector3 uv1, Vector3 uv2)
    {
        if (!edgeSet)
        {
            edgeSet = true;
            edgeVertex = vert1;
            edgeUV = uv1;
        }

        else
        {
            edgePlane.Set3Points(edgeVertex, vert1, vert2);

            partMesh.AddTriangles(submesh, edgeVertex,
                edgePlane.GetSide(edgeVertex + normal) ? vert1 : vert2,
                edgePlane.GetSide(edgeVertex + normal) ? vert2 : vert1,
                normal, normal, normal,
                edgeUV, uv1, uv2
            );
        }
    }
}


public class PartMesh
{
    //Public
    public Vector3[] Vertices;
    public Vector3[] Normals;
    public int[][] Triangles;
    public Vector2[] UV;
    public GameObject go;
    public Bounds bounds = new Bounds();

    //Private
    private List<Vector3> _vertices = new List<Vector3>();
    private List<Vector3> _normals = new List<Vector3>();
    private List<List<int>> _triangles = new List<List<int>>();
    private List<Vector2> _uv = new List<Vector2>();

    public PartMesh()
    {

    }

    public void AddTriangles(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3,
    Vector3 normal1, Vector3 normal2, Vector3 normal3,
    Vector3 uv1, Vector3 uv2, Vector3 uv3)
    {
        if (_triangles.Count - 1 < submesh)
        {
            _triangles.Add(new List<int>());
        }

        _triangles[submesh].Add(_vertices.Count);
        _vertices.Add(vert1);
        _triangles[submesh].Add(_vertices.Count);
        _vertices.Add(vert2);
        _triangles[submesh].Add(_vertices.Count);
        _vertices.Add(vert3);

        _normals.Add(normal1);
        _normals.Add(normal2);
        _normals.Add(normal3);

        _uv.Add(uv1);
        _uv.Add(uv2);
        _uv.Add(uv3);

        bounds.min = Vector3.Min(bounds.min, vert1);
        bounds.min = Vector3.Min(bounds.min, vert2);
        bounds.min = Vector3.Min(bounds.min, vert3);

        bounds.max = Vector3.Min(bounds.max, vert1);
        bounds.max = Vector3.Min(bounds.max, vert2);
        bounds.max = Vector3.Min(bounds.max, vert3);
    }

    public void FillArrays()
    {
        Vertices = _vertices.ToArray();
        Normals = _normals.ToArray();
        UV = _uv.ToArray();

        Triangles = new int[_triangles.Count][];
        for (int i = 0; i < _triangles.Count; i++)
        {
            Triangles[i] = _triangles[i].ToArray();
        }
    }

    public void MakeGameObject(GameObject original)
    {
        go = new GameObject(original.name);
        go.transform.position = original.transform.position;
        go.transform.rotation = original.transform.rotation;
        go.transform.localScale = original.transform.localScale;

        Mesh newMesh = new Mesh();
        newMesh.name = original.GetComponent<MeshFilter>().mesh.name;

        newMesh.vertices = Vertices;
        newMesh.normals = Normals;
        newMesh.uv = UV;
        for (int i = 0; i < Triangles.Length; i++)
        {
            newMesh.SetTriangles(Triangles[i], i, true);
        }

        bounds = newMesh.bounds;

        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        renderer.materials = original.GetComponent<MeshRenderer>().materials;

        MeshFilter filter = go.AddComponent<MeshFilter>();
        filter.mesh = newMesh;

        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;

        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.useGravity = true;

        // CutEvent meshCut = gameObject.AddComponent<MeshCut>();
    }
}
