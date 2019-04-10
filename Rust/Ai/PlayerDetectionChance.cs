// Decompiled with JetBrains decompiler
// Type: Rust.Ai.PlayerDetectionChance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class PlayerDetectionChance : OptionScorerBase<BasePlayer>
  {
    [ApexSerialization]
    private float score;

    public virtual float Score(IAIContext context, BasePlayer option)
    {
      PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
      if (playerTargetContext != null && PlayerDetectionChance.Evaluate(playerTargetContext.Self, playerTargetContext.Dot[playerTargetContext.CurrentOptionsIndex], option))
        return this.score;
      return 0.0f;
    }

    public static bool Evaluate(IAIAgent self, float dot, BasePlayer option)
    {
      NPCPlayerApex npcPlayerApex = self as NPCPlayerApex;
      if (!Object.op_Inequality((Object) npcPlayerApex, (Object) null))
        return true;
      if ((double) Time.get_time() > (double) npcPlayerApex.NextDetectionCheck)
      {
        npcPlayerApex.NextDetectionCheck = Time.get_time() + 2f;
        bool flag1 = (double) Random.get_value() < (double) PlayerDetectionChance.FovDetection(dot, option);
        bool flag2 = (double) Random.get_value() < (double) PlayerDetectionChance.NoiseLevel(option);
        bool flag3 = (double) Random.get_value() < (double) PlayerDetectionChance.LightDetection(option);
        npcPlayerApex.LastDetectionCheckResult = flag1 | flag2 | flag3;
      }
      return npcPlayerApex.LastDetectionCheckResult;
    }

    private static float FovDetection(float dot, BasePlayer option)
    {
      return (float) (((double) dot >= 0.75 ? 1.5 : ((double) dot + 1.0) * 0.5) * (option.IsRunning() ? 1.5 : 1.0) * (option.IsDucked() ? 0.75 : 1.0));
    }

    private static float NoiseLevel(BasePlayer option)
    {
      float num = (float) ((option.IsDucked() ? 0.5 : 1.0) * (option.IsRunning() ? 1.5 : 1.0) * ((double) option.estimatedSpeed <= 0.00999999977648258 ? 0.100000001490116 : 1.0));
      return option.inventory.containerWear.itemList.Count != 0 ? num + (float) option.inventory.containerWear.itemList.Count * 0.025f : num * 0.1f;
    }

    private static float LightDetection(BasePlayer option)
    {
      bool flag = false;
      Item activeItem = option.GetActiveItem();
      if (activeItem != null)
      {
        HeldEntity heldEntity = activeItem.GetHeldEntity() as HeldEntity;
        if (Object.op_Inequality((Object) heldEntity, (Object) null))
          flag = heldEntity.LightsOn();
      }
      return !flag ? 0.0f : 0.1f;
    }

    public PlayerDetectionChance()
    {
      base.\u002Ector();
    }
  }
}
