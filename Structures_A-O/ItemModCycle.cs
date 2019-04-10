// Decompiled with JetBrains decompiler
// Type: ItemModCycle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ItemModCycle : ItemMod
{
  public float timeBetweenCycles = 1f;
  public ItemMod[] actions;
  public float timerStart;
  public bool onlyAdvanceTimerWhenPass;

  public override void OnItemCreated(Item itemcreated)
  {
    float timeTaken = this.timerStart;
    itemcreated.onCycle += (Action<Item, float>) ((item, delta) =>
    {
      if (this.onlyAdvanceTimerWhenPass && !this.CanCycle(item))
        return;
      timeTaken += delta;
      if ((double) timeTaken < (double) this.timeBetweenCycles)
        return;
      timeTaken = 0.0f;
      if (!this.onlyAdvanceTimerWhenPass && !this.CanCycle(item))
        return;
      this.CustomCycle(item, delta);
    });
  }

  private bool CanCycle(Item item)
  {
    foreach (ItemMod action in this.actions)
    {
      if (!action.CanDoAction(item, item.GetOwnerPlayer()))
        return false;
    }
    return true;
  }

  public void CustomCycle(Item item, float delta)
  {
    BasePlayer ownerPlayer = item.GetOwnerPlayer();
    foreach (ItemMod action in this.actions)
      action.DoAction(item, ownerPlayer);
  }

  private void OnValidate()
  {
    if (this.actions != null)
      return;
    Debug.LogWarning((object) "ItemModMenuOption: actions is null", (Object) ((Component) this).get_gameObject());
  }
}
