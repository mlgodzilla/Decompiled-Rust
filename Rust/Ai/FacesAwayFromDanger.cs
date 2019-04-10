// Decompiled with JetBrains decompiler
// Type: Rust.Ai.FacesAwayFromDanger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class FacesAwayFromDanger : WeightedScorerBase<Vector3>
  {
    public override float GetScore(BaseContext c, Vector3 position)
    {
      float num1 = 0.0f;
      Vector3 vector3_1 = position;
      Vector3 position1 = ((Component) c.Entity).get_transform().get_position();
      Vector3 normalized1 = ((Vector3) ref position1).get_normalized();
      Vector3 vector3_2 = Vector3.op_Subtraction(vector3_1, normalized1);
      for (int index = 0; index < c.Memory.All.Count; ++index)
      {
        Vector3 vector3_3 = Vector3.op_Subtraction(c.Memory.All[index].Position, ((Component) c.Entity).get_transform().get_position());
        Vector3 normalized2 = ((Vector3) ref vector3_3).get_normalized();
        float num2 = Vector3.Dot(vector3_2, normalized2);
        num1 += -num2;
      }
      return num1;
    }
  }
}
