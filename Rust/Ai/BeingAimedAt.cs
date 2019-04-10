// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BeingAimedAt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public sealed class BeingAimedAt : BaseScorer
  {
    [ApexSerialization]
    public float arc;
    [ApexSerialization]
    public BeingAimedAt.Equality EqualityType;

    public override float GetScore(BaseContext c)
    {
      float num1 = 0.0f;
      int num2 = 0;
      foreach (BaseEntity baseEntity in c.Memory.Visible)
      {
        BasePlayer basePlayer = baseEntity as BasePlayer;
        if (Object.op_Inequality((Object) basePlayer, (Object) null) && !(basePlayer is IAIAgent))
        {
          Vector3 vector3 = basePlayer.eyes.BodyForward();
          float num3 = 0.0f;
          float num4 = Vector3.Dot(c.AIAgent.CurrentAimAngles, vector3);
          switch (this.EqualityType)
          {
            case BeingAimedAt.Equality.Equal:
              num3 = Mathf.Approximately(num4, this.arc) ? 1f : 0.0f;
              break;
            case BeingAimedAt.Equality.LEqual:
              num3 = (double) num4 <= (double) this.arc ? 1f : 0.0f;
              break;
            case BeingAimedAt.Equality.GEqual:
              num3 = (double) num4 >= (double) this.arc ? 1f : 0.0f;
              break;
            case BeingAimedAt.Equality.NEqual:
              num3 = Mathf.Approximately(num4, this.arc) ? 0.0f : 1f;
              break;
            case BeingAimedAt.Equality.Less:
              num3 = (double) num4 < (double) this.arc ? 1f : 0.0f;
              break;
            case BeingAimedAt.Equality.Greater:
              num3 = (double) num4 > (double) this.arc ? 1f : 0.0f;
              break;
          }
          num1 += num3;
          ++num2;
        }
      }
      if (num2 > 0)
        num1 /= (float) num2;
      return num1;
    }

    public enum Equality
    {
      Equal,
      LEqual,
      GEqual,
      NEqual,
      Less,
      Greater,
    }
  }
}
