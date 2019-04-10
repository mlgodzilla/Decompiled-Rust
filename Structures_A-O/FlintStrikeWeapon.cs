// Decompiled with JetBrains decompiler
// Type: FlintStrikeWeapon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class FlintStrikeWeapon : BaseProjectile
{
  public float successFraction = 0.5f;
  public RecoilProperties strikeRecoil;

  public override RecoilProperties GetRecoil()
  {
    return this.strikeRecoil;
  }
}
