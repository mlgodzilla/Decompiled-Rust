// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Murderer.MurdererMemory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Rust.Ai.HTN.Murderer
{
  [Serializable]
  public class MurdererMemory : BaseNpcMemory
  {
    [NonSerialized]
    public MurdererContext MurdererContext;
    public Vector3 CachedPreferredDistanceDestination;
    public float CachedPreferredDistanceDestinationTime;
    public Vector3 CachedRoamDestination;
    public float CachedRoamDestinationTime;

    public override BaseNpcDefinition Definition
    {
      get
      {
        return this.MurdererContext.Body.AiDefinition;
      }
    }

    public MurdererMemory(MurdererContext context)
      : base((BaseNpcContext) context)
    {
      this.MurdererContext = context;
    }

    public override void ResetState()
    {
      base.ResetState();
    }

    protected override void OnSetPrimaryKnownEnemyPlayer(ref BaseNpcMemory.EnemyPlayerInfo info)
    {
      base.OnSetPrimaryKnownEnemyPlayer(ref info);
      Vector3 vector3 = Vector3.op_Subtraction(info.LastKnownPosition, this.MurdererContext.BodyPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 1.0)
        return;
      this.MurdererContext.HasVisitedLastKnownEnemyPlayerLocation = false;
    }
  }
}
