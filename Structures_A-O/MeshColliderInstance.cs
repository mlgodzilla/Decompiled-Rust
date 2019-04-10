// Decompiled with JetBrains decompiler
// Type: MeshColliderInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct MeshColliderInstance
{
  public Transform transform;
  public Rigidbody rigidbody;
  public Collider collider;
  public OBB bounds;
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
