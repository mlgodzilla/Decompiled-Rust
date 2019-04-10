// Decompiled with JetBrains decompiler
// Type: BaseDetector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class BaseDetector : IOEntity
{
  public PlayerDetectionTrigger myTrigger;
  public const BaseEntity.Flags Flag_HasContents = BaseEntity.Flags.Reserved1;

  public virtual void OnObjects()
  {
    this.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
    if (!this.IsPowered())
      return;
    this.MarkDirty();
  }

  public virtual void OnEmpty()
  {
    this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
    if (!this.IsPowered())
      return;
    this.MarkDirty();
  }

  public override int ConsumptionAmount()
  {
    return base.ConsumptionAmount();
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    if (!this.HasFlag(BaseEntity.Flags.Reserved1))
      return 0;
    return base.GetPassthroughAmount(0);
  }
}
