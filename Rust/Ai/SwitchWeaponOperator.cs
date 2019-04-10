// Decompiled with JetBrains decompiler
// Type: Rust.Ai.SwitchWeaponOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class SwitchWeaponOperator : BaseAction
  {
    [ApexSerialization]
    private NPCPlayerApex.WeaponTypeEnum WeaponType;

    public override void DoExecute(BaseContext c)
    {
      SwitchWeaponOperator.TrySwitchWeaponTo(c as NPCHumanContext, this.WeaponType);
    }

    public static bool TrySwitchWeaponTo(NPCHumanContext c, NPCPlayerApex.WeaponTypeEnum WeaponType)
    {
      if (c != null && (double) Time.get_realtimeSinceStartup() >= (double) c.Human.NextWeaponSwitchTime)
      {
        uint svActiveItemId = c.Human.svActiveItemID;
        Item obj;
        switch (WeaponType)
        {
          case NPCPlayerApex.WeaponTypeEnum.CloseRange:
            obj = SwitchWeaponOperator.FindBestMelee(c);
            if (obj == null)
            {
              obj = SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.None, NPCPlayerApex.WeaponTypeEnum.CloseRange, c) ?? SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.CloseRange, NPCPlayerApex.WeaponTypeEnum.MediumRange, c) ?? SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.MediumRange, NPCPlayerApex.WeaponTypeEnum.LongRange, c);
              c.Human.StoppingDistance = 2.5f;
              break;
            }
            c.Human.StoppingDistance = 1.5f;
            break;
          case NPCPlayerApex.WeaponTypeEnum.MediumRange:
            obj = SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.CloseRange, NPCPlayerApex.WeaponTypeEnum.MediumRange, c) ?? SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.MediumRange, NPCPlayerApex.WeaponTypeEnum.LongRange, c);
            c.Human.StoppingDistance = 0.1f;
            break;
          case NPCPlayerApex.WeaponTypeEnum.LongRange:
            obj = SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.MediumRange, NPCPlayerApex.WeaponTypeEnum.LongRange, c) ?? SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.CloseRange, NPCPlayerApex.WeaponTypeEnum.MediumRange, c);
            c.Human.StoppingDistance = 0.1f;
            break;
          default:
            c.Human.UpdateActiveItem(0U);
            if ((int) svActiveItemId != (int) c.Human.svActiveItemID)
            {
              c.Human.NextWeaponSwitchTime = Time.get_realtimeSinceStartup() + c.Human.WeaponSwitchFrequency;
              c.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte) c.Human.GetCurrentWeaponTypeEnum(), true, true);
              c.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte) c.Human.GetCurrentAmmoStateEnum(), true, true);
            }
            c.Human.StoppingDistance = 1f;
            return true;
        }
        if (obj != null)
        {
          c.Human.UpdateActiveItem(obj.uid);
          if ((int) svActiveItemId != (int) c.Human.svActiveItemID)
          {
            c.Human.NextWeaponSwitchTime = Time.get_realtimeSinceStartup() + c.Human.WeaponSwitchFrequency;
            c.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte) c.Human.GetCurrentWeaponTypeEnum(), true, true);
            c.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte) c.Human.GetCurrentAmmoStateEnum(), true, true);
            c.SetFact(NPCPlayerApex.Facts.CurrentToolType, (byte) 0, true, true);
          }
          return true;
        }
      }
      return false;
    }

    private static Item FindBestMelee(NPCHumanContext c)
    {
      if (c.Human.GetPathStatus() != (byte) 0)
        return (Item) null;
      Item obj = (Item) null;
      BaseMelee baseMelee = (BaseMelee) null;
      foreach (Item allItem in c.Human.inventory.AllItems())
      {
        if (allItem.info.category == ItemCategory.Weapon && !allItem.isBroken)
        {
          BaseMelee heldEntity = allItem.GetHeldEntity() as BaseMelee;
          if (Object.op_Implicit((Object) heldEntity))
          {
            if (obj == null)
            {
              obj = allItem;
              baseMelee = heldEntity;
            }
            else if ((double) heldEntity.hostileScore > (double) baseMelee.hostileScore)
            {
              obj = allItem;
              baseMelee = heldEntity;
            }
          }
        }
      }
      return obj;
    }

    private static Item FindBestProjInRange(
      NPCPlayerApex.WeaponTypeEnum from,
      NPCPlayerApex.WeaponTypeEnum to,
      NPCHumanContext c)
    {
      Item obj = (Item) null;
      BaseProjectile baseProjectile = (BaseProjectile) null;
      foreach (Item allItem in c.Human.inventory.AllItems())
      {
        if (allItem.info.category == ItemCategory.Weapon && !allItem.isBroken)
        {
          BaseProjectile heldEntity = allItem.GetHeldEntity() as BaseProjectile;
          if (Object.op_Inequality((Object) heldEntity, (Object) null) && heldEntity.effectiveRangeType <= to && heldEntity.effectiveRangeType > from)
          {
            if (obj == null)
            {
              obj = allItem;
              baseProjectile = heldEntity;
            }
            else if ((double) heldEntity.hostileScore > (double) baseProjectile.hostileScore)
            {
              obj = allItem;
              baseProjectile = heldEntity;
            }
          }
        }
      }
      return obj;
    }
  }
}
