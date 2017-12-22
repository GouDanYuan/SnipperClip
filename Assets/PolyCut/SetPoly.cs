using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SetPoly : MonoBehaviour
{
    public Vector3[] vertices;
    public int[] triangles;

    private MeshFilter meshFilter;
    private PolygonCollider2D eCollider;
    int[] tr;
    void Start()
    {
        meshFilter = this.GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.triangles = triangles;
        eCollider = this.GetComponent<PolygonCollider2D>();


        CutCut.GenerateTriangle(ref vertices, ref tr);

        SetMesh(tr, vertices);
    }

    public void ResetMesh()
    {
        SetMesh(tr, vertices);
    }
    public void SetMesh(int[] tr, Vector3[] ver)
    {
        meshFilter.mesh.vertices = ver;
        meshFilter.mesh.triangles = tr;
        Vector2[] temp = new Vector2[vertices.Length + 1];
        Vector3 trans;
        for (int i = 0; i < temp.Length - 1; i++)
        {
            trans = vertices[i];
            temp[i] = trans;
        }
        temp[temp.Length - 1] = vertices[0];
        eCollider.points = temp;
    }

    public void SetMesh(Mesh mesh)
    {
        meshFilter.mesh = mesh;

    }
}
