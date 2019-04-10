// Decompiled with JetBrains decompiler
// Type: VLB.GlobalMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace VLB
{
  public static class GlobalMesh
  {
    private static Mesh ms_Mesh;

    public static Mesh mesh
    {
      get
      {
        if (Object.op_Equality((Object) GlobalMesh.ms_Mesh, (Object) null))
        {
          GlobalMesh.ms_Mesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
          ((Object) GlobalMesh.ms_Mesh).set_hideFlags(Consts.ProceduralObjectsHideFlags);
        }
        return GlobalMesh.ms_Mesh;
      }
    }
  }
}
