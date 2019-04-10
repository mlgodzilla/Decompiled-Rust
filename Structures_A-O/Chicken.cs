// Decompiled with JetBrains decompiler
// Type: Chicken
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Chicken : BaseAnimalNPC
{
  [ServerVar(Help = "Population active on the server, per square km")]
  public static float Population = 3f;

  public override BaseEntity.TraitFlag Traits
  {
    get
    {
      return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
    }
  }

  public override bool WantsToEat(BaseEntity best)
  {
    if (best.HasTrait(BaseEntity.TraitFlag.Alive) || best.HasTrait(BaseEntity.TraitFlag.Meat))
      return false;
    CollectibleEntity collectibleEntity = best as CollectibleEntity;
    if (Object.op_Inequality((Object) collectibleEntity, (Object) null))
    {
      foreach (ItemAmount itemAmount in collectibleEntity.itemList)
      {
        if (itemAmount.itemDef.category == ItemCategory.Food)
          return true;
      }
    }
    return base.WantsToEat(best);
  }

  public override string Categorize()
  {
    return nameof (Chicken);
  }
}
