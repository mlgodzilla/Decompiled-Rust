// Decompiled with JetBrains decompiler
// Type: NPCShopKeeper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCShopKeeper : NPCPlayer
{
  private float greetDir;
  private Vector3 initialFacingDir;
  private BasePlayer lastWavedAtPlayer;

  public void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_green());
    Gizmos.DrawCube(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 1f)), new Vector3(0.5f, 1f, 0.5f));
  }

  public override void UpdateProtectionFromClothing()
  {
  }

  public override void Hurt(HitInfo info)
  {
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.initialFacingDir = Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), Vector3.get_forward());
    this.Invoke(new Action(this.DelayedSleepEnd), 3f);
    this.SetAimDirection(Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), Vector3.get_forward()));
    this.InvokeRandomized(new Action(this.Greeting), Random.Range(5f, 10f), 5f, Random.Range(0.0f, 2f));
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
  }

  public void DelayedSleepEnd()
  {
    this.EndSleeping();
  }

  public void Greeting()
  {
    List<BasePlayer> list = (List<BasePlayer>) Pool.GetList<BasePlayer>();
    Vis.Entities<BasePlayer>(((Component) this).get_transform().get_position(), 10f, list, 131072, (QueryTriggerInteraction) 2);
    ((Component) this).get_transform().get_position();
    BasePlayer basePlayer1 = (BasePlayer) null;
    foreach (BasePlayer basePlayer2 in list)
    {
      if (!basePlayer2.isClient && !basePlayer2.IsNpc && (!Object.op_Equality((Object) basePlayer2, (Object) this) && basePlayer2.IsVisible(this.eyes.position, float.PositiveInfinity)) && (!Object.op_Equality((Object) basePlayer2, (Object) this.lastWavedAtPlayer) && (double) Vector3.Dot(Vector3Ex.Direction2D(basePlayer2.eyes.position, this.eyes.position), this.initialFacingDir) >= 0.200000002980232))
      {
        basePlayer1 = basePlayer2;
        break;
      }
    }
    if (Object.op_Equality((Object) basePlayer1, (Object) null) && !list.Contains(this.lastWavedAtPlayer))
      this.lastWavedAtPlayer = (BasePlayer) null;
    if (Object.op_Inequality((Object) basePlayer1, (Object) null))
    {
      this.SignalBroadcast(BaseEntity.Signal.Gesture, "wave", (Connection) null);
      this.SetAimDirection(Vector3Ex.Direction2D(basePlayer1.eyes.position, this.eyes.position));
      this.lastWavedAtPlayer = basePlayer1;
    }
    else
      this.SetAimDirection(this.initialFacingDir);
    // ISSUE: cast to a reference type
    Pool.FreeList<BasePlayer>((List<M0>&) ref list);
  }
}
