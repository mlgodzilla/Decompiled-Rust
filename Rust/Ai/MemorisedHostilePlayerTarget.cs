// Decompiled with JetBrains decompiler
// Type: Rust.Ai.MemorisedHostilePlayerTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
  public class MemorisedHostilePlayerTarget : ActionBase<PlayerTargetContext>
  {
    [ApexSerialization]
    public float HostilityTimeout;

    public virtual void Execute(PlayerTargetContext context)
    {
      BaseContext context1 = context.Self.GetContext(Guid.Empty) as BaseContext;
      if (context1 == null)
        return;
      float num1 = 0.0f;
      BasePlayer basePlayer = (BasePlayer) null;
      Vector3 vector3_1 = Vector3.get_zero();
      float num2 = context1.AIAgent.GetStats.AggressionRange * context1.AIAgent.GetStats.AggressionRange;
      for (int index = 0; index < context1.Memory.All.Count; ++index)
      {
        Memory.SeenInfo seenInfo = context1.Memory.All[index];
        BasePlayer entity = seenInfo.Entity as BasePlayer;
        if (Object.op_Inequality((Object) entity, (Object) null) && (double) Time.get_time() < (double) context1.Memory.GetExtendedInfo(seenInfo.Entity).LastHurtUsTime + (double) this.HostilityTimeout && (double) seenInfo.Danger > (double) num1)
        {
          Vector3 vector3_2 = Vector3.op_Subtraction(seenInfo.Position, context1.Position);
          if ((double) ((Vector3) ref vector3_2).get_sqrMagnitude() <= (double) num2)
          {
            num1 = seenInfo.Danger;
            basePlayer = entity;
            vector3_1 = seenInfo.Position;
          }
        }
      }
      if (!Object.op_Inequality((Object) basePlayer, (Object) null))
        return;
      context.Target = basePlayer;
      context.Score = num1;
      context.LastKnownPosition = vector3_1;
    }

    public MemorisedHostilePlayerTarget()
    {
      base.\u002Ector();
    }
  }
}
