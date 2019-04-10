// Decompiled with JetBrains decompiler
// Type: BuildingGradeDecay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class BuildingGradeDecay : Decay
{
  public BuildingGrade.Enum decayGrade;

  public override float GetDecayDelay(BaseEntity entity)
  {
    return this.GetDecayDelay(this.decayGrade);
  }

  public override float GetDecayDuration(BaseEntity entity)
  {
    return this.GetDecayDuration(this.decayGrade);
  }

  public override bool ShouldDecay(BaseEntity entity)
  {
    if (ConVar.Decay.upkeep)
      return true;
    return entity.IsOutside();
  }
}
