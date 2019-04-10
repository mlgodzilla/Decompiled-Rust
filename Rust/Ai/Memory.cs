// Decompiled with JetBrains decompiler
// Type: Rust.Ai.Memory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class Memory
  {
    public List<BaseEntity> Visible = new List<BaseEntity>();
    public List<Memory.SeenInfo> All = new List<Memory.SeenInfo>();
    public List<Memory.ExtendedInfo> AllExtended = new List<Memory.ExtendedInfo>();

    public Memory.SeenInfo Update(
      BaseEntity entity,
      float score,
      Vector3 direction,
      float dot,
      float distanceSqr,
      byte lineOfSight,
      bool updateLastHurtUsTime,
      float lastHurtUsTime,
      out Memory.ExtendedInfo extendedInfo)
    {
      return this.Update(entity, entity.ServerPosition, score, direction, dot, distanceSqr, lineOfSight, updateLastHurtUsTime, lastHurtUsTime, out extendedInfo);
    }

    public Memory.SeenInfo Update(
      BaseEntity entity,
      Vector3 position,
      float score,
      Vector3 direction,
      float dot,
      float distanceSqr,
      byte lineOfSight,
      bool updateLastHurtUsTime,
      float lastHurtUsTime,
      out Memory.ExtendedInfo extendedInfo)
    {
      extendedInfo = new Memory.ExtendedInfo();
      bool flag = false;
      for (int index = 0; index < this.AllExtended.Count; ++index)
      {
        if (Object.op_Equality((Object) this.AllExtended[index].Entity, (Object) entity))
        {
          Memory.ExtendedInfo extendedInfo1 = this.AllExtended[index];
          extendedInfo1.Direction = direction;
          extendedInfo1.Dot = dot;
          extendedInfo1.DistanceSqr = distanceSqr;
          extendedInfo1.LineOfSight = lineOfSight;
          if (updateLastHurtUsTime)
            extendedInfo1.LastHurtUsTime = lastHurtUsTime;
          this.AllExtended[index] = extendedInfo1;
          extendedInfo = extendedInfo1;
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        if (updateLastHurtUsTime)
        {
          Memory.ExtendedInfo extendedInfo1 = new Memory.ExtendedInfo()
          {
            Entity = entity,
            Direction = direction,
            Dot = dot,
            DistanceSqr = distanceSqr,
            LineOfSight = lineOfSight,
            LastHurtUsTime = lastHurtUsTime
          };
          this.AllExtended.Add(extendedInfo1);
          extendedInfo = extendedInfo1;
        }
        else
        {
          Memory.ExtendedInfo extendedInfo1 = new Memory.ExtendedInfo()
          {
            Entity = entity,
            Direction = direction,
            Dot = dot,
            DistanceSqr = distanceSqr,
            LineOfSight = lineOfSight
          };
          this.AllExtended.Add(extendedInfo1);
          extendedInfo = extendedInfo1;
        }
      }
      return this.Update(entity, position, score);
    }

    public Memory.SeenInfo Update(BaseEntity ent, float danger = 0.0f)
    {
      return this.Update(ent, ent.ServerPosition, danger);
    }

    public Memory.SeenInfo Update(BaseEntity ent, Vector3 position, float danger = 0.0f)
    {
      for (int index = 0; index < this.All.Count; ++index)
      {
        if (Object.op_Equality((Object) this.All[index].Entity, (Object) ent))
        {
          Memory.SeenInfo seenInfo = this.All[index];
          seenInfo.Position = position;
          seenInfo.Timestamp = Time.get_realtimeSinceStartup();
          seenInfo.Danger += danger;
          this.All[index] = seenInfo;
          return seenInfo;
        }
      }
      Memory.SeenInfo seenInfo1 = new Memory.SeenInfo()
      {
        Entity = ent,
        Position = position,
        Timestamp = Time.get_realtimeSinceStartup(),
        Danger = danger
      };
      this.All.Add(seenInfo1);
      this.Visible.Add(ent);
      return seenInfo1;
    }

    public void AddDanger(Vector3 position, float amount)
    {
      for (int index = 0; index < this.All.Count; ++index)
      {
        if (Mathf.Approximately((float) this.All[index].Position.x, (float) position.x) && Mathf.Approximately((float) this.All[index].Position.y, (float) position.y) && Mathf.Approximately((float) this.All[index].Position.z, (float) position.z))
        {
          Memory.SeenInfo seenInfo = this.All[index];
          seenInfo.Danger = amount;
          this.All[index] = seenInfo;
          return;
        }
      }
      this.All.Add(new Memory.SeenInfo()
      {
        Position = position,
        Timestamp = Time.get_realtimeSinceStartup(),
        Danger = amount
      });
    }

    public Memory.SeenInfo GetInfo(BaseEntity entity)
    {
      foreach (Memory.SeenInfo seenInfo in this.All)
      {
        if (Object.op_Equality((Object) seenInfo.Entity, (Object) entity))
          return seenInfo;
      }
      return new Memory.SeenInfo();
    }

    public Memory.SeenInfo GetInfo(Vector3 position)
    {
      foreach (Memory.SeenInfo seenInfo in this.All)
      {
        Vector3 vector3 = Vector3.op_Subtraction(seenInfo.Position, position);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 1.0)
          return seenInfo;
      }
      return new Memory.SeenInfo();
    }

    public Memory.ExtendedInfo GetExtendedInfo(BaseEntity entity)
    {
      foreach (Memory.ExtendedInfo extendedInfo in this.AllExtended)
      {
        if (Object.op_Equality((Object) extendedInfo.Entity, (Object) entity))
          return extendedInfo;
      }
      return new Memory.ExtendedInfo();
    }

    internal void Forget(float maxSecondsOld)
    {
      for (int index1 = 0; index1 < this.All.Count; ++index1)
      {
        float num1 = Time.get_realtimeSinceStartup() - this.All[index1].Timestamp;
        if ((double) num1 > (double) maxSecondsOld)
        {
          if (Object.op_Inequality((Object) this.All[index1].Entity, (Object) null))
          {
            this.Visible.Remove(this.All[index1].Entity);
            for (int index2 = 0; index2 < this.AllExtended.Count; ++index2)
            {
              if (Object.op_Equality((Object) this.AllExtended[index2].Entity, (Object) this.All[index1].Entity))
              {
                this.AllExtended.RemoveAt(index2);
                break;
              }
            }
          }
          this.All.RemoveAt(index1);
          --index1;
        }
        else if ((double) num1 > 0.0)
        {
          float num2 = num1 / maxSecondsOld;
          if ((double) this.All[index1].Danger > 0.0)
          {
            Memory.SeenInfo seenInfo = this.All[index1];
            seenInfo.Danger -= num2;
            this.All[index1] = seenInfo;
          }
          if ((double) num1 >= 1.0)
          {
            for (int index2 = 0; index2 < this.AllExtended.Count; ++index2)
            {
              if (Object.op_Equality((Object) this.AllExtended[index2].Entity, (Object) this.All[index1].Entity))
              {
                Memory.ExtendedInfo extendedInfo = this.AllExtended[index2];
                extendedInfo.LineOfSight = (byte) 0;
                this.AllExtended[index2] = extendedInfo;
                break;
              }
            }
          }
        }
      }
      for (int index = 0; index < this.Visible.Count; ++index)
      {
        if (Object.op_Equality((Object) this.Visible[index], (Object) null))
        {
          this.Visible.RemoveAt(index);
          --index;
        }
      }
      for (int index = 0; index < this.AllExtended.Count; ++index)
      {
        if (Object.op_Equality((Object) this.AllExtended[index].Entity, (Object) null))
        {
          this.AllExtended.RemoveAt(index);
          --index;
        }
      }
    }

    public struct SeenInfo
    {
      public BaseEntity Entity;
      public Vector3 Position;
      public float Timestamp;
      public float Danger;
    }

    public struct ExtendedInfo
    {
      public BaseEntity Entity;
      public Vector3 Direction;
      public float Dot;
      public float DistanceSqr;
      public byte LineOfSight;
      public float LastHurtUsTime;
    }
  }
}
