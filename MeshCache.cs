// Decompiled with JetBrains decompiler
// Type: MeshCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public static class MeshCache
{
  public static Dictionary<Mesh, MeshCache.Data> dictionary = new Dictionary<Mesh, MeshCache.Data>();

  public static MeshCache.Data Get(Mesh mesh)
  {
    MeshCache.Data data;
    if (!MeshCache.dictionary.TryGetValue(mesh, out data))
    {
      data = new MeshCache.Data();
      data.mesh = mesh;
      data.vertices = mesh.get_vertices();
      data.normals = mesh.get_normals();
      data.tangents = mesh.get_tangents();
      data.colors32 = mesh.get_colors32();
      data.triangles = mesh.get_triangles();
      data.uv = mesh.get_uv();
      data.uv2 = mesh.get_uv2();
      data.uv3 = mesh.get_uv3();
      data.uv4 = mesh.get_uv4();
      MeshCache.dictionary.Add(mesh, data);
    }
    return data;
  }

  [Serializable]
  public class Data
  {
    public Mesh mesh;
    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector4[] tangents;
    public Color32[] colors32;
    public int[] triangles;
    public Vector2[] uv;
    public Vector2[] uv2;
    public Vector2[] uv3;
    public Vector2[] uv4;
  }
}
