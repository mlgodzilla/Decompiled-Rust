// Decompiled with JetBrains decompiler
// Type: Decay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Decay : PrefabAttribute, IServerComponent
{
  private const float hours = 3600f;

  protected float GetDecayDelay(BuildingGrade.Enum grade)
  {
    if (ConVar.Decay.upkeep)
    {
      if ((double) ConVar.Decay.delay_override > 0.0)
        return ConVar.Decay.delay_override;
      switch (grade)
      {
        case BuildingGrade.Enum.Wood:
          return ConVar.Decay.delay_wood * 3600f;
        case BuildingGrade.Enum.Stone:
          return ConVar.Decay.delay_stone * 3600f;
        case BuildingGrade.Enum.Metal:
          return ConVar.Decay.delay_metal * 3600f;
        case BuildingGrade.Enum.TopTier:
          return ConVar.Decay.delay_toptier * 3600f;
        default:
          return ConVar.Decay.delay_twig * 3600f;
      }
    }
    else
    {
      switch (grade)
      {
        case BuildingGrade.Enum.Wood:
          return 64800f;
        case BuildingGrade.Enum.Stone:
          return 64800f;
        case BuildingGrade.Enum.Metal:
          return 64800f;
        case BuildingGrade.Enum.TopTier:
          return 86400f;
        default:
          return 3600f;
      }
    }
  }

  protected float GetDecayDuration(BuildingGrade.Enum grade)
  {
    if (ConVar.Decay.upkeep)
    {
      if ((double) ConVar.Decay.duration_override > 0.0)
        return ConVar.Decay.duration_override;
      switch (grade)
      {
        case BuildingGrade.Enum.Wood:
          return ConVar.Decay.duration_wood * 3600f;
        case BuildingGrade.Enum.Stone:
          return ConVar.Decay.duration_stone * 3600f;
        case BuildingGrade.Enum.Metal:
          return ConVar.Decay.duration_metal * 3600f;
        case BuildingGrade.Enum.TopTier:
          return ConVar.Decay.duration_toptier * 3600f;
        default:
          return ConVar.Decay.duration_twig * 3600f;
      }
    }
    else
    {
      switch (grade)
      {
        case BuildingGrade.Enum.Wood:
          return 86400f;
        case BuildingGrade.Enum.Stone:
          return 172800f;
        case BuildingGrade.Enum.Metal:
          return 259200f;
        case BuildingGrade.Enum.TopTier:
          return 432000f;
        default:
          return 3600f;
      }
    }
  }

  public static void BuildingDecayTouch(BuildingBlock buildingBlock)
  {
    if (ConVar.Decay.upkeep)
      return;
    List<DecayEntity> list = (List<DecayEntity>) Pool.GetList<DecayEntity>();
    Vis.Entities<DecayEntity>(((Component) buildingBlock).get_transform().get_position(), 40f, list, 2097408, (QueryTriggerInteraction) 2);
    for (int index = 0; index < list.Count; ++index)
    {
      DecayEntity decayEntity = list[index];
      BuildingBlock buildingBlock1 = decayEntity as BuildingBlock;
      if (!Object.op_Implicit((Object) buildingBlock1) || (int) buildingBlock1.buildingID == (int) buildingBlock.buildingID)
        decayEntity.DecayTouch();
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<DecayEntity>((List<M0>&) ref list);
  }

  public static void EntityLinkDecayTouch(BaseEntity ent)
  {
    if (ConVar.Decay.upkeep)
      return;
    ent.EntityLinkBroadcast<DecayEntity>((Action<DecayEntity>) (decayEnt => decayEnt.DecayTouch()));
  }

  public static void RadialDecayTouch(Vector3 pos, float radius, int mask)
  {
    if (ConVar.Decay.upkeep)
      return;
    List<DecayEntity> list = (List<DecayEntity>) Pool.GetList<DecayEntity>();
    Vis.Entities<DecayEntity>(pos, radius, list, mask, (QueryTriggerInteraction) 2);
    for (int index = 0; index < list.Count; ++index)
      list[index].DecayTouch();
    // ISSUE: cast to a reference type
    Pool.FreeList<DecayEntity>((List<M0>&) ref list);
  }

  public virtual bool ShouldDecay(BaseEntity entity)
  {
    return true;
  }

  public abstract float GetDecayDelay(BaseEntity entity);

  public abstract float GetDecayDuration(BaseEntity entity);

  protected override System.Type GetIndexedType()
  {
    return typeof (Decay);
  }
}
