// Decompiled with JetBrains decompiler
// Type: RHIBAIController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class RHIBAIController : FacepunchBehaviour
{
  public List<Vector3> nodes;

  [ContextMenu("Calculate Path")]
  public void SetupPatrolPath()
  {
    // ISSUE: variable of the null type
    __Null x = TerrainMeta.Size.x;
    int num1 = Mathf.CeilToInt((float) (x * 2.0 * 3.14159274101257) / 30f);
    this.nodes = new List<Vector3>();
    float num2 = (float) x;
    float num3 = 0.0f;
    for (int index = 0; index < num1; ++index)
    {
      float num4 = (float) ((double) index / (double) num1 * 360.0);
      this.nodes.Add(new Vector3(Mathf.Sin(num4 * ((float) Math.PI / 180f)) * num2, num3, Mathf.Cos(num4 * ((float) Math.PI / 180f)) * num2));
    }
    float num5 = 2f;
    float num6 = 200f;
    float num7 = 150f;
    float num8 = 8f;
    bool flag1 = true;
    int num9 = 1;
    float num10 = 20f;
    Vector3[] vector3Array = new Vector3[5]
    {
      new Vector3(0.0f, 0.0f, 0.0f),
      new Vector3(num10, 0.0f, 0.0f),
      new Vector3(-num10, 0.0f, 0.0f),
      new Vector3(0.0f, 0.0f, num10),
      new Vector3(0.0f, 0.0f, -num10)
    };
    while (flag1)
    {
      Debug.Log((object) ("Loop # :" + (object) num9));
      ++num9;
      flag1 = false;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        Vector3 node1 = this.nodes[index1];
        int index2 = index1 == 0 ? num1 - 1 : index1 - 1;
        Vector3 node2 = this.nodes[index1 == num1 - 1 ? 0 : index1 + 1];
        Vector3 node3 = this.nodes[index2];
        Vector3 vector3_1 = node1;
        Vector3 vector3_2 = Vector3.op_Subtraction(Vector3.get_zero(), node1);
        Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
        Vector3 vector3_3 = Vector3.op_Addition(node1, Vector3.op_Multiply(normalized, num5));
        if ((double) Vector3.Distance(vector3_3, node2) <= (double) num6 && (double) Vector3.Distance(vector3_3, node3) <= (double) num6)
        {
          bool flag2 = true;
          for (int index3 = 0; index3 < vector3Array.Length; ++index3)
          {
            Vector3 pos = Vector3.op_Addition(vector3_3, vector3Array[index3]);
            if ((double) this.GetWaterDepth(pos) < (double) num8)
              flag2 = false;
            Vector3 vector3_4 = normalized;
            if (Vector3.op_Inequality(pos, Vector3.get_zero()))
            {
              vector3_2 = Vector3.op_Subtraction(pos, vector3_1);
              vector3_4 = ((Vector3) ref vector3_2).get_normalized();
            }
            RaycastHit raycastHit;
            if (Physics.Raycast(vector3_1, vector3_4, ref raycastHit, num7, 1218511105))
              flag2 = false;
          }
          if (flag2)
          {
            flag1 = true;
            this.nodes[index1] = vector3_3;
          }
        }
      }
    }
    List<int> intList = new List<int>();
    LineUtility.Simplify(this.nodes, 15f, intList);
    List<Vector3> nodes = this.nodes;
    this.nodes = new List<Vector3>();
    foreach (int index in intList)
      this.nodes.Add(nodes[index]);
  }

  public float GetWaterDepth(Vector3 pos)
  {
    RaycastHit raycastHit;
    if (!Physics.Raycast(pos, Vector3.get_down(), ref raycastHit, 100f, 8388608))
      return 100f;
    return ((RaycastHit) ref raycastHit).get_distance();
  }

  public void OnDrawGizmosSelected()
  {
    if (TerrainMeta.Path.OceanPatrolClose == null)
      return;
    for (int index = 0; index < TerrainMeta.Path.OceanPatrolClose.Count; ++index)
    {
      Vector3 vector3 = TerrainMeta.Path.OceanPatrolClose[index];
      Gizmos.set_color(Color.get_green());
      Gizmos.DrawSphere(vector3, 3f);
      Gizmos.DrawLine(vector3, index + 1 == TerrainMeta.Path.OceanPatrolClose.Count ? TerrainMeta.Path.OceanPatrolClose[0] : TerrainMeta.Path.OceanPatrolClose[index + 1]);
    }
  }

  public RHIBAIController()
  {
    base.\u002Ector();
  }
}
