// Decompiled with JetBrains decompiler
// Type: ItemPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;

public class ItemPickup : DroppedItem
{
  public int amount = 1;
  public ItemDefinition itemDef;
  public ulong skinOverride;

  public override float GetDespawnDuration()
  {
    return float.PositiveInfinity;
  }

  public override void Spawn()
  {
    base.Spawn();
    if (Application.isLoadingSave != null)
      return;
    Item in_item = ItemManager.Create(this.itemDef, this.amount, this.skinOverride);
    this.InitializeItem(in_item);
    in_item.SetWorldEntity((BaseEntity) this);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.IdleDestroy();
  }
}
