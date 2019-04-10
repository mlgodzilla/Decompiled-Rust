// Decompiled with JetBrains decompiler
// Type: Rust.Ai.SwitchToolOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using ConVar;
using UnityEngine;

namespace Rust.Ai
{
  public class SwitchToolOperator : BaseAction
  {
    [ApexSerialization]
    private NPCPlayerApex.ToolTypeEnum ToolTypeDay;
    [ApexSerialization]
    private NPCPlayerApex.ToolTypeEnum ToolTypeNight;

    public override void DoExecute(BaseContext c)
    {
      SwitchToolOperator.TrySwitchToolTo(c as NPCHumanContext, this.ToolTypeDay, this.ToolTypeNight);
    }

    public static bool TrySwitchToolTo(
      NPCHumanContext c,
      NPCPlayerApex.ToolTypeEnum toolDay,
      NPCPlayerApex.ToolTypeEnum toolNight)
    {
      if (c != null)
      {
        Item obj = (Item) null;
        uint svActiveItemId = c.Human.svActiveItemID;
        if (Object.op_Inequality((Object) TOD_Sky.get_Instance(), (Object) null))
          obj = !TOD_Sky.get_Instance().get_IsDay() ? SwitchToolOperator.FindTool(c, toolNight) : SwitchToolOperator.FindTool(c, toolDay);
        if (obj != null)
        {
          c.Human.UpdateActiveItem(obj.uid);
          if ((int) svActiveItemId != (int) c.Human.svActiveItemID)
          {
            c.Human.NextToolSwitchTime = Time.get_realtimeSinceStartup() + c.Human.ToolSwitchFrequency;
            c.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte) 0, true, true);
            c.SetFact(NPCPlayerApex.Facts.CurrentToolType, (byte) c.Human.GetCurrentToolTypeEnum(), true, true);
          }
          return true;
        }
      }
      return false;
    }

    public static Item FindTool(NPCHumanContext c, NPCPlayerApex.ToolTypeEnum tool)
    {
      foreach (Item allItem in c.Human.inventory.AllItems())
      {
        if (allItem.info.category == ItemCategory.Tool)
        {
          HeldEntity heldEntity = allItem.GetHeldEntity() as HeldEntity;
          if (Object.op_Inequality((Object) heldEntity, (Object) null) && heldEntity.toolType == tool)
            return allItem;
        }
      }
      return (Item) null;
    }

    public class HasCurrentToolType : BaseScorer
    {
      [ApexSerialization(defaultValue = NPCPlayerApex.ToolTypeEnum.None)]
      public NPCPlayerApex.ToolTypeEnum value;

      public override float GetScore(BaseContext c)
      {
        return (NPCPlayerApex.ToolTypeEnum) c.GetFact(NPCPlayerApex.Facts.CurrentToolType) != this.value ? 0.0f : 1f;
      }
    }

    public class TargetVisibleFor : BaseScorer
    {
      [ApexSerialization]
      public float duration;

      public override float GetScore(BaseContext c)
      {
        return SwitchToolOperator.TargetVisibleFor.Test(c as NPCHumanContext, this.duration);
      }

      public static float Test(NPCHumanContext c, float duration)
      {
        return c == null || (!Object.op_Inequality((Object) c.Human.AttackTarget, (Object) null) || !Object.op_Equality((Object) c.Human.lastAttacker, (Object) c.Human.AttackTarget) || (double) c.Human.SecondsSinceAttacked >= 10.0) && (double) c.Human.AttackTargetVisibleFor < (double) duration ? 0.0f : 1f;
      }
    }

    public class ReactiveAimsAtTarget : BaseScorer
    {
      public override float GetScore(BaseContext c)
      {
        return SwitchToolOperator.ReactiveAimsAtTarget.Test(c as NPCHumanContext);
      }

      public static float Test(NPCHumanContext c)
      {
        if (Object.op_Equality((Object) c.Human, (Object) null) || Object.op_Equality((Object) ((Component) c.Human).get_transform(), (Object) null) || (c.Human.IsDestroyed || Object.op_Equality((Object) c.Human.AttackTarget, (Object) null)) || (Object.op_Equality((Object) ((Component) c.Human.AttackTarget).get_transform(), (Object) null) || c.Human.AttackTarget.IsDestroyed))
          return 0.0f;
        Vector3 vector3 = Vector3.op_Subtraction(c.Human.AttackTarget.ServerPosition, c.Position);
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        float num = Vector3.Dot(c.Human.eyes.BodyForward(), normalized);
        if (c.Human.isMounted)
          return (double) num < (double) AI.npc_valid_mounted_aim_cone ? 0.0f : 1f;
        return (double) num < (double) AI.npc_valid_aim_cone ? 0.0f : 1f;
      }
    }
  }
}
