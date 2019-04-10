// Decompiled with JetBrains decompiler
// Type: OccludeeSphere
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct OccludeeSphere
{
  public int id;
  public OccludeeState state;
  public OcclusionCulling.Sphere sphere;

  public bool IsRegistered
  {
    get
    {
      return this.id >= 0;
    }
  }

  public void Invalidate()
  {
    this.id = -1;
    this.state = (OccludeeState) null;
    this.sphere = new OcclusionCulling.Sphere();
  }

  public OccludeeSphere(int id)
  {
    this.id = id;
    this.state = id < 0 ? (OccludeeState) null : OcclusionCulling.GetStateById(id);
    this.sphere = new OcclusionCulling.Sphere(Vector3.get_zero(), 0.0f);
  }

  public OccludeeSphere(int id, OcclusionCulling.Sphere sphere)
  {
    this.id = id;
    this.state = id < 0 ? (OccludeeState) null : OcclusionCulling.GetStateById(id);
    this.sphere = sphere;
  }
}
