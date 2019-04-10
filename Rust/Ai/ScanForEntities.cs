// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ScanForEntities
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
  [FriendlyName("Scan for Entities", "Update Context.Entities")]
  public sealed class ScanForEntities : BaseAction
  {
    public BaseEntity[] Results = new BaseEntity[64];
    [ApexSerialization]
    public int forgetTime = 10;

    public override void DoExecute(BaseContext c)
    {
      if (BaseEntity.Query.Server == null)
        return;
      int inSphere = BaseEntity.Query.Server.GetInSphere(c.Position, c.AIAgent.GetStats.VisionRange, this.Results, new Func<BaseEntity, bool>(ScanForEntities.AiCaresAbout));
      if (inSphere == 0)
        return;
      for (int index = 0; index < inSphere; ++index)
      {
        BaseEntity result = this.Results[index];
        if (!Object.op_Equality((Object) result, (Object) null) && !Object.op_Equality((Object) result, (Object) c.Entity) && (result.isServer && ScanForEntities.WithinVisionCone(c.AIAgent, result)))
        {
          BasePlayer basePlayer = result as BasePlayer;
          if (Object.op_Inequality((Object) basePlayer, (Object) null) && !result.IsNpc)
          {
            if (!ConVar.AI.ignoreplayers)
            {
              Vector3 attackPosition = c.AIAgent.AttackPosition;
              if ((basePlayer.IsVisible(attackPosition, basePlayer.CenterPoint(), float.PositiveInfinity) || basePlayer.IsVisible(attackPosition, basePlayer.eyes.position, float.PositiveInfinity) ? 1 : (basePlayer.IsVisible(attackPosition, ((Component) basePlayer).get_transform().get_position(), float.PositiveInfinity) ? 1 : 0)) == 0)
                continue;
            }
            else
              continue;
          }
          c.Memory.Update(result, 0.0f);
        }
      }
      c.Memory.Forget((float) this.forgetTime);
    }

    private static bool WithinVisionCone(IAIAgent agent, BaseEntity other)
    {
      if ((double) agent.GetStats.VisionCone == -1.0)
        return true;
      BaseCombatEntity entity = agent.Entity;
      ((Component) entity).get_transform().get_forward();
      BasePlayer basePlayer = entity as BasePlayer;
      if (Object.op_Inequality((Object) basePlayer, (Object) null))
        basePlayer.eyes.BodyForward();
      Vector3 vector3 = Vector3.op_Subtraction(((Component) other).get_transform().get_position(), ((Component) entity).get_transform().get_position());
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      return (double) Vector3.Dot(((Component) entity).get_transform().get_forward(), normalized) >= (double) agent.GetStats.VisionCone;
    }

    private static bool AiCaresAbout(BaseEntity ent)
    {
      return ent is BasePlayer || ent is BaseNpc || (ent is WorldItem || ent is BaseCorpse);
    }
  }
}
