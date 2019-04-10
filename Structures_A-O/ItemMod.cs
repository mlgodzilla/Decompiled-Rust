// Decompiled with JetBrains decompiler
// Type: ItemMod
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemMod : MonoBehaviour
{
  private ItemMod[] siblingMods;

  public virtual void ModInit()
  {
    this.siblingMods = (ItemMod[]) ((Component) this).GetComponents<ItemMod>();
  }

  public virtual void OnItemCreated(Item item)
  {
  }

  public virtual void OnVirginItem(Item item)
  {
  }

  public virtual void ServerCommand(Item item, string command, BasePlayer player)
  {
  }

  public virtual void DoAction(Item item, BasePlayer player)
  {
  }

  public virtual void OnRemove(Item item)
  {
  }

  public virtual void OnParentChanged(Item item)
  {
  }

  public virtual void CollectedForCrafting(Item item, BasePlayer crafter)
  {
  }

  public virtual void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
  {
  }

  public virtual void OnAttacked(Item item, HitInfo info)
  {
  }

  public virtual void OnChanged(Item item)
  {
  }

  public virtual bool CanDoAction(Item item, BasePlayer player)
  {
    foreach (ItemMod siblingMod in this.siblingMods)
    {
      if (!siblingMod.Passes(item))
        return false;
    }
    return true;
  }

  public virtual bool Passes(Item item)
  {
    return true;
  }

  public virtual void OnRemovedFromWorld(Item item)
  {
  }

  public virtual void OnMovedToWorld(Item item)
  {
  }

  public ItemMod()
  {
    base.\u002Ector();
  }
}
