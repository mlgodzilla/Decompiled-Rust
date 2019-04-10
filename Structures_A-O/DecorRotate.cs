// Decompiled with JetBrains decompiler
// Type: DecorRotate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DecorRotate : DecorComponent
{
  public Vector3 MinRotation = new Vector3(0.0f, -180f, 0.0f);
  public Vector3 MaxRotation = new Vector3(0.0f, 180f, 0.0f);

  public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
  {
    uint num1 = SeedEx.Seed(pos, World.Seed) + 2U;
    float num2 = SeedRandom.Range(ref num1, (float) this.MinRotation.x, (float) this.MaxRotation.x);
    float num3 = SeedRandom.Range(ref num1, (float) this.MinRotation.y, (float) this.MaxRotation.y);
    float num4 = SeedRandom.Range(ref num1, (float) this.MinRotation.z, (float) this.MaxRotation.z);
    rot = Quaternion.op_Multiply(Quaternion.Euler(num2, num3, num4), rot);
  }
}
