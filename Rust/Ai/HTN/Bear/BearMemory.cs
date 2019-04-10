// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.BearMemory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Rust.Ai.HTN.Bear
{
  public class BearMemory : BaseNpcMemory
  {
    [NonSerialized]
    public BearContext BearContext;
    public Vector3 CachedPreferredDistanceDestination;
    public float CachedPreferredDistanceDestinationTime;

    public override BaseNpcDefinition Definition
    {
      get
      {
        return this.BearContext.Body.AiDefinition;
      }
    }

    public BearMemory(BearContext context)
      : base((BaseNpcContext) context)
    {
      this.BearContext = context;
    }

    public override void ResetState()
    {
      base.ResetState();
    }

    protected override void OnSetPrimaryKnownEnemyPlayer(ref BaseNpcMemory.EnemyPlayerInfo info)
    {
      base.OnSetPrimaryKnownEnemyPlayer(ref info);
      Vector3 vector3 = Vector3.op_Subtraction(info.LastKnownPosition, ((Component) this.BearContext.Body).get_transform().get_position());
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 1.0)
        return;
      this.BearContext.HasVisitedLastKnownEnemyPlayerLocation = false;
    }
  }
}
