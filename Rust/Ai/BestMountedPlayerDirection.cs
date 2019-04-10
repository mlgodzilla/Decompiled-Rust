// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BestMountedPlayerDirection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class BestMountedPlayerDirection : OptionScorerBase<BasePlayer>
  {
    [ApexSerialization]
    private float score;

    public virtual float Score(IAIContext context, BasePlayer option)
    {
      PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
      if (playerTargetContext != null)
      {
        BasePlayer self = playerTargetContext.Self as BasePlayer;
        Vector3 dir;
        float dot;
        if (Object.op_Implicit((Object) self) && self.isMounted && BestMountedPlayerDirection.Evaluate(self, option.ServerPosition, out dir, out dot))
        {
          playerTargetContext.Direction[playerTargetContext.CurrentOptionsIndex] = dir;
          playerTargetContext.Dot[playerTargetContext.CurrentOptionsIndex] = dot;
          return (float) (((double) dot + 1.0) * 0.5) * this.score;
        }
      }
      playerTargetContext.Direction[playerTargetContext.CurrentOptionsIndex] = Vector3.get_zero();
      playerTargetContext.Dot[playerTargetContext.CurrentOptionsIndex] = 0.0f;
      return 0.0f;
    }

    public static bool Evaluate(
      BasePlayer self,
      Vector3 optionPosition,
      out Vector3 dir,
      out float dot)
    {
      BaseMountable mounted = self.GetMounted();
      ref Vector3 local = ref dir;
      Vector3 vector3 = Vector3.op_Subtraction(optionPosition, self.ServerPosition);
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      local = normalized;
      dot = Vector3.Dot(dir, ((Component) mounted).get_transform().get_forward());
      if ((double) dot >= -0.100000001490116)
        return true;
      dot = -1f;
      return false;
    }

    public BestMountedPlayerDirection()
    {
      base.\u002Ector();
    }
  }
}
