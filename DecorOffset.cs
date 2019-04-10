// Decompiled with JetBrains decompiler
// Type: DecorOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DecorOffset : DecorComponent
{
  public Vector3 MinOffset = new Vector3(0.0f, 0.0f, 0.0f);
  public Vector3 MaxOffset = new Vector3(0.0f, 0.0f, 0.0f);

  public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
  {
    uint num = SeedEx.Seed(pos, World.Seed) + 1U;
    ref __Null local1 = ref pos.x;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local1 = ^(float&) ref local1 + (float) scale.x * SeedRandom.Range(ref num, (float) this.MinOffset.x, (float) this.MaxOffset.x);
    ref __Null local2 = ref pos.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local2 = ^(float&) ref local2 + (float) scale.y * SeedRandom.Range(ref num, (float) this.MinOffset.y, (float) this.MaxOffset.y);
    ref __Null local3 = ref pos.z;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local3 = ^(float&) ref local3 + (float) scale.z * SeedRandom.Range(ref num, (float) this.MinOffset.z, (float) this.MaxOffset.z);
  }
}
