// Decompiled with JetBrains decompiler
// Type: AsyncTerrainNavMeshBake
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AsyncTerrainNavMeshBake : CustomYieldInstruction
{
  private List<int> indices;
  private List<Vector3> vertices;
  private List<Vector3> normals;
  private List<int> triangles;
  private Vector3 pivot;
  private int width;
  private int height;
  private bool normal;
  private bool alpha;
  private Action worker;

  public virtual bool keepWaiting
  {
    get
    {
      return this.worker != null;
    }
  }

  public bool isDone
  {
    get
    {
      return this.worker == null;
    }
  }

  public NavMeshBuildSource CreateNavMeshBuildSource(bool addSourceObject)
  {
    NavMeshBuildSource navMeshBuildSource = (NavMeshBuildSource) null;
    ((NavMeshBuildSource) ref navMeshBuildSource).set_transform(Matrix4x4.TRS(this.pivot, Quaternion.get_identity(), Vector3.get_one()));
    ((NavMeshBuildSource) ref navMeshBuildSource).set_shape((NavMeshBuildSourceShape) 0);
    if (addSourceObject)
      ((NavMeshBuildSource) ref navMeshBuildSource).set_sourceObject((Object) this.mesh);
    return navMeshBuildSource;
  }

  public Mesh mesh
  {
    get
    {
      Mesh mesh = new Mesh();
      if (this.vertices != null)
      {
        mesh.SetVertices(this.vertices);
        Pool.FreeList<Vector3>((List<M0>&) ref this.vertices);
      }
      if (this.normals != null)
      {
        mesh.SetNormals(this.normals);
        Pool.FreeList<Vector3>((List<M0>&) ref this.normals);
      }
      if (this.triangles != null)
      {
        mesh.SetTriangles(this.triangles, 0);
        Pool.FreeList<int>((List<M0>&) ref this.triangles);
      }
      if (this.indices != null)
        Pool.FreeList<int>((List<M0>&) ref this.indices);
      return mesh;
    }
  }

  public AsyncTerrainNavMeshBake(Vector3 pivot, int width, int height, bool normal, bool alpha)
  {
    this.\u002Ector();
    this.pivot = pivot;
    this.width = width;
    this.height = height;
    this.normal = normal;
    this.alpha = alpha;
    this.indices = (List<int>) Pool.GetList<int>();
    this.vertices = (List<Vector3>) Pool.GetList<Vector3>();
    this.normals = normal ? (List<Vector3>) Pool.GetList<Vector3>() : (List<Vector3>) null;
    this.triangles = (List<int>) Pool.GetList<int>();
    this.Invoke();
  }

  private void DoWork()
  {
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector((float) (this.width / 2), 0.0f, (float) (this.height / 2));
    Vector3 vector3_2;
    ((Vector3) ref vector3_2).\u002Ector((float) (this.pivot.x - vector3_1.x), 0.0f, (float) (this.pivot.z - vector3_1.z));
    TerrainHeightMap heightMap = TerrainMeta.HeightMap;
    TerrainAlphaMap alphaMap = TerrainMeta.AlphaMap;
    int num1 = 0;
    for (int index = 0; index <= this.height; ++index)
    {
      int num2 = 0;
      while (num2 <= this.width)
      {
        Vector3 worldPos = Vector3.op_Addition(new Vector3((float) num2, 0.0f, (float) index), vector3_2);
        Vector3 vector3_3 = Vector3.op_Subtraction(new Vector3((float) num2, 0.0f, (float) index), vector3_1);
        float height = heightMap.GetHeight(worldPos);
        if ((double) height < -1.0)
          this.indices.Add(-1);
        else if (this.alpha && (double) alphaMap.GetAlpha(worldPos) < 0.100000001490116)
        {
          this.indices.Add(-1);
        }
        else
        {
          if (this.normal)
            this.normals.Add(heightMap.GetNormal(worldPos));
          worldPos.y = (__Null) (double) (vector3_3.y = (__Null) (height - (float) this.pivot.y));
          this.indices.Add(this.vertices.Count);
          this.vertices.Add(vector3_3);
        }
        ++num2;
        ++num1;
      }
    }
    int index1 = 0;
    int num3 = 0;
    while (num3 < this.height)
    {
      int num2 = 0;
      while (num2 < this.width)
      {
        int index2 = this.indices[index1];
        int index3 = this.indices[index1 + this.width + 1];
        int index4 = this.indices[index1 + 1];
        int index5 = this.indices[index1 + 1];
        int index6 = this.indices[index1 + this.width + 1];
        int index7 = this.indices[index1 + this.width + 2];
        if (index2 != -1 && index3 != -1 && index4 != -1)
        {
          this.triangles.Add(index2);
          this.triangles.Add(index3);
          this.triangles.Add(index4);
        }
        if (index5 != -1 && index6 != -1 && index7 != -1)
        {
          this.triangles.Add(index5);
          this.triangles.Add(index6);
          this.triangles.Add(index7);
        }
        ++num2;
        ++index1;
      }
      ++num3;
      ++index1;
    }
  }

  private void Invoke()
  {
    this.worker = new Action(this.DoWork);
    this.worker.BeginInvoke(new AsyncCallback(this.Callback), (object) null);
  }

  private void Callback(IAsyncResult result)
  {
    this.worker.EndInvoke(result);
    this.worker = (Action) null;
  }
}
