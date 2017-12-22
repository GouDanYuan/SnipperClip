using System.Collections;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using UnityEngine;
using ClipperLib;

public class CutCut
{
    private static List<int> allTriangles = new List<int>();
    private static List<Vector3> allVerticals = new List<Vector3>();



    public static Mesh[] GenerateMesh(Transform cut, Transform sub, ClipType clipType)
    {
        List<List<IntPoint>> res = new List<List<IntPoint>>();
        res = ClippingPoly(cut, sub, clipType);

        if (res == null || res.Count == 0)
            return null;
        Mesh[] meshs = new Mesh[res.Count];

        for (int i = 0; i < res.Count; i++)
        {
            Vector3[] vers = new Vector3[res[i].Count];
            for (int j = 0; j < vers.Length; j++)
                vers[j] = sub.InverseTransformPoint(new Vector3(res[i][j].X, res[i][j].Y));
            int[] tris = null;
            GenerateTriangle(ref vers, ref tris);
            meshs[i] = new Mesh();
            meshs[i].vertices = vers;
            meshs[i].triangles = tris;
        }
        return meshs;
    }



    public static void GenerateTriangle(ref Vector3[] vertical, ref int[] triangles)
    {
        Polygon poly = new Polygon();
        Vertex[] ver = new Vertex[vertical.Length];
        for (int i = 0; i < ver.Length; i++)
            ver[i] = new Vertex(vertical[i].x, vertical[i].y, 1);

        poly.Add(new Contour(ver, 1));

        ConstraintOptions options = new ConstraintOptions();
        QualityOptions quality = new QualityOptions();

        IMesh mesh = poly.Triangulate(options, quality);
        allTriangles.Clear();
        allVerticals.Clear();
        foreach (Triangle item in mesh.Triangles)
        {
            allTriangles.Add(item.GetVertexID(2));
            allTriangles.Add(item.GetVertexID(1));
            allTriangles.Add(item.GetVertexID(0));
        }
        foreach (Vertex item in mesh.Vertices)
        {
            allVerticals.Add(new Vector3((float)item.X, (float)item.Y));
        }
        vertical = allVerticals.ToArray();
        triangles = allTriangles.ToArray();
    }


    public static List<List<IntPoint>> ClippingPoly(Transform cut, Transform sub, ClipType clipType)
    {
        Clipper clip = new Clipper();
        List<List<IntPoint>> res = new List<List<IntPoint>>();
        AddMeshVerticals(cut, clip, PolyType.ptClip);
        AddMeshVerticals(sub, clip, PolyType.ptSubject);
        clip.Execute(clipType, res, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
        return res;
    }

    private static void AddMeshVerticals(Transform trans, Clipper clip, PolyType type)
    {
        List<IntPoint> tempPoint = new List<IntPoint>();
        Vector3 temp;
        MeshFilter[] meshFilter = trans.GetComponentsInChildren<MeshFilter>();

        for (int j = 0; j < meshFilter.Length; j++)
        {
            tempPoint.Clear();
            for (int i = 0; i < meshFilter[j].mesh.vertices.Length; i++)
            {
                temp = trans.TransformPoint(meshFilter[j].mesh.vertices[i]);
                tempPoint.Add(new IntPoint(temp.x, temp.y));
            }
            clip.AddPath(tempPoint, type, true);
        }
    }

}
