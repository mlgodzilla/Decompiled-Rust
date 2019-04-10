// Decompiled with JetBrains decompiler
// Type: RealmedCollider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RealmedCollider : BasePrefab
{
  public Collider ServerCollider;
  public Collider ClientCollider;

  public override void PreProcess(
    IPrefabProcessor process,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
    if (Object.op_Inequality((Object) this.ServerCollider, (Object) this.ClientCollider))
    {
      if (clientside)
      {
        if (Object.op_Implicit((Object) this.ServerCollider))
        {
          process.RemoveComponent((Component) this.ServerCollider);
          this.ServerCollider = (Collider) null;
        }
      }
      else if (Object.op_Implicit((Object) this.ClientCollider))
      {
        process.RemoveComponent((Component) this.ClientCollider);
        this.ClientCollider = (Collider) null;
      }
    }
    process.RemoveComponent((Component) this);
  }
}
