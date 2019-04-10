// Decompiled with JetBrains decompiler
// Type: MeshColliderData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderData
{
  public List<int> triangles;
  public List<Vector3> vertices;
  public List<Vector3> normals;

  public void Alloc()
  {
    if (this.triangles == null)
      this.triangles = (List<int>) Pool.GetList<int>();
    if (this.vertices == null)
      this.vertices = (List<Vector3>) Pool.GetList<Vector3>();
    if (this.normals != null)
      return;
    this.normals = (List<Vector3>) Pool.GetList<Vector3>();
  }

  public void Free()
  {
    if (this.triangles != null)
    {
      // ISSUE: cast to a reference type
      Pool.FreeList<int>((List<M0>&) ref this.triangles);
    }
    if (this.vertices != null)
    {
      // ISSUE: cast to a reference type
      Pool.FreeList<Vector3>((List<M0>&) ref this.vertices);
    }
    if (this.normals == null)
      return;
    // ISSUE: cast to a reference type
    Pool.FreeList<Vector3>((List<M0>&) ref this.normals);
  }

  public void Clear()
  {
    if (this.triangles != null)
      this.triangles.Clear();
    if (this.vertices != null)
      this.vertices.Clear();
    if (this.normals == null)
      return;
    this.normals.Clear();
  }

  public void Apply(Mesh mesh)
  {
    mesh.Clear();
    if (this.vertices != null)
      mesh.SetVertices(this.vertices);
    if (this.triangles != null)
      mesh.SetTriangles(this.triangles, 0);
    if (this.normals == null)
      return;
    if (this.normals.Count == this.vertices.Count)
    {
      mesh.SetNormals(this.normals);
    }
    else
    {
      if (this.normals.Count <= 0 || Batching.verbose <= 0)
        return;
      Debug.LogWarning((object) "Skipping collider normals because some meshes were missing them.");
    }
  }

  public void Combine(MeshColliderGroup meshGroup)
  {
    for (int index1 = 0; index1 < meshGroup.data.Count; ++index1)
    {
      MeshColliderInstance colliderInstance = meshGroup.data[index1];
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(colliderInstance.position, colliderInstance.rotation, colliderInstance.scale);
      int count = this.vertices.Count;
      for (int index2 = 0; index2 < colliderInstance.data.triangles.Length; ++index2)
        this.triangles.Add(count + colliderInstance.data.triangles[index2]);
      for (int index2 = 0; index2 < colliderInstance.data.vertices.Length; ++index2)
        this.vertices.Add(((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(colliderInstance.data.vertices[index2]));
      for (int index2 = 0; index2 < colliderInstance.data.normals.Length; ++index2)
        this.normals.Add(((Matrix4x4) ref matrix4x4).MultiplyVector(colliderInstance.data.normals[index2]));
    }
  }

  public void Combine(MeshColliderGroup meshGroup, MeshColliderLookup colliderLookup)
  {
    for (int index1 = 0; index1 < meshGroup.data.Count; ++index1)
    {
      MeshColliderInstance instance = meshGroup.data[index1];
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(instance.position, instance.rotation, instance.scale);
      int count = this.vertices.Count;
      for (int index2 = 0; index2 < instance.data.triangles.Length; ++index2)
        this.triangles.Add(count + instance.data.triangles[index2]);
      for (int index2 = 0; index2 < instance.data.vertices.Length; ++index2)
        this.vertices.Add(((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(instance.data.vertices[index2]));
      for (int index2 = 0; index2 < instance.data.normals.Length; ++index2)
        this.normals.Add(((Matrix4x4) ref matrix4x4).MultiplyVector(instance.data.normals[index2]));
      colliderLookup.Add(instance);
    }
  }
}
