using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossVerticle
{

    /// <summary>顶点 </summary>
    public Vector3 Verticle { get; set; }
    /// <summary> 标志 0 普通点  -1 入点  1出点</summary>
    public int Flag { get; set; }
    /// <summary> 索引剪切框</summary>
    public int CutIndex { get; set; }
    /// <summary> 索引被剪切框</summary>
    public int BaseIndex { get; set; }
    /// <summary> 列表被剪切框索引</summary>
    public int NowBaseIndex { get; set; }
    /// <summary> 列表剪切框索引</summary>
    public int NowCutIndex { get; set; }




    public CrossVerticle(Vector3 verticle, int flag, int indexCut, int indexBase)
    {
        this.Verticle = verticle;
        this.Flag = flag;
        this.CutIndex = indexCut;
        this.BaseIndex = indexBase;
    }

}



public class DrawPoly : MonoBehaviour
{
    /// <summary>基本形状插入结果列表 </summary>
    public List<CrossVerticle> baseRes = new List<CrossVerticle>();
    /// <summary>剪切框插入结果列表 </summary>
    public List<CrossVerticle> cutRes = new List<CrossVerticle>();
    ///<summary>交点列表 </summary>
    public List<CrossVerticle> crossVerticle = new List<CrossVerticle>();
    /// <summary>出入点标志位切换 </summary>
    private int flagIndex = 1;

    public MeshFilter otherMeshFilter;
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Button btn;
    private Mesh mesh;
    private MeshFilter meshFilter;

    private CrossVerticle S;
    private List<CrossVerticle> tr = new List<CrossVerticle>();
    private void Start()
    {
        meshFilter = this.GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        btn.onClick.AddListener(OnClickCut);
    }


    public List<Vector3> test = new List<Vector3>();
    private void TreatTest()
    {
        for (int i = 0; i < pointBase.Length; i++)
        {
            test.Add(pointBase[i]);
        }
    }

    private void Update()
    {
        for (int i = 0; i < test.Count; i++)
        {
            if (i == test.Count - 1)
            {
                Debug.DrawLine(test[i], test[0]);
            }
            else
            {
                Debug.DrawLine(test[i], test[(i + 1)]);
            }
        }
    }

    public Vector3[] pointCut;
    public Vector3[] pointBase;

    private void OnClickCut()
    {

        test.Clear();
        crossVerticle.Clear();
         pointCut = otherMeshFilter.mesh.vertices;
         pointBase = meshFilter.mesh.vertices;
        for (int i = 0; i < pointCut.Length; i++)
            pointCut[i] = otherMeshFilter.transform.TransformPoint(pointCut[i]);
        for (int i = 0; i < pointBase.Length; i++)
            pointBase[i] = meshFilter.transform.TransformPoint(pointBase[i]);
        List<CrossVerticle> temp = new List<CrossVerticle>();

        Vector3 res;
        for (int i = 0; i < pointCut.Length; i++)
        {
            Vector3 a;
            Vector3 b;
            if (i == pointCut.Length - 1)
            {
                a = pointCut[i];
                b = pointCut[0];
            }
            else
            {
                a = pointCut[i];
                b = pointCut[i + 1];
            }
            for (int j = 0; j < pointBase.Length; j++)
            {
                Vector3 c;
                Vector3 d;
                if (j == pointBase.Length - 1)
                {
                    c = pointBase[j];
                    d = pointBase[0];
                }
                else
                {
                    c = pointBase[j];
                    d = pointBase[j + 1];
                }

                if (CaculateCross(a, b, c, d, out res))
                {
                    temp.Add(new CrossVerticle(res, 1, i, j));
                }
            }
            SortDis(a, temp);
            //crossVerticle.Reverse();
        }

        InsertArr(pointBase, pointCut, crossVerticle);
        //  GetTriangles(baseRes, cutRes);
        TreatTest();
    }




    /// <summary>顺序插入两个数组 </summary>
    /// <param name="pointBase">数组Base</param>
    /// <param name="pointCut">数组Cut</param>
    /// <param name="crossVerticle">交点</param>
    private void InsertArr(Vector3[] pointBase, Vector3[] pointCut, List<CrossVerticle> crossVerticle)
    {
        baseRes.Clear();
        cutRes.Clear();
        List<CrossVerticle> temp = new List<CrossVerticle>();
        for (int i = 0; i < pointBase.Length; i++)
        {
            temp.Clear();
            baseRes.Add(new CrossVerticle(pointBase[i], 0, 0, 0));
            baseRes[baseRes.Count - 1].NowBaseIndex = baseRes.Count - 1;
            baseRes[baseRes.Count - 1].Flag = 3;
            for (int j = 0; j < crossVerticle.Count; j++)
            {
                if (crossVerticle[j].BaseIndex == i)
                {
                    temp.Add(crossVerticle[j]);
                    //baseRes.Add(crossVerticle[j]);
                    //baseRes[baseRes.Count - 1].NowBaseIndex = baseRes.Count - 1;
                }
            }
            temp.Sort(delegate (CrossVerticle a, CrossVerticle b)
            {
                if (Vector3.Distance(a.Verticle, pointBase[i]) >= Vector3.Distance(b.Verticle, pointBase[i]))
                    return 1;
                else
                    return -1;
            });

            for (int j = 0; j < temp.Count; j++)
            {
                baseRes.Add(temp[j]);
                baseRes[baseRes.Count - 1].NowBaseIndex = baseRes.Count - 1;
            }
        }

        for (int i = 0; i < pointCut.Length; i++)
        {
            cutRes.Add(new CrossVerticle(pointCut[i], 0, 0, 0));
            cutRes[cutRes.Count - 1].NowCutIndex = cutRes.Count - 1;
            cutRes[cutRes.Count - 1].Flag = 3;

            for (int j = 0; j < crossVerticle.Count; j++)
            {
                if (crossVerticle[j].CutIndex == i)
                {
                    cutRes.Add(crossVerticle[j]);
                    cutRes[cutRes.Count - 1].NowCutIndex = cutRes.Count - 1;
                }
            }
        }
    }


    private void SortDis(Vector3 a, List<CrossVerticle> temp)
    {
        temp.Sort(delegate (CrossVerticle x, CrossVerticle y)
        {
            if (Vector3.Distance(x.Verticle, a) >= Vector3.Distance(y.Verticle, a))
                return 1;
            else
                return -1;
        });
        for (int i = 0; i < temp.Count; i++)
        {
            flagIndex *= -1;
            temp[i].Flag = flagIndex;
            crossVerticle.Add(temp[i]);
        }
        temp.Clear();
    }



    //step 4
    /// <summary>截取三角形</summary>
    /// <param name="baseRes">被剪切</param>
    /// <param name="cutRes">剪切</param>
    private void GetTriangles(List<CrossVerticle> baseRes, List<CrossVerticle> cutRes)
    {
        for (int i = 0; i < baseRes.Count; i++)
        {
            if (baseRes[i].Flag == -1)
            {
                tr.Add(baseRes[i]);
                SaveDeleteIn(baseRes[i]);
                TreatBase(baseRes[i].NowBaseIndex);
                return;
            }
        }
    }
    //step 6
    private void SaveDeleteIn(CrossVerticle crossVerticle)
    {
        S = crossVerticle;
        crossVerticle.Flag = 3;
    }


    //step7
    /// <summary>
    /// 沿数组3顺序取顶点：
    /// </summary>
    /// <param name="index"></param>
    private void TreatBase(int index)
    {
        while (index < 20)
        {
            index = (index + 1) % baseRes.Count;
            if (baseRes[index].Flag == 0)
            {
                tr.Add(baseRes[index]);
            }
            else
            {
                TreatCut(baseRes[index].NowCutIndex);
                return;
            }
        }
    }


    //step8
    /// <summary>
    /// 沿数组4顺序取顶点
    /// </summary>
    /// <param name="index"></param>
    private void TreatCut(int index)
    {
        while (index < 20)
        {
            index %= baseRes.Count;
            if (cutRes[index].Flag == -1)
            {
                tr.Add(cutRes[index]);
            }
            else
            {
                if (cutRes[index] != S)
                {
                    SaveDeleteIn(cutRes[index]);
                    TreatBase(cutRes[index].NowBaseIndex);
                }
                return;
            }
            index++;
        }
    }





    private bool CaculateCross(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 res)
    {
        res = Vector3.zero;
        float area_abc = (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);

        float area_abd = (a.x - d.x) * (b.y - d.y) - (a.y - d.y) * (b.x - d.x);

        if (area_abc * area_abd >= 0)
            return false;

        float area_cda = (c.x - a.x) * (d.y - a.y) - (c.y - a.y) * (d.x - a.x);
        float area_cdb = area_cda + area_abc - area_abd;
        if (area_cda * area_cdb >= 0)
            return false;

        float t = area_cda / (area_abd - area_abc);
        float dx = t * (b.x - a.x);
        float dy = t * (b.y - a.y);
        res = new Vector3(a.x + dx, a.y + dy, 0);
        return true;
    }



}






/*  private bool CaculateCross(Vector3[] lineIn, Vector3[] lineOut, out Vector3 res)
  {
      //判断平行
      float k1 = (lineIn[0].y - lineIn[1].y) / (lineIn[0].x - lineIn[1].x);
      float k2 = (lineOut[0].y - lineOut[1].y) / (lineOut[0].x - lineOut[1].x);
      res = Vector3.zero;
      if (k1 == k2)
      {
          print("pingxing");
          return false;
      }
      else
      {
          float b1 = lineIn[0].y - k1 * lineIn[0].x;
          float b2 = lineOut[0].y - k2 * lineOut[0].x;
          if (b1 == b2)
          {
              print("tongyitiaozhixian");
              return false;
          }
          else
          {
              Vector3 point = new Vector3();
              point.x = (b2 - b1) / (k1 - k2);
              point.y = k1 * ((b2 - b1) / (k1 - k2)) + b1;
              //判断范围
              if (InRange(point, lineIn, lineOut))
              {
                  res = point;
                  return true;
              }
          }
      }
      return false;
  }
  */


/*  /// <summary>
  /// 判断一个点在不在两条线段的范围内
  /// </summary>
  /// <param name="value">计算出来的交点</param>
  /// <param name="lineIn">第一条直线</param>
  /// <param name="lineOut">第二条直线</param>
  /// <returns></returns>
  private bool InRange(Vector3 value, Vector3[] lineIn, Vector3[] lineOut)
  {
      if (IsInRange(value.x, lineIn[0].x, lineIn[1].x) && IsInRange(value.x, lineOut[0].x, lineOut[1].x) && IsInRange(value.y, lineIn[0].y, lineIn[1].y) && IsInRange(value.y, lineIn[0].y, lineIn[1].y)
      )
      {
          return true;
      }
      else
      {
          return false;
      }
  }


  //判断一个点在不在一个一维坐标内
  private bool IsInRange(float value, float r1, float r2)
  {
      if (r1 > r2)
      {
          if (value > r2 && value < r1)
              return true;
      }
      else
      {
          if (value < r2 && value > r1)
              return true;
      }
      return false;
  }
}
*/
