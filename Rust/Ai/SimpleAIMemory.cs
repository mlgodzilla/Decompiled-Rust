// Decompiled with JetBrains decompiler
// Type: Rust.AI.SimpleAIMemory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
  public class SimpleAIMemory
  {
    public List<BaseEntity> Visible = new List<BaseEntity>();
    public List<SimpleAIMemory.SeenInfo> All = new List<SimpleAIMemory.SeenInfo>();

    public void Update(BaseEntity ent)
    {
      for (int index = 0; index < this.All.Count; ++index)
      {
        if (Object.op_Equality((Object) this.All[index].Entity, (Object) ent))
        {
          SimpleAIMemory.SeenInfo seenInfo = this.All[index];
          seenInfo.Position = ((Component) ent).get_transform().get_position();
          seenInfo.Timestamp = Mathf.Max(Time.get_realtimeSinceStartup(), seenInfo.Timestamp);
          this.All[index] = seenInfo;
          return;
        }
      }
      this.All.Add(new SimpleAIMemory.SeenInfo()
      {
        Entity = ent,
        Position = ((Component) ent).get_transform().get_position(),
        Timestamp = Time.get_realtimeSinceStartup()
      });
      this.Visible.Add(ent);
    }

    public void AddDanger(Vector3 position, float amount)
    {
      this.All.Add(new SimpleAIMemory.SeenInfo()
      {
        Position = position,
        Timestamp = Time.get_realtimeSinceStartup(),
        Danger = amount
      });
    }

    internal void Forget(float secondsOld)
    {
      for (int index = 0; index < this.All.Count; ++index)
      {
        if ((double) Time.get_realtimeSinceStartup() - (double) this.All[index].Timestamp > (double) secondsOld)
        {
          if (Object.op_Inequality((Object) this.All[index].Entity, (Object) null))
            this.Visible.Remove(this.All[index].Entity);
          this.All.RemoveAt(index);
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
  }
}
