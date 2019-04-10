// Decompiled with JetBrains decompiler
// Type: Wolf
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class Wolf : BaseAnimalNPC
{
  [ServerVar(Help = "Population active on the server, per square km")]
  public static float Population = 2f;

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
    if (best.HasTrait(BaseEntity.TraitFlag.Meat))
      return true;
    return base.WantsToEat(best);
  }

  public override string Categorize()
  {
    return nameof (Wolf);
  }
}
