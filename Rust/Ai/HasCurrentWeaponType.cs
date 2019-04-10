// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasCurrentWeaponType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;

namespace Rust.Ai
{
  public class HasCurrentWeaponType : BaseScorer
  {
    [ApexSerialization(defaultValue = NPCPlayerApex.WeaponTypeEnum.None)]
    public NPCPlayerApex.WeaponTypeEnum value;

    public override float GetScore(BaseContext c)
    {
      return (NPCPlayerApex.WeaponTypeEnum) c.GetFact(NPCPlayerApex.Facts.CurrentWeaponType) != this.value ? 0.0f : 1f;
    }
  }
}
