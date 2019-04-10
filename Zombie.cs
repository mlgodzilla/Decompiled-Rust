// Decompiled with JetBrains decompiler
// Type: Zombie
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class Zombie : BaseAnimalNPC
{
  [ServerVar(Help = "Population active on the server, per square km")]
  public static float Population;

  public override BaseEntity.TraitFlag Traits
  {
    get
    {
      return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
    }
  }

  public override bool WantsToEat(BaseEntity best)
  {
    if (best.HasTrait(BaseEntity.TraitFlag.Alive))
      return false;
    return base.WantsToEat(best);
  }

  protected override void TickSleep()
  {
    this.Sleep = 100f;
  }

  public override string Categorize()
  {
    return nameof (Zombie);
  }
}
