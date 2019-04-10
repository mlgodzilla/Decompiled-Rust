// Decompiled with JetBrains decompiler
// Type: MeshInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct MeshInstance
{
  public Vector3 position;
  public Quaternion rotation;
  public Vector3 scale;
  public MeshCache.Data data;

  public Mesh mesh
  {
    get
    {
      return this.data.mesh;
    }
    set
    {
      this.data = MeshCache.Get(value);
    }
  }
}
