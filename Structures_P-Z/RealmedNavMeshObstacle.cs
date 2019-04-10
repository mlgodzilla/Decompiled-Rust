// Decompiled with JetBrains decompiler
// Type: RealmedNavMeshObstacle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

public class RealmedNavMeshObstacle : BasePrefab
{
  public NavMeshObstacle Obstacle;

  public override void PreProcess(
    IPrefabProcessor process,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    if (bundling)
      return;
    base.PreProcess(process, rootObj, name, serverside, clientside, false);
    if (this.isServer && Object.op_Implicit((Object) this.Obstacle))
    {
      if (AiManager.nav_disable)
      {
        process.RemoveComponent((Component) this.Obstacle);
        this.Obstacle = (NavMeshObstacle) null;
      }
      else if (AiManager.nav_obstacles_carve_state >= 2)
        this.Obstacle.set_carving(true);
      else if (AiManager.nav_obstacles_carve_state == 1)
        this.Obstacle.set_carving(((Component) this.Obstacle).get_gameObject().get_layer() == 21);
      else
        this.Obstacle.set_carving(false);
    }
    process.RemoveComponent((Component) this);
  }
}
