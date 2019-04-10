// Decompiled with JetBrains decompiler
// Type: MeshRendererData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererData
{
  public List<int> triangles;
  public List<Vector3> vertices;
  public List<Vector3> normals;
  public List<Vector4> tangents;
  public List<Color32> colors32;
  public List<Vector2> uv;
  public List<Vector2> uv2;
  public List<Vector4> positions;

  public void Alloc()
  {
    if (this.triangles == null)
      this.triangles = (List<int>) Pool.GetList<int>();
    if (this.vertices == null)
      this.vertices = (List<Vector3>) Pool.GetList<Vector3>();
    if (this.normals == null)
      this.normals = (List<Vector3>) Pool.GetList<Vector3>();
    if (this.tangents == null)
      this.tangents = (List<Vector4>) Pool.GetList<Vector4>();
    if (this.colors32 == null)
      this.colors32 = (List<Color32>) Pool.GetList<Color32>();
    if (this.uv == null)
      this.uv = (List<Vector2>) Pool.GetList<Vector2>();
    if (this.uv2 == null)
      this.uv2 = (List<Vector2>) Pool.GetList<Vector2>();
    if (this.positions != null)
      return;
    this.positions = (List<Vector4>) Pool.GetList<Vector4>();
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
    if (this.normals != null)
    {
      // ISSUE: cast to a reference type
      Pool.FreeList<Vector3>((List<M0>&) ref this.normals);
    }
    if (this.tangents != null)
    {
      // ISSUE: cast to a reference type
      Pool.FreeList<Vector4>((List<M0>&) ref this.tangents);
    }
    if (this.colors32 != null)
    {
      // ISSUE: cast to a reference type
      Pool.FreeList<Color32>((List<M0>&) ref this.colors32);
    }
    if (this.uv != null)
    {
      // ISSUE: cast to a reference type
      Pool.FreeList<Vector2>((List<M0>&) ref this.uv);
    }
    if (this.uv2 != null)
    {
      // ISSUE: cast to a reference type
      Pool.FreeList<Vector2>((List<M0>&) ref this.uv2);
    }
    if (this.positions == null)
      return;
    // ISSUE: cast to a reference type
    Pool.FreeList<Vector4>((List<M0>&) ref this.positions);
  }

  public void Clear()
  {
    if (this.triangles != null)
      this.triangles.Clear();
    if (this.vertices != null)
      this.vertices.Clear();
    if (this.normals != null)
      this.normals.Clear();
    if (this.tangents != null)
      this.tangents.Clear();
    if (this.colors32 != null)
      this.colors32.Clear();
    if (this.uv != null)
      this.uv.Clear();
    if (this.uv2 != null)
      this.uv2.Clear();
    if (this.positions == null)
      return;
    this.positions.Clear();
  }

  public void Apply(Mesh mesh)
  {
    mesh.Clear();
    if (this.vertices != null)
      mesh.SetVertices(this.vertices);
    if (this.triangles != null)
      mesh.SetTriangles(this.triangles, 0);
    if (this.normals != null)
    {
      if (this.normals.Count == this.vertices.Count)
        mesh.SetNormals(this.normals);
      else if (this.normals.Count > 0 && Batching.verbose > 0)
        Debug.LogWarning((object) "Skipping renderer normals because some meshes were missing them.");
    }
    if (this.tangents != null)
    {
      if (this.tangents.Count == this.vertices.Count)
        mesh.SetTangents(this.tangents);
      else if (this.tangents.Count > 0 && Batching.verbose > 0)
        Debug.LogWarning((object) "Skipping renderer tangents because some meshes were missing them.");
    }
    if (this.colors32 != null)
    {
      if (this.colors32.Count == this.vertices.Count)
        mesh.SetColors(this.colors32);
      else if (this.colors32.Count > 0 && Batching.verbose > 0)
        Debug.LogWarning((object) "Skipping renderer colors because some meshes were missing them.");
    }
    if (this.uv != null)
    {
      if (this.uv.Count == this.vertices.Count)
        mesh.SetUVs(0, this.uv);
      else if (this.uv.Count > 0 && Batching.verbose > 0)
        Debug.LogWarning((object) "Skipping renderer uvs because some meshes were missing them.");
    }
    if (this.uv2 != null)
    {
      if (this.uv2.Count == this.vertices.Count)
        mesh.SetUVs(1, this.uv2);
      else if (this.uv2.Count > 0 && Batching.verbose > 0)
        Debug.LogWarning((object) "Skipping renderer uv2s because some meshes were missing them.");
    }
    if (this.positions == null)
      return;
    mesh.SetUVs(2, this.positions);
  }

  public void Combine(MeshRendererGroup meshGroup)
  {
    for (int index1 = 0; index1 < meshGroup.data.Count; ++index1)
    {
      MeshRendererInstance rendererInstance = meshGroup.data[index1];
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(rendererInstance.position, rendererInstance.rotation, rendererInstance.scale);
      int count = this.vertices.Count;
      for (int index2 = 0; index2 < rendererInstance.data.triangles.Length; ++index2)
        this.triangles.Add(count + rendererInstance.data.triangles[index2]);
      for (int index2 = 0; index2 < rendererInstance.data.vertices.Length; ++index2)
      {
        this.vertices.Add(((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(rendererInstance.data.vertices[index2]));
        this.positions.Add(Vector4.op_Implicit(rendererInstance.position));
      }
      for (int index2 = 0; index2 < rendererInstance.data.normals.Length; ++index2)
        this.normals.Add(((Matrix4x4) ref matrix4x4).MultiplyVector(rendererInstance.data.normals[index2]));
      for (int index2 = 0; index2 < rendererInstance.data.tangents.Length; ++index2)
      {
        Vector4 tangent = rendererInstance.data.tangents[index2];
        Vector3 vector3_1;
        ((Vector3) ref vector3_1).\u002Ector((float) tangent.x, (float) tangent.y, (float) tangent.z);
        Vector3 vector3_2 = ((Matrix4x4) ref matrix4x4).MultiplyVector(vector3_1);
        this.tangents.Add(new Vector4((float) vector3_2.x, (float) vector3_2.y, (float) vector3_2.z, (float) tangent.w));
      }
      for (int index2 = 0; index2 < rendererInstance.data.colors32.Length; ++index2)
        this.colors32.Add(rendererInstance.data.colors32[index2]);
      for (int index2 = 0; index2 < rendererInstance.data.uv.Length; ++index2)
        this.uv.Add(rendererInstance.data.uv[index2]);
      for (int index2 = 0; index2 < rendererInstance.data.uv2.Length; ++index2)
        this.uv2.Add(rendererInstance.data.uv2[index2]);
    }
  }

  public void Combine(MeshRendererGroup meshGroup, MeshRendererLookup rendererLookup)
  {
    for (int index1 = 0; index1 < meshGroup.data.Count; ++index1)
    {
      MeshRendererInstance instance = meshGroup.data[index1];
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(instance.position, instance.rotation, instance.scale);
      int count = this.vertices.Count;
      for (int index2 = 0; index2 < instance.data.triangles.Length; ++index2)
        this.triangles.Add(count + instance.data.triangles[index2]);
      for (int index2 = 0; index2 < instance.data.vertices.Length; ++index2)
      {
        this.vertices.Add(((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(instance.data.vertices[index2]));
        this.positions.Add(Vector4.op_Implicit(instance.position));
      }
      for (int index2 = 0; index2 < instance.data.normals.Length; ++index2)
        this.normals.Add(((Matrix4x4) ref matrix4x4).MultiplyVector(instance.data.normals[index2]));
      for (int index2 = 0; index2 < instance.data.tangents.Length; ++index2)
      {
        Vector4 tangent = instance.data.tangents[index2];
        Vector3 vector3_1;
        ((Vector3) ref vector3_1).\u002Ector((float) tangent.x, (float) tangent.y, (float) tangent.z);
        Vector3 vector3_2 = ((Matrix4x4) ref matrix4x4).MultiplyVector(vector3_1);
        this.tangents.Add(new Vector4((float) vector3_2.x, (float) vector3_2.y, (float) vector3_2.z, (float) tangent.w));
      }
      for (int index2 = 0; index2 < instance.data.colors32.Length; ++index2)
        this.colors32.Add(instance.data.colors32[index2]);
      for (int index2 = 0; index2 < instance.data.uv.Length; ++index2)
        this.uv.Add(instance.data.uv[index2]);
      for (int index2 = 0; index2 < instance.data.uv2.Length; ++index2)
        this.uv2.Add(instance.data.uv2[index2]);
      rendererLookup.Add(instance);
    }
  }
}
