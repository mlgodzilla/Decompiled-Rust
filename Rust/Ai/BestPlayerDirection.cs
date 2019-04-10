// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BestPlayerDirection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class BestPlayerDirection : OptionScorerBase<BasePlayer>
  {
    [ApexSerialization]
    private float score;

    public virtual float Score(IAIContext context, BasePlayer option)
    {
      PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
      Vector3 dir;
      float dot;
      if (playerTargetContext != null && BestPlayerDirection.Evaluate(playerTargetContext.Self, option.ServerPosition, out dir, out dot))
      {
        playerTargetContext.Direction[playerTargetContext.CurrentOptionsIndex] = dir;
        playerTargetContext.Dot[playerTargetContext.CurrentOptionsIndex] = dot;
        return (float) (((double) dot + 1.0) * 0.5) * this.score;
      }
      playerTargetContext.Direction[playerTargetContext.CurrentOptionsIndex] = Vector3.get_zero();
      playerTargetContext.Dot[playerTargetContext.CurrentOptionsIndex] = -1f;
      return 0.0f;
    }

    public static bool Evaluate(
      IAIAgent self,
      Vector3 optionPosition,
      out Vector3 dir,
      out float dot)
    {
      dir = Vector3.op_Subtraction(optionPosition, self.Entity.ServerPosition);
      NPCPlayerApex npcPlayerApex = self as NPCPlayerApex;
      if (Object.op_Inequality((Object) npcPlayerApex, (Object) null))
      {
        if (npcPlayerApex.ToEnemyRangeEnum(((Vector3) ref dir).get_sqrMagnitude()) == NPCPlayerApex.EnemyRangeEnum.CloseAttackRange)
        {
          dot = 1f;
          ((Vector3) ref dir).Normalize();
          return true;
        }
        ((Vector3) ref dir).Normalize();
        dot = Vector3.Dot(dir, npcPlayerApex.eyes.BodyForward());
        if ((double) dot < (double) self.GetStats.VisionCone)
        {
          dot = -1f;
          return false;
        }
      }
      else
      {
        ((Vector3) ref dir).Normalize();
        dot = Vector3.Dot(dir, ((Component) self.Entity).get_transform().get_forward());
        if ((double) dot < (double) self.GetStats.VisionCone)
        {
          dot = -1f;
          return false;
        }
      }
      return true;
    }

    public BestPlayerDirection()
    {
      base.\u002Ector();
    }
  }
}
