// Decompiled with JetBrains decompiler
// Type: SimpleLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class SimpleLight : IOEntity
{
  public override void ResetIOState()
  {
    base.ResetIOState();
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    base.IOStateChanged(inputAmount, inputSlot);
    this.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
  }
}
