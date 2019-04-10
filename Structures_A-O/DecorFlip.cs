// Decompiled with JetBrains decompiler
// Type: DecorFlip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DecorFlip : DecorComponent
{
  public DecorFlip.AxisType FlipAxis = DecorFlip.AxisType.Y;

  public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
  {
    uint num = SeedEx.Seed(pos, World.Seed) + 4U;
    if ((double) SeedRandom.Value(ref num) > 0.5)
      return;
    switch (this.FlipAxis)
    {
      case DecorFlip.AxisType.X:
      case DecorFlip.AxisType.Z:
        rot = Quaternion.op_Multiply(Quaternion.AngleAxis(180f, Quaternion.op_Multiply(rot, Vector3.get_up())), rot);
        break;
      case DecorFlip.AxisType.Y:
        rot = Quaternion.op_Multiply(Quaternion.AngleAxis(180f, Quaternion.op_Multiply(rot, Vector3.get_forward())), rot);
        break;
    }
  }

  public enum AxisType
  {
    X,
    Y,
    Z,
  }
}
