// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ProximityToPeers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class ProximityToPeers : WeightedScorerBase<Vector3>
  {
    [ApexSerialization(defaultValue = 14f)]
    public float desiredRange = 14f;

    public override float GetScore(BaseContext c, Vector3 position)
    {
      float num1 = float.MaxValue;
      Vector3 vector3_1 = Vector3.get_zero();
      for (int index = 0; index < c.Memory.All.Count; ++index)
      {
        Memory.SeenInfo memory = c.Memory.All[index];
        if (!Object.op_Equality((Object) memory.Entity, (Object) null))
        {
          float num2 = this.Test(memory, c);
          if ((double) num2 > 0.0)
          {
            Vector3 vector3_2 = Vector3.op_Subtraction(position, memory.Position);
            float num3 = ((Vector3) ref vector3_2).get_sqrMagnitude() * num2;
            if ((double) num3 < (double) num1)
            {
              num1 = num3;
              vector3_1 = memory.Position;
            }
          }
        }
      }
      if (Vector3.op_Equality(vector3_1, Vector3.get_zero()))
        return 0.0f;
      return (float) (1.0 - (double) Vector3.Distance(vector3_1, position) / (double) this.desiredRange);
    }

    protected virtual float Test(Memory.SeenInfo memory, BaseContext c)
    {
      return Object.op_Equality((Object) memory.Entity, (Object) null) || Object.op_Equality((Object) (memory.Entity as BaseNpc), (Object) null) ? 0.0f : 1f;
    }
  }
}
