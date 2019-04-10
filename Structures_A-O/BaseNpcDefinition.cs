// Decompiled with JetBrains decompiler
// Type: BaseNpcDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN;
using System;
using UnityEngine;

public abstract class BaseNpcDefinition : Definition<BaseNpcDefinition>
{
  [Header("Domain")]
  public HTNDomain Domain;
  [Header("Base Stats")]
  public BaseNpcDefinition.InfoStats Info;
  public BaseNpcDefinition.VitalStats Vitals;
  public BaseNpcDefinition.MovementStats Movement;
  public BaseNpcDefinition.SensoryStats Sensory;
  public BaseNpcDefinition.MemoryStats Memory;
  public BaseNpcDefinition.EngagementStats Engagement;

  public virtual void Loadout(HTNPlayer target)
  {
  }

  public virtual void OnlyLoadoutWeapons(HTNPlayer target)
  {
  }

  public virtual void StartVoices(HTNPlayer target)
  {
  }

  public virtual void StopVoices(HTNPlayer target)
  {
  }

  public virtual BaseCorpse OnCreateCorpse(HTNPlayer target)
  {
    return (BaseCorpse) null;
  }

  public virtual void Loadout(HTNAnimal target)
  {
  }

  public virtual void StartVoices(HTNAnimal target)
  {
  }

  public virtual void StopVoices(HTNAnimal target)
  {
  }

  public virtual BaseCorpse OnCreateCorpse(HTNAnimal target)
  {
    return (BaseCorpse) null;
  }

  [Serializable]
  public class InfoStats
  {
    public BaseNpcDefinition.Family Family;
    public BaseNpcDefinition.Family[] Predators;
    public BaseNpcDefinition.Family[] Prey;

    public BaseNpc.AiStatistics.FamilyEnum ToFamily(BaseNpcDefinition.Family family)
    {
      switch (family)
      {
        case BaseNpcDefinition.Family.Scientist:
          return BaseNpc.AiStatistics.FamilyEnum.Scientist;
        case BaseNpcDefinition.Family.Murderer:
          return BaseNpc.AiStatistics.FamilyEnum.Murderer;
        case BaseNpcDefinition.Family.Horse:
          return BaseNpc.AiStatistics.FamilyEnum.Horse;
        case BaseNpcDefinition.Family.Deer:
          return BaseNpc.AiStatistics.FamilyEnum.Deer;
        case BaseNpcDefinition.Family.Boar:
          return BaseNpc.AiStatistics.FamilyEnum.Boar;
        case BaseNpcDefinition.Family.Wolf:
          return BaseNpc.AiStatistics.FamilyEnum.Wolf;
        case BaseNpcDefinition.Family.Bear:
          return BaseNpc.AiStatistics.FamilyEnum.Bear;
        case BaseNpcDefinition.Family.Chicken:
          return BaseNpc.AiStatistics.FamilyEnum.Chicken;
        case BaseNpcDefinition.Family.Zombie:
          return BaseNpc.AiStatistics.FamilyEnum.Zombie;
        default:
          return BaseNpc.AiStatistics.FamilyEnum.Player;
      }
    }
  }

  [Serializable]
  public class VitalStats
  {
    public float HP = 100f;
  }

  [Serializable]
  public class MovementStats
  {
    public float DuckSpeed = 1.7f;
    public float WalkSpeed = 2.8f;
    public float RunSpeed = 5.5f;
    public float TurnSpeed = 120f;
    public float Acceleration = 12f;
  }

  [Serializable]
  public class SensoryStats
  {
    public float VisionRange = 40f;
    public float HearingRange = 20f;
    [Range(0.0f, 360f)]
    public float FieldOfView = 120f;
    private const float Inv180 = 0.005555556f;

    public float SqrVisionRange
    {
      get
      {
        return this.VisionRange * this.VisionRange;
      }
    }

    public float SqrHearingRange
    {
      get
      {
        return this.HearingRange * this.HearingRange;
      }
    }

    public float FieldOfViewRadians
    {
      get
      {
        return (float) (((double) this.FieldOfView - 180.0) * -0.00555555569007993 - 0.100000001490116);
      }
    }
  }

  [Serializable]
  public class MemoryStats
  {
    public float ForgetTime = 30f;
    public float ForgetInRangeTime = 5f;
    public float NoSeeReturnToSpawnTime = 10f;
    public float ForgetAnimalInRangeTime = 5f;
  }

  [Serializable]
  public class EngagementStats
  {
    public float CloseRange = 2f;
    public float MediumRange = 20f;
    public float LongRange = 100f;
    public float AggroRange = 100f;
    public float DeaggroRange = 150f;
    public float Hostility = 1f;
    public float Defensiveness = 1f;

    public float SqrCloseRange
    {
      get
      {
        return this.CloseRange * this.CloseRange;
      }
    }

    public float SqrMediumRange
    {
      get
      {
        return this.MediumRange * this.MediumRange;
      }
    }

    public float SqrLongRange
    {
      get
      {
        return this.LongRange * this.LongRange;
      }
    }

    public float SqrAggroRange
    {
      get
      {
        return this.AggroRange * this.AggroRange;
      }
    }

    public float SqrDeaggroRange
    {
      get
      {
        return this.DeaggroRange * this.DeaggroRange;
      }
    }

    public float CloseRangeFirearm(AttackEntity ent)
    {
      if (!Object.op_Implicit((Object) ent))
        return this.CloseRange;
      return this.CloseRange + ent.CloseRangeAddition;
    }

    public float MediumRangeFirearm(AttackEntity ent)
    {
      if (!Object.op_Implicit((Object) ent))
        return this.MediumRange;
      return this.MediumRange + ent.MediumRangeAddition;
    }

    public float LongRangeFirearm(AttackEntity ent)
    {
      if (!Object.op_Implicit((Object) ent))
        return this.LongRange;
      return this.LongRange + ent.LongRangeAddition;
    }

    public float SqrCloseRangeFirearm(AttackEntity ent)
    {
      double num = (double) this.CloseRangeFirearm(ent);
      return (float) (num * num);
    }

    public float SqrMediumRangeFirearm(AttackEntity ent)
    {
      double num = (double) this.MediumRangeFirearm(ent);
      return (float) (num * num);
    }

    public float SqrLongRangeFirearm(AttackEntity ent)
    {
      double num = (double) this.LongRangeFirearm(ent);
      return (float) (num * num);
    }

    public float CenterOfCloseRange()
    {
      return this.CloseRange * 0.5f;
    }

    public float CenterOfCloseRangeFirearm(AttackEntity ent)
    {
      return this.CloseRangeFirearm(ent) * 0.5f;
    }

    public float SqrCenterOfCloseRange()
    {
      double num = (double) this.CenterOfCloseRange();
      return (float) (num * num);
    }

    public float SqrCenterOfCloseRangeFirearm(AttackEntity ent)
    {
      double num = (double) this.CenterOfCloseRangeFirearm(ent);
      return (float) (num * num);
    }

    public float CenterOfMediumRange()
    {
      return this.MediumRange - (this.MediumRange - this.CloseRange) * 0.5f;
    }

    public float CenterOfMediumRangeFirearm(AttackEntity ent)
    {
      double num = (double) this.MediumRangeFirearm(ent);
      return (float) (num - (double) ((float) num - this.CloseRangeFirearm(ent)) * 0.5);
    }

    public float SqrCenterOfMediumRange()
    {
      double num = (double) this.CenterOfMediumRange();
      return (float) (num * num);
    }

    public float SqrCenterOfMediumRangeFirearm(AttackEntity ent)
    {
      double num = (double) this.CenterOfMediumRangeFirearm(ent);
      return (float) (num * num);
    }
  }

  [Serializable]
  public class RoamStats
  {
    public float MaxRoamRange = 20f;
    public float MinRoamDelay = 5f;
    public float MaxRoamDelay = 10f;

    public float SqrMaxRoamRange
    {
      get
      {
        return this.MaxRoamRange * this.MaxRoamRange;
      }
    }
  }

  public enum Family
  {
    Player,
    Scientist,
    Murderer,
    Horse,
    Deer,
    Boar,
    Wolf,
    Bear,
    Chicken,
    Zombie,
  }
}
