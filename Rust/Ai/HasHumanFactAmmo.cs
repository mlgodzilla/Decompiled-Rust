// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasHumanFactAmmo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class HasHumanFactAmmo : BaseScorer
  {
    [ApexSerialization(defaultValue = NPCPlayerApex.AmmoStateEnum.Full)]
    public NPCPlayerApex.AmmoStateEnum value;
    [ApexSerialization]
    public bool requireRanged;
    [ApexSerialization(defaultValue = HasHumanFactAmmo.EqualityEnum.Equal)]
    public HasHumanFactAmmo.EqualityEnum Equality;

    public override float GetScore(BaseContext c)
    {
      if (this.requireRanged && c.GetFact(NPCPlayerApex.Facts.CurrentWeaponType) == (byte) 1)
        return this.Equality <= HasHumanFactAmmo.EqualityEnum.Equal ? 0.0f : 1f;
      byte fact = c.GetFact(NPCPlayerApex.Facts.CurrentAmmoState);
      switch (this.Equality)
      {
        case HasHumanFactAmmo.EqualityEnum.Greater:
          return (NPCPlayerApex.AmmoStateEnum) fact >= this.value ? 0.0f : 1f;
        case HasHumanFactAmmo.EqualityEnum.Gequal:
          return (NPCPlayerApex.AmmoStateEnum) fact > this.value ? 0.0f : 1f;
        case HasHumanFactAmmo.EqualityEnum.Lequal:
          return (NPCPlayerApex.AmmoStateEnum) fact < this.value ? 0.0f : 1f;
        case HasHumanFactAmmo.EqualityEnum.Lesser:
          return (NPCPlayerApex.AmmoStateEnum) fact <= this.value ? 0.0f : 1f;
        default:
          return (NPCPlayerApex.AmmoStateEnum) fact != this.value ? 0.0f : 1f;
      }
    }

    public enum EqualityEnum
    {
      Greater,
      Gequal,
      Equal,
      Lequal,
      Lesser,
    }
  }
}
