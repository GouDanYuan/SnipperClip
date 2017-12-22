using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ClipperLib;

public class CutControl : MonoBehaviour
{
    public Transform cut;
    public Transform sub;

    public ClipType clipType = ClipType.ctDifference;
    

    private void OnClickCut()
    {
        MeshFilter[] childCutMeshs = new MeshFilter[cut.childCount];
        MeshFilter[] childSubMeshs = new MeshFilter[sub.childCount];
        for (int i = 0; i < childCutMeshs.Length; i++)
            childCutMeshs[i] = cut.GetChild(i).GetComponent<MeshFilter>();
        for (int i = 0; i < childSubMeshs.Length; i++)
            childSubMeshs[i] = sub.GetChild(i).GetComponent<MeshFilter>();

        Mesh[] meshs = CutCut.GenerateMesh(cut, sub, clipType);

        if (meshs == null)
            return;


        if (meshs.Length - childSubMeshs.Length > 0)
        {
            for (int i = 0; i < meshs.Length - childSubMeshs.Length; i++)
            {
                GameObject temp = Instantiate<GameObject>(sub.GetChild(0).gameObject, sub);
                if (temp.GetComponent<SetPoly>() != null)
                    Destroy(temp.GetComponent<SetPoly>());
            }
        }

        childSubMeshs = new MeshFilter[sub.childCount];
        for (int i = 0; i < childSubMeshs.Length; i++)
            childSubMeshs[i] = sub.GetChild(i).GetComponent<MeshFilter>();


        foreach (MeshFilter item in childSubMeshs)
            item.gameObject.SetActive(false);

        for (int i = 0; i < meshs.Length; i++)
        {
            childSubMeshs[i].mesh = meshs[i];
            SetEdgeCollider(childSubMeshs[i]);

            childSubMeshs[i].gameObject.SetActive(true);
        }
    }

    private void SetEdgeCollider(MeshFilter meshFilter)
    {
        PolygonCollider2D eCollider = meshFilter.GetComponent<PolygonCollider2D>();
        Vector2[] temp = new Vector2[meshFilter.mesh.vertices.Length + 1];
        Vector3 trans;
        for (int i = 0; i < temp.Length - 1; i++)
        {
            trans = meshFilter.mesh.vertices[i];
            temp[i] = trans;
        }
        temp[temp.Length - 1] = meshFilter.mesh.vertices[0];
        eCollider.points = temp;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Key_Q();
        }
        if (Input.GetKey(KeyCode.E))
        {
            Key_E();
        }
        if (Input.GetKey(KeyCode.W))
        {
            Key_W();
        }
        if (Input.GetKey(KeyCode.S))
        {
            Key_S();
        }
        if (Input.GetKey(KeyCode.A))
        {
            Key_A();
        }
        if (Input.GetKey(KeyCode.D))
        {
            Key_D();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Key_X();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Key_Z();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            OnClickSwtich();
        }

    }

    public void OnClickSwtich()
    {
        Transform temp = cut;
        cut = sub;
        sub = temp;
    }
    public void Key_Q()
    {
        sub.transform.rotation *= Quaternion.Euler(0, 0, 2f);
    }
    public void Key_E()
    {
        sub.transform.rotation *= Quaternion.Euler(0, 0, -2f);
    }
    public void Key_W()
    {
        sub.transform.position += Vector3.up * 1;
    }
    public void Key_S()
    {
        sub.transform.position += Vector3.up * -1;
    }
    public void Key_A()
    {
        sub.transform.position += Vector3.left * 1;
    }
    public void Key_D()
    {
        sub.transform.position += Vector3.left * -1;
    }
    public void Key_X()
    {
        OnClickCut();
    }
    public void Key_Z()
    {
        foreach (MeshFilter item in sub.GetComponentsInChildren<MeshFilter>())
            item.mesh = null;
        sub.GetComponentInChildren<SetPoly>().ResetMesh();
    }
}
