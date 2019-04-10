// Decompiled with JetBrains decompiler
// Type: Stocking
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;

public class Stocking : LootContainer
{
  public static ListHashSet<Stocking> stockings;

  public override void ServerInit()
  {
    base.ServerInit();
    if (Stocking.stockings == null)
      Stocking.stockings = new ListHashSet<Stocking>(8);
    Stocking.stockings.Add(this);
  }

  internal override void DoServerDestroy()
  {
    Stocking.stockings.Remove(this);
    base.DoServerDestroy();
  }

  public bool IsEmpty()
  {
    if (this.inventory == null)
      return false;
    for (int index = this.inventory.itemList.Count - 1; index >= 0; --index)
    {
      if (this.inventory.itemList[index] != null)
        return false;
    }
    return true;
  }

  public override void SpawnLoot()
  {
    if (this.inventory == null)
    {
      Debug.Log((object) ("CONTACT DEVELOPERS! Stocking::PopulateLoot has null inventory!!! " + ((Object) this).get_name()));
    }
    else
    {
      if (!this.IsEmpty())
        return;
      base.SpawnLoot();
      this.SetFlag(BaseEntity.Flags.On, true, false, true);
      this.Hurt(this.MaxHealth() * 0.1f, DamageType.Generic, (BaseEntity) null, false);
    }
  }

  public override void PlayerStoppedLooting(BasePlayer player)
  {
    base.PlayerStoppedLooting(player);
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    if (!this.IsEmpty() || (double) this.healthFraction > 0.100000001490116)
      return;
    this.Hurt(this.health, DamageType.Generic, (BaseEntity) this, false);
  }
}
