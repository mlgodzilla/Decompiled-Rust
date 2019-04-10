// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.BaseNpcMemory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN
{
  [Serializable]
  public class BaseNpcMemory
  {
    [ReadOnly]
    private readonly List<BaseNpcMemory.FailedDestinationInfo> _failedDestinationMemory = new List<BaseNpcMemory.FailedDestinationInfo>(10);
    [ReadOnly]
    public List<BaseNpcMemory.EnemyPlayerInfo> KnownEnemyPlayers = new List<BaseNpcMemory.EnemyPlayerInfo>(10);
    [ReadOnly]
    public List<BaseNpcMemory.EntityOfInterestInfo> KnownEntitiesOfInterest = new List<BaseNpcMemory.EntityOfInterestInfo>(10);
    [ReadOnly]
    public List<BaseNpcMemory.EntityOfInterestInfo> KnownTimedExplosives = new List<BaseNpcMemory.EntityOfInterestInfo>(10);
    [ReadOnly]
    public bool HasTargetDestination;
    [ReadOnly]
    public Vector3 TargetDestination;
    [ReadOnly]
    public BaseNpcMemory.EnemyPlayerInfo PrimaryKnownEnemyPlayer;
    [ReadOnly]
    public AnimalInfo PrimaryKnownAnimal;
    [ReadOnly]
    public Vector3 LastClosestEdgeNormal;
    [NonSerialized]
    public BaseNpcContext NpcContext;

    public virtual BaseNpcDefinition Definition
    {
      get
      {
        return (BaseNpcDefinition) null;
      }
    }

    public BaseNpcMemory(BaseNpcContext context)
    {
      this.NpcContext = context;
    }

    public virtual void ResetState()
    {
      this.HasTargetDestination = false;
      this._failedDestinationMemory?.Clear();
      this.PrimaryKnownEnemyPlayer.PlayerInfo.Player = (BasePlayer) null;
      this.KnownEnemyPlayers?.Clear();
      this.KnownEntitiesOfInterest?.Clear();
      this.PrimaryKnownAnimal.Animal = (BaseNpc) null;
      this.LastClosestEdgeNormal = Vector3.get_zero();
    }

    public bool IsValid(Vector3 destination)
    {
      foreach (BaseNpcMemory.FailedDestinationInfo failedDestinationInfo in this._failedDestinationMemory)
      {
        Vector3 vector3 = Vector3.op_Subtraction(failedDestinationInfo.Destination, destination);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 0.100000001490116)
          return false;
      }
      return true;
    }

    public void AddFailedDestination(Vector3 destination)
    {
      for (int index = 0; index < this._failedDestinationMemory.Count; ++index)
      {
        BaseNpcMemory.FailedDestinationInfo failedDestinationInfo = this._failedDestinationMemory[index];
        Vector3 vector3 = Vector3.op_Subtraction(failedDestinationInfo.Destination, destination);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 0.100000001490116)
        {
          failedDestinationInfo.Time = Time.get_time();
          this._failedDestinationMemory[index] = failedDestinationInfo;
          return;
        }
      }
      this._failedDestinationMemory.Add(new BaseNpcMemory.FailedDestinationInfo()
      {
        Time = Time.get_time(),
        Destination = destination
      });
    }

    public void ForgetPrimiaryEnemyPlayer()
    {
      this.PrimaryKnownEnemyPlayer.PlayerInfo.Player = (BasePlayer) null;
    }

    public void ForgetPrimiaryAnimal()
    {
      this.PrimaryKnownAnimal.Animal = (BaseNpc) null;
    }

    public void RememberPrimaryAnimal(BaseNpc animal)
    {
      if (Interface.CallHook("OnNpcPlayerTarget", (object) this, (object) animal) != null)
        return;
      for (int index = 0; index < this.NpcContext.AnimalsInRange.Count; ++index)
      {
        AnimalInfo animalInfo = this.NpcContext.AnimalsInRange[index];
        if (Object.op_Equality((Object) animalInfo.Animal, (Object) animal))
        {
          this.PrimaryKnownAnimal = animalInfo;
          break;
        }
      }
    }

    public void RememberPrimaryEnemyPlayer(BasePlayer primaryTarget)
    {
      for (int index = 0; index < this.KnownEnemyPlayers.Count; ++index)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = this.KnownEnemyPlayers[index];
        if (Object.op_Equality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) primaryTarget))
        {
          this.OnSetPrimaryKnownEnemyPlayer(ref knownEnemyPlayer);
          break;
        }
      }
    }

    protected virtual void OnSetPrimaryKnownEnemyPlayer(ref BaseNpcMemory.EnemyPlayerInfo info)
    {
      this.PrimaryKnownEnemyPlayer = info;
    }

    public void RememberEnemyPlayer(
      IHTNAgent npc,
      ref NpcPlayerInfo info,
      float time,
      float uncertainty = 0.0f,
      string debugStr = "ENEMY!")
    {
      if (Object.op_Equality((Object) info.Player, (Object) null) || Object.op_Equality((Object) ((Component) info.Player).get_transform(), (Object) null) || (info.Player.IsDestroyed || info.Player.IsDead()) || info.Player.IsWounded())
        return;
      if (Mathf.Approximately(info.SqrDistance, 0.0f))
      {
        ref NpcPlayerInfo local = ref info;
        Vector3 vector3 = Vector3.op_Subtraction(npc.BodyPosition, ((Component) info.Player).get_transform().get_position());
        double sqrMagnitude = (double) ((Vector3) ref vector3).get_sqrMagnitude();
        local.SqrDistance = (float) sqrMagnitude;
      }
      for (int index = 0; index < this.KnownEnemyPlayers.Count; ++index)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = this.KnownEnemyPlayers[index];
        if (Object.op_Equality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) info.Player))
        {
          knownEnemyPlayer.PlayerInfo = info;
          if ((double) uncertainty < 0.0500000007450581)
          {
            knownEnemyPlayer.LastKnownLocalPosition = ((Component) info.Player).get_transform().get_localPosition();
            ref BaseNpcMemory.EnemyPlayerInfo local = ref knownEnemyPlayer;
            Vector3 localVelocity = info.Player.GetLocalVelocity();
            Vector3 normalized = ((Vector3) ref localVelocity).get_normalized();
            local.LastKnownLocalHeading = normalized;
            knownEnemyPlayer.OurLastLocalPositionWhenLastSeen = npc.transform.get_localPosition();
            knownEnemyPlayer.BodyVisibleWhenLastNoticed = info.BodyVisible;
            knownEnemyPlayer.HeadVisibleWhenLastNoticed = info.HeadVisible;
          }
          else
          {
            Vector2 vector2 = Vector2.op_Multiply(Random.get_insideUnitCircle(), uncertainty);
            knownEnemyPlayer.LastKnownLocalPosition = Vector3.op_Addition(((Component) info.Player).get_transform().get_localPosition(), new Vector3((float) vector2.x, 0.0f, (float) vector2.y));
            ref BaseNpcMemory.EnemyPlayerInfo local = ref knownEnemyPlayer;
            Vector3 vector3 = Vector3.op_Subtraction(knownEnemyPlayer.LastKnownPosition, this.NpcContext.BodyPosition);
            Vector3 normalized = ((Vector3) ref vector3).get_normalized();
            local.LastKnownLocalHeading = normalized;
            knownEnemyPlayer.BodyVisibleWhenLastNoticed = info.BodyVisible;
            knownEnemyPlayer.HeadVisibleWhenLastNoticed = info.HeadVisible;
          }
          knownEnemyPlayer.Time = time;
          this.KnownEnemyPlayers[index] = knownEnemyPlayer;
          if (!Object.op_Equality((Object) this.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) info.Player))
            return;
          this.PrimaryKnownEnemyPlayer = knownEnemyPlayer;
          return;
        }
      }
      this.KnownEnemyPlayers.Add(new BaseNpcMemory.EnemyPlayerInfo()
      {
        PlayerInfo = info,
        LastKnownLocalPosition = ((Component) info.Player).get_transform().get_localPosition(),
        Time = time
      });
    }

    public void RememberEntityOfInterest(
      IHTNAgent npc,
      BaseEntity entityOfInterest,
      float time,
      string debugStr)
    {
      TimedExplosive explosive = entityOfInterest as TimedExplosive;
      if (Object.op_Inequality((Object) explosive, (Object) null))
        this.RememberTimedExplosives(npc, explosive, time, "EXPLOSIVE!");
      bool flag = false;
      for (int index = 0; index < this.KnownEntitiesOfInterest.Count; ++index)
      {
        BaseNpcMemory.EntityOfInterestInfo entityOfInterestInfo = this.KnownEntitiesOfInterest[index];
        if (Object.op_Equality((Object) entityOfInterestInfo.Entity, (Object) null))
        {
          this.KnownEntitiesOfInterest.RemoveAt(index);
          --index;
        }
        else if (Object.op_Equality((Object) entityOfInterestInfo.Entity, (Object) entityOfInterest))
        {
          entityOfInterestInfo.Time = time;
          this.KnownEntitiesOfInterest[index] = entityOfInterestInfo;
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this.KnownEntitiesOfInterest.Add(new BaseNpcMemory.EntityOfInterestInfo()
      {
        Entity = entityOfInterest,
        Time = time
      });
    }

    public void RememberTimedExplosives(
      IHTNAgent npc,
      TimedExplosive explosive,
      float time,
      string debugStr)
    {
      bool flag = false;
      for (int index = 0; index < this.KnownTimedExplosives.Count; ++index)
      {
        BaseNpcMemory.EntityOfInterestInfo knownTimedExplosive = this.KnownTimedExplosives[index];
        if (Object.op_Equality((Object) knownTimedExplosive.Entity, (Object) null))
        {
          this.KnownTimedExplosives.RemoveAt(index);
          --index;
        }
        else if (Object.op_Equality((Object) knownTimedExplosive.Entity, (Object) explosive))
        {
          knownTimedExplosive.Time = time;
          this.KnownTimedExplosives[index] = knownTimedExplosive;
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this.KnownTimedExplosives.Add(new BaseNpcMemory.EntityOfInterestInfo()
      {
        Entity = (BaseEntity) explosive,
        Time = time
      });
    }

    protected virtual void OnForget(BasePlayer player)
    {
    }

    public void Forget(float memoryTimeout)
    {
      float time = Time.get_time();
      for (int index = 0; index < this._failedDestinationMemory.Count; ++index)
      {
        BaseNpcMemory.FailedDestinationInfo failedDestinationInfo = this._failedDestinationMemory[index];
        if ((double) time - (double) failedDestinationInfo.Time > (double) memoryTimeout)
        {
          this._failedDestinationMemory.RemoveAt(index);
          --index;
        }
      }
      for (int index = 0; index < this.KnownEnemyPlayers.Count; ++index)
      {
        BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = this.KnownEnemyPlayers[index];
        float num = time - knownEnemyPlayer.Time;
        if ((double) num > (double) memoryTimeout)
        {
          this.KnownEnemyPlayers.RemoveAt(index);
          --index;
          if (Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null))
          {
            this.OnForget(knownEnemyPlayer.PlayerInfo.Player);
            if (Object.op_Equality((Object) this.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) knownEnemyPlayer.PlayerInfo.Player))
              this.ForgetPrimiaryEnemyPlayer();
          }
        }
        else if (Object.op_Equality((Object) this.PrimaryKnownEnemyPlayer.PlayerInfo.Player, (Object) knownEnemyPlayer.PlayerInfo.Player))
        {
          this.PrimaryKnownEnemyPlayer.PlayerInfo.AudibleScore *= (float) (1.0 - (double) num / (double) memoryTimeout);
          this.PrimaryKnownEnemyPlayer.PlayerInfo.VisibilityScore *= (float) (1.0 - (double) num / (double) memoryTimeout);
        }
      }
      for (int index = 0; index < this.KnownEntitiesOfInterest.Count; ++index)
      {
        BaseNpcMemory.EntityOfInterestInfo entityOfInterestInfo = this.KnownEntitiesOfInterest[index];
        if ((double) time - (double) entityOfInterestInfo.Time > (double) memoryTimeout)
        {
          this.KnownEntitiesOfInterest.RemoveAt(index);
          --index;
        }
      }
      if (!Object.op_Inequality((Object) this.PrimaryKnownAnimal.Animal, (Object) null) || (double) time - (double) this.PrimaryKnownAnimal.Time <= (double) memoryTimeout)
        return;
      this.PrimaryKnownAnimal.Animal = (BaseNpc) null;
    }

    public virtual bool ShouldRemoveOnPlayerForgetTimeout(float time, NpcPlayerInfo player)
    {
      if (Object.op_Equality((Object) player.Player, (Object) null) || Object.op_Equality((Object) ((Component) player.Player).get_transform(), (Object) null) || (player.Player.IsDestroyed || player.Player.IsDead()) || player.Player.IsWounded())
        return true;
      double num1 = (double) time;
      double time1 = (double) player.Time;
      BaseNpcDefinition definition = this.Definition;
      double num2 = definition != null ? (double) definition.Memory.ForgetInRangeTime : 0.0;
      double num3 = time1 + num2;
      return num1 > num3;
    }

    [Serializable]
    private struct FailedDestinationInfo
    {
      public float Time;
      public Vector3 Destination;
    }

    [Serializable]
    public struct EnemyPlayerInfo
    {
      public float Time;
      public NpcPlayerInfo PlayerInfo;
      public Vector3 LastKnownLocalPosition;
      public Vector3 LastKnownLocalHeading;
      public Vector3 OurLastLocalPositionWhenLastSeen;
      public bool BodyVisibleWhenLastNoticed;
      public bool HeadVisibleWhenLastNoticed;

      public Vector3 LastKnownPosition
      {
        get
        {
          if (Object.op_Inequality((Object) this.PlayerInfo.Player, (Object) null))
          {
            BaseEntity parentEntity = this.PlayerInfo.Player.GetParentEntity();
            if (Object.op_Inequality((Object) parentEntity, (Object) null))
              return ((Component) parentEntity).get_transform().TransformPoint(this.LastKnownLocalPosition);
          }
          return this.LastKnownLocalPosition;
        }
      }

      public Vector3 LastKnownHeading
      {
        get
        {
          if (Object.op_Inequality((Object) this.PlayerInfo.Player, (Object) null))
          {
            BaseEntity parentEntity = this.PlayerInfo.Player.GetParentEntity();
            if (Object.op_Inequality((Object) parentEntity, (Object) null))
              return ((Component) parentEntity).get_transform().TransformDirection(this.LastKnownLocalHeading);
          }
          return this.LastKnownLocalHeading;
        }
      }

      public Vector3 OurLastPositionWhenLastSeen
      {
        get
        {
          if (Object.op_Inequality((Object) this.PlayerInfo.Player, (Object) null))
          {
            BaseEntity parentEntity = this.PlayerInfo.Player.GetParentEntity();
            if (Object.op_Inequality((Object) parentEntity, (Object) null))
              return ((Component) parentEntity).get_transform().TransformPoint(this.OurLastLocalPositionWhenLastSeen);
          }
          return this.OurLastLocalPositionWhenLastSeen;
        }
      }
    }

    [Serializable]
    public struct EntityOfInterestInfo
    {
      public float Time;
      public BaseEntity Entity;
    }
  }
}
