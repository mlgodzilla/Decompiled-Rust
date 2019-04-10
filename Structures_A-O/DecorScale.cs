// Decompiled with JetBrains decompiler
// Type: DecorScale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DecorScale : DecorComponent
{
  public Vector3 MinScale = new Vector3(1f, 1f, 1f);
  public Vector3 MaxScale = new Vector3(2f, 2f, 2f);

  public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
  {
    uint num1 = SeedEx.Seed(pos, World.Seed) + 3U;
    float num2 = SeedRandom.Value(ref num1);
    ref __Null local1 = ref scale.x;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local1 = ^(float&) ref local1 * Mathf.Lerp((float) this.MinScale.x, (float) this.MaxScale.x, num2);
    ref __Null local2 = ref scale.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local2 = ^(float&) ref local2 * Mathf.Lerp((float) this.MinScale.y, (float) this.MaxScale.y, num2);
    ref __Null local3 = ref scale.z;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local3 = ^(float&) ref local3 * Mathf.Lerp((float) this.MinScale.z, (float) this.MaxScale.z, num2);
  }
}
